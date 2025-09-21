using UnityEngine;

public class RoadSegment : MonoBehaviour
{
    [Header("Road Settings")]
    public float roadLength = 100f;
    
    [Header("Overlap Settings")]
    public float overlapAmount = 5f;
    
    [Header("Debug")]
    public bool showDebug = false;
    
    private bool hasSpawnedNext = false;
    
    void Start()
    {
        // Auto-detect road length from mesh/collider
        DetectRoadLength();
        
        if (showDebug)
        {
            Debug.Log($"Road {name} initialized with length: {roadLength}");
        }
    }
    
    void DetectRoadLength()
    {
        // Get road length from renderers
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        
        if (renderers.Length > 0)
        {
            Bounds totalBounds = renderers[0].bounds;
            
            foreach (Renderer r in renderers)
            {
                // Only consider large objects (the road mesh, not obstacles/gems)
                if (r.bounds.size.magnitude > 10f)
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
        // Check if next road should be spawned
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
        
        // Calculate spawn position with overlap to prevent gaps
        float spawnDistance = roadLength - overlapAmount;
        Vector3 spawnPosition = transform.position + (Vector3.forward * spawnDistance);
        
        RoadManager.Instance.SpawnRoad(spawnPosition);
        
        if (showDebug)
        {
            Debug.Log($"Road {name}: Current position {transform.position}, spawning next road at: {spawnPosition}");
            Debug.Log($"Spawn distance: {spawnDistance} (road length: {roadLength} - overlap: {overlapAmount})");
        }
    }
    
    bool ShouldDelete()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return false;
        
        Vector3 playerPos = player.transform.position;
        Vector3 roadStart = transform.position - (Vector3.forward * roadLength * 0.5f);
        
        // Delete when player is well past this road
        float deleteDistance = roadLength + 50f;
        bool shouldDelete = playerPos.z > roadStart.z + deleteDistance;
        
        if (showDebug && shouldDelete)
        {
            Debug.Log($"Road {name}: Should delete. Player Z: {playerPos.z}, Road Start Z: {roadStart.z}");
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
    
    // Visualize road bounds in Scene view
    void OnDrawGizmos()
    {
        if (!showDebug) return;
        
        // Draw road bounds
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(20f, 2f, roadLength));
        
        // Draw spawn point for next road
        Vector3 spawnPoint = transform.position + (Vector3.forward * (roadLength - overlapAmount));
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawnPoint, 5f);
        
        // Draw overlap area
        Gizmos.color = Color.yellow;
        Vector3 overlapCenter = transform.position + (Vector3.forward * (roadLength * 0.5f - overlapAmount * 0.5f));
        Gizmos.DrawWireCube(overlapCenter, new Vector3(20f, 1f, overlapAmount));
        
        // Draw road direction
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, Vector3.forward * roadLength * 0.5f);
    }
}