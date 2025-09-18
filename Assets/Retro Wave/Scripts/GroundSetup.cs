using UnityEngine;

public class GroundSetup : MonoBehaviour
{
    [Header("Physics Material Settings")]
    public float bounciness = 0.3f; // Reduced from 1.0f for gentler bouncing
    public float friction = 0.1f; // Small amount of friction for more realistic physics
    public PhysicsMaterialCombine frictionCombine = PhysicsMaterialCombine.Average;
    public PhysicsMaterialCombine bounceCombine = PhysicsMaterialCombine.Average; // Changed from Maximum
    
    [Header("Debug")]
    public bool debugSetup = true;
    
    void Start()
    {
        SetupGroundPhysics();
    }
    
    void SetupGroundPhysics()
    {
        // Get or add collider
        Collider groundCollider = GetComponent<Collider>();
        if (groundCollider == null)
        {
            // Add BoxCollider for simple ground 
            groundCollider = gameObject.AddComponent<BoxCollider>();
            if (debugSetup)
                Debug.Log($"Added BoxCollider to {gameObject.name}");
        }
        
        // Create and assign physics material
        PhysicsMaterial groundMaterial = new PhysicsMaterial("GroundMaterial");
        groundMaterial.bounciness = bounciness;
        groundMaterial.staticFriction = friction;
        groundMaterial.dynamicFriction = friction;
        groundMaterial.frictionCombine = frictionCombine;
        groundMaterial.bounceCombine = bounceCombine;
        
        groundCollider.material = groundMaterial;
        
        if (debugSetup)
        {
            Debug.Log($"Ground setup complete for {gameObject.name}");
            Debug.Log($"Layer: {gameObject.layer}, Bounciness: {bounciness}");
        }
        if (gameObject.layer == 0) 
        {
            // gameObject.layer = LayerMask.NameToLayer("Ground");
        }
    }
}