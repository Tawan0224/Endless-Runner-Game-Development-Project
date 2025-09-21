using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Obstacle Settings")]
    public bool destroyOnCollision = true;
    public float destructionDelay = 0.5f;
    
    [Header("Visual Effects")]
    public GameObject collisionEffect;
    public Color obstacleColor = Color.red;
    
    [Header("Audio")]
    public AudioClip collisionSound;
    private AudioSource audioSource;
    
    [Header("Animation (Optional)")]
    public bool rotateObstacle = false;
    public Vector3 rotationSpeed = new Vector3(0, 45, 0);
    public bool enablePulsing = false;
    public float pulseSpeed = 2f;
    public float pulseScale = 0.2f;
    
    private Vector3 originalScale;
    private bool hasCollided = false;
    
    void Start()
    {
        // Ensure obstacle has a collider
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider>();
            Debug.Log("Added BoxCollider to obstacle");
        }
        
        // Set as trigger for collision detection
        col.isTrigger = true;
        
        // Ensure obstacle has correct tag
        if (!gameObject.CompareTag("Obstacle"))
        {
            gameObject.tag = "Obstacle";
            Debug.Log("Set tag to 'Obstacle'");
        }
        
        // Setup audio component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && collisionSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Store original scale for pulsing animation
        originalScale = transform.localScale;
        
        // Apply obstacle color
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            renderer.material.color = obstacleColor;
        }
    }
    
    void Update()
    {
        // Rotation animation
        if (rotateObstacle)
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
        
        // Pulsing animation
        if (enablePulsing)
        {
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseScale;
            transform.localScale = originalScale * pulse;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check for player collision
        if (other.CompareTag("Player") && !hasCollided)
        {
            hasCollided = true;
            HandlePlayerCollision(other);
        }
    }
    
    void HandlePlayerCollision(Collider player)
    {
        Debug.Log($"Player collided with obstacle: {gameObject.name}");
        
        // Play collision sound
        if (audioSource != null && collisionSound != null)
        {
            audioSource.PlayOneShot(collisionSound);
        }
        
        // Spawn collision effect
        if (collisionEffect != null)
        {
            GameObject effect = Instantiate(collisionEffect, transform.position, Quaternion.identity);
            // Auto-destroy effect after 3 seconds
            Destroy(effect, 3f);
        }
        
        // Trigger game over
        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.TriggerGameOver();
        }
        else
        {
            Debug.LogError("GameOverManager.Instance is null! Make sure GameOverManager is in the scene.");
        }
        
        // Disable visual components but keep for sound/effects
        DisableObstacleVisuals();
        
        // Destroy obstacle after delay
        if (destroyOnCollision)
        {
            Destroy(gameObject, destructionDelay);
        }
    }
    
    void DisableObstacleVisuals()
    {
        // Disable renderer
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }
        
        // Disable collider to prevent multiple collisions
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
        
        // Stop animations
        rotateObstacle = false;
        enablePulsing = false;
        
        // Reset scale
        transform.localScale = originalScale;
    }
    
    // Reset obstacle for object pooling
    public void ResetObstacle()
    {
        hasCollided = false;
        
        // Re-enable components
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = true;
        }
        
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
            col.isTrigger = true;
        }
        
        // Reset scale
        transform.localScale = originalScale;
        
        // Reset tag
        gameObject.tag = "Obstacle";
    }
    
    // Set obstacle properties dynamically
    public void SetObstacleProperties(Color color, bool shouldDestroy = true, float delay = 0.5f)
    {
        obstacleColor = color;
        destroyOnCollision = shouldDestroy;
        destructionDelay = delay;
        
        // Apply color
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            renderer.material.color = obstacleColor;
        }
    }
}