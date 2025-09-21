using UnityEngine;

public class GroundSetup : MonoBehaviour
{
    [Header("Physics Material Settings")]
    public float bounciness = 0.3f;
    public float friction = 0.1f;
    public PhysicsMaterialCombine frictionCombine = PhysicsMaterialCombine.Average;
    public PhysicsMaterialCombine bounceCombine = PhysicsMaterialCombine.Average;
    
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
            // Add BoxCollider for ground
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
        
        // Set ground layer if on default layer
        if (gameObject.layer == 0) 
        {
            // gameObject.layer = LayerMask.NameToLayer("Ground");
        }
    }
}