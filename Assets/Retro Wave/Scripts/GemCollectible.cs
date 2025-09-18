using UnityEngine;

public class GemCollectible : MonoBehaviour
{
    [Header("Gem Settings")]
    public int gemValue = 10;
    public AudioClip collectSound;
    
    [Header("Visual Effects")]
    public GameObject collectEffect; // Particle effect prefab
    public bool rotateGem = true;
    public float rotationSpeed = 90f;
    
    [Header("Bob Animation")]
    public bool enableBobbing = true;
    public float bobHeight = 0.5f;
    public float bobSpeed = 2f;
    
    private Vector3 startPosition;
    private AudioSource audioSource;
    
    void Start()
    {
        startPosition = transform.position;
        
        // Get or create AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && collectSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Ensure we have a trigger collider
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            col = gameObject.AddComponent<SphereCollider>();
        }
        col.isTrigger = true;
    }
    
    void Update()
    {
        // Rotate the gem
        if (rotateGem)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
        
        // Bob up and down
        if (enableBobbing)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectGem();
        }
    }
    
    void CollectGem()
    {
        // Add to score
        ScoreManager.Instance.AddScore(gemValue);
        
        // Play sound effect
        if (audioSource != null && collectSound != null)
        {
            audioSource.PlayOneShot(collectSound);
        }
        
        // Spawn particle effect
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }
        
        // Disable visual components but keep GameObject for sound
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        
        // Destroy after sound finishes (or immediately if no sound)
        float destroyDelay = (collectSound != null) ? collectSound.length : 0f;
        Destroy(gameObject, destroyDelay);
    }
}