using UnityEngine;

public class GemCollectible : MonoBehaviour
{
    [Header("Gem Settings")]
    public int gemValue = 10;
    public AudioClip collectSound;
    
    [Header("Visual Effects")]
    public GameObject collectEffect;
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
        
        // Setup audio component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && collectSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Ensure trigger collider exists
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
        
        // Bob up and down animation
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
        // Add points to score
        ScoreManager.Instance.AddScore(gemValue);
        
        // Play collection sound
        if (audioSource != null && collectSound != null)
        {
            audioSource.PlayOneShot(collectSound);
        }
        
        // Spawn particle effect
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }
        
        // Hide visual components but keep GameObject for sound
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        
        // Destroy after sound finishes
        float destroyDelay = (collectSound != null) ? collectSound.length : 0f;
        Destroy(gameObject, destroyDelay);
    }
}