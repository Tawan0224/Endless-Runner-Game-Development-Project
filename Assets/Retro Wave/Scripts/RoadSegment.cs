// SimpleRoadSegment.cs - Attach to each road prefab
using UnityEngine;

public class RoadSegment : MonoBehaviour
{
    [Header("Road Settings")]
    public float roadLength = 100f; // Set this to match your actual road length
    
    [Header("Debug")]
    public bool showDebug = false;
    
    private bool hasSpawnedNext = false;
    
    void Start()
    {
        // Auto-detect road length from the road mesh/collider
        DetectRoadLength();
        
        if (showDebug)
        {
            Debug.Log($"Road {name} initialized with length: {roadLength}");
        }
    }
    
    void DetectRoadLength()
    {
        // Try to get the road length from renderers (the actual road mesh)
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        
        if (renderers.Length > 0)
        {
            Bounds totalBounds = renderers[0].bounds;
            
            foreach (Renderer r in renderers)
            {
                // Skip small objects (likely obstacles/gems)
                if (r.bounds.size.magnitude > 10f) // Only consider large objects (the road itself)
                {
                    totalBounds.Encapsulate(r.bounds);
                }
            }
            
            roadLength = totalBounds.size.z;
            
            if (showDebug)
            {
                Debug.Log($"Auto-detected road length: {roadLength} for {name}");
            }
        }
    }
    
    void Update()
    {
        // Check if we should spawn the next road
        if (!hasSpawnedNext && ShouldSpawnNext())
        {
            hasSpawnedNext = true;
            SpawnNextRoad();
        }
        
        // Check if this road should be deleted
        if (ShouldDelete())
        {
            DeleteThisRoad();
        }
    }
    
    bool ShouldSpawnNext()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return false;
        
        Vector3 playerPos = player.transform.position;
        Vector3 roadEnd = transform.position + (Vector3.forward * roadLength * 0.5f);
        
        // Spawn next road when player is halfway through this road
        float spawnTriggerDistance = roadLength * 0.5f;
        float distanceToEnd = roadEnd.z - playerPos.z;
        
        bool shouldSpawn = distanceToEnd < spawnTriggerDistance;
        
        if (showDebug && shouldSpawn)
        {
            Debug.Log($"Road {name}: Spawning next road. Player Z: {playerPos.z}, Road End Z: {roadEnd.z}, Distance to end: {distanceToEnd}");
        }
        
        return shouldSpawn;
    }
    
    void SpawnNextRoad()
    {
        if (RoadManager.Instance == null)
        {
            Debug.LogError("RoadManager not found!");
            return;
        }
        
        // Simple approach: spawn the next road one full road length ahead + extra spacing
        float totalDistance = roadLength + RoadManager.Instance.extraSpacing;
        Vector3 spawnPosition = transform.position + (Vector3.forward * totalDistance);
        
        RoadManager.Instance.SpawnRoad(spawnPosition);
        
        if (showDebug)
        {
            Debug.Log($"Road {name}: Current position {transform.position}, spawning next road at: {spawnPosition}, distance: {totalDistance} (road: {roadLength} + spacing: {RoadManager.Instance.extraSpacing})");
        }
    }
    
    bool ShouldDelete()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return false;
        
        Vector3 playerPos = player.transform.position;
        Vector3 roadEnd = transform.position + (Vector3.forward * roadLength * 0.5f);
        
        // Delete when player is well past this road
        float deleteDistance = roadLength + 50f; // Extra safety margin
        bool shouldDelete = playerPos.z > roadEnd.z + deleteDistance;
        
        if (showDebug && shouldDelete)
        {
            Debug.Log($"Road {name}: Should delete. Player Z: {playerPos.z}, Road End Z: {roadEnd.z}");
        }
        
        return shouldDelete;
    }
    
    void DeleteThisRoad()
    {
        if (showDebug)
        {
            Debug.Log($"Deleting road: {name}");
        }
        
        Destroy(gameObject);
    }
    
    // Show road bounds in Scene view
    void OnDrawGizmos()
    {
        if (!showDebug) return;
        
        // Draw road bounds
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(20f, 2f, roadLength));
        
        // Draw spawn point
        Vector3 spawnPoint = transform.position + (Vector3.forward * roadLength);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawnPoint, 5f);
        
        // Draw road direction
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector3.forward * roadLength * 0.5f);
    }
}

