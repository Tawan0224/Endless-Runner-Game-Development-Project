using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 250f;
    public float strafeSpeed = 200f;

    [Header("Hovering Settings")]
    public float hoverHeight = 2f;
    public float hoverForce = 500f;
    public float hoverDamping = 50f;
    public float fallMultiplier = 5f;

    [Header("Banking Settings")]
    public float maxBankAngle = 45f;
    public float bankSpeed = 5f;

    [Header("Ground Detection")]
    public LayerMask groundLayer = -1;
    public float groundCheckDistance = 10f;
    public Transform groundCheckPoint;

    [Header("Bounce Settings")]
    public float bounceForce = 100f;
    public float minBounceVelocity = 2f;
    public bool enableBouncing = true;
    public bool debugBouncing = true;

    [Header("Fall Settings")]
    public float maxFallSpeed = 50f;
    public bool enableFastFall = true;

    private Rigidbody rb;
    private float currentBankAngle = 0f;
    private Vector3 originalRotation;
    private bool isGrounded = false;
    private bool justBounced = false;
    private float bounceTimer = 0f;
    private float bounceDisableTime = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.freezeRotation = false;

        // Prevent unwanted spinning
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

        // Use continuous collision detection
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // Store original rotation
        originalRotation = transform.eulerAngles;

        // Ensure vehicle has a collider
        Collider vehicleCollider = GetComponent<Collider>();
        if (vehicleCollider == null)
        {
            vehicleCollider = gameObject.AddComponent<BoxCollider>();
            Debug.Log("Added BoxCollider to vehicle");
        }

        // Setup ground check point if not assigned
        if (groundCheckPoint == null)
        {
            GameObject checkPoint = new GameObject("GroundCheckPoint");
            checkPoint.transform.SetParent(transform);
            checkPoint.transform.localPosition = Vector3.down * 0.5f;
            groundCheckPoint = checkPoint.transform;
        }
        
        // Ensure player has correct tag
        if (!gameObject.CompareTag("Player"))
        {
            gameObject.tag = "Player";
        }
    }

    void Update()
    {
        // Don't update movement if game is stopped
        if (IsGameStopped()) return;
        
        CheckGroundStatus();
        HandleMovement();
        HandleHovering();
        HandleBanking();
        HandleFastFall();

        // Update bounce timer
        if (justBounced)
        {
            bounceTimer += Time.deltaTime;
            if (bounceTimer >= bounceDisableTime)
            {
                justBounced = false;
                bounceTimer = 0f;
            }
        }
    }
    
    bool IsGameStopped()
    {
        // Check if game is over
        if (GameOverManager.Instance != null && GameOverManager.Instance.IsGameOver())
        {
            return true;
        }
        
        // Check if game is paused
        if (FindObjectOfType<PauseManager>() != null)
        {
            PauseManager pauseManager = FindObjectOfType<PauseManager>();
            if (pauseManager.IsGamePaused())
            {
                return true;
            }
        }
        
        return false;
    }

    void CheckGroundStatus()
    {
        Vector3 rayOrigin = groundCheckPoint ? groundCheckPoint.position : transform.position;
        RaycastHit hit;
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, out hit, groundCheckDistance, groundLayer);

        // Draw debug ray in scene view
        if (debugBouncing)
        {
            Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
        }
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");

        Vector3 movement = Vector3.zero;
        movement += transform.forward * forwardSpeed;
        movement += transform.right * horizontal * strafeSpeed;

        // Apply horizontal movement only
        Vector3 newVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
        rb.linearVelocity = newVelocity;
    }

    void HandleHovering()
    {
        // Don't hover immediately after bouncing
        if (justBounced) return;

        Vector3 rayOrigin = groundCheckPoint ? groundCheckPoint.position : transform.position;
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, groundCheckDistance, groundLayer))
        {
            float currentHeight = hit.distance;
            float heightDifference = hoverHeight - currentHeight;

            // Only apply hover force if close to target height
            if (currentHeight <= hoverHeight * 2f)
            {
                // Calculate hover force based on height difference
                float force = heightDifference * hoverForce;

                // Add damping to reduce oscillation
                force -= rb.linearVelocity.y * hoverDamping;

                // Apply hover force
                rb.AddForce(Vector3.up * force);
            }

            if (debugBouncing)
            {
                Debug.DrawLine(rayOrigin, hit.point, Color.blue);
            }
        }
        else
        {
            // Apply stronger gravity if no ground detected
            rb.AddForce(Physics.gravity * fallMultiplier, ForceMode.Acceleration);
        }
    }

    void HandleFastFall()
    {
        if (!enableFastFall) return;

        // Apply additional downward force when falling
        if (rb.linearVelocity.y < 0 && !isGrounded)
        {
            rb.AddForce(Vector3.down * fallMultiplier * 100f, ForceMode.Force);
        }

        // Limit maximum fall speed
        if (rb.linearVelocity.y < -maxFallSpeed)
        {
            Vector3 velocity = rb.linearVelocity;
            velocity.y = -maxFallSpeed;
            rb.linearVelocity = velocity;
        }
    }

    void HandleBanking()
    {
        float horizontal = Input.GetAxis("Horizontal");

        // Calculate target bank angle based on input
        float targetBankAngle = -horizontal * maxBankAngle;

        // Smoothly interpolate to target angle
        currentBankAngle = Mathf.Lerp(currentBankAngle, targetBankAngle, Time.deltaTime * bankSpeed);

        // Apply banking while preserving physics rotation
        Vector3 currentEuler = transform.eulerAngles;
        transform.eulerAngles = new Vector3(currentEuler.x, currentEuler.y, originalRotation.z + currentBankAngle);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (debugBouncing)
        {
            Debug.Log($"Collision detected with: {collision.gameObject.name}, Layer: {collision.gameObject.layer}");
            Debug.Log($"Velocity: {rb.linearVelocity.y}, Min required: {-minBounceVelocity}");
        }

        if (enableBouncing)
        {
            HandleGroundBounce(collision);
        }
    }

    bool IsGroundCollision(Collision collision)
    {
        // Check if collision is with ground layer
        bool isGround = (groundLayer.value & (1 << collision.gameObject.layer)) > 0;
        if (debugBouncing && isGround)
        {
            Debug.Log($"Ground collision confirmed with: {collision.gameObject.name}");
        }
        return isGround;
    }

    void HandleGroundBounce(Collision collision)
    {
        // Check if this is a ground collision
        if (!IsGroundCollision(collision))
        {
            if (debugBouncing)
                Debug.Log("Not a ground collision, skipping bounce");
            return;
        }

        // Get collision normal
        Vector3 collisionNormal = collision.contacts[0].normal;

        if (debugBouncing)
        {
            Debug.Log($"Collision normal: {collisionNormal}");
            Debug.Log($"Current velocity Y: {rb.linearVelocity.y}");
        }

        // Only bounce if moving downward with sufficient speed
        if (rb.linearVelocity.y < -minBounceVelocity)
        {
            if (debugBouncing)
                Debug.Log("BOUNCING!");

            // Apply gentle bounce
            Vector3 currentVelocity = rb.linearVelocity;
            currentVelocity.y = bounceForce;
            rb.linearVelocity = currentVelocity;

            // Set bounce state to temporarily disable hover
            justBounced = true;
            bounceTimer = 0f;
        }
        else
        {
            if (debugBouncing)
                Debug.Log($"Not bouncing - velocity too low: {rb.linearVelocity.y} (need < {-minBounceVelocity})");
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Don't process collisions if game is stopped
        if (IsGameStopped()) return;
        
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("Player hit an obstacle!");
        }
        
        if (other.CompareTag("Gem"))
        {
            Debug.Log("Player collected a gem!");
        }
    }

    // Adjust bounce settings during gameplay
    public void SetBounceSettings(float force, float minVelocity, bool enabled)
    {
        bounceForce = force;
        minBounceVelocity = minVelocity;
        enableBouncing = enabled;
    }

    // Adjust hover settings during gameplay
    public void SetHoverSettings(float height, float force, float damping, float fallMult = 3f)
    {
        hoverHeight = height;
        hoverForce = force;
        hoverDamping = damping;
        fallMultiplier = fallMult;
    }

    // Reset banking when player stops moving
    public void ResetBanking()
    {
        currentBankAngle = 0f;
        Vector3 currentEuler = transform.eulerAngles;
        transform.eulerAngles = new Vector3(currentEuler.x, currentEuler.y, originalRotation.z);
    }
    
    // Stop player movement when game over
    public void StopPlayer()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
    
    // Disable player input
    public void DisableMovement()
    {
        this.enabled = false;
    }
    
    // Re-enable player input
    public void EnableMovement()
    {
        this.enabled = true;
    }
}