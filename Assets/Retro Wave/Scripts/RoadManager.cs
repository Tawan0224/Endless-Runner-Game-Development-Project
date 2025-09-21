// SimpleRoadManager.cs - Place on empty GameObject in scene
using UnityEngine;
using System.Collections.Generic;

public class RoadManager : MonoBehaviour
{
    public static RoadManager Instance;
    
    [Header("Road Setup")]
    public GameObject[] roadPrefabs; // Your road prefabs with obstacles/gems attached
    public int maxActiveRoads = 5; // Keep this many roads active
    public float extraSpacing = 10f; // Add extra space between roads to prevent overlap
    
    [Header("Debug")]
    public bool showDebug = false;
    
    private List<GameObject> activeRoads = new List<GameObject>();
    private int lastUsedRoadIndex = -1;
    
    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        if (roadPrefabs.Length == 0)
        {
            Debug.LogError("No road prefabs assigned to RoadManager!");
        }
        
        if (showDebug)
        {
            Debug.Log($"RoadManager initialized with {roadPrefabs.Length} road prefabs");
        }
    }
    
    public void SpawnRoad(Vector3 position)
    {
        // Don't spawn if we have too many roads
        if (activeRoads.Count >= maxActiveRoads)
        {
            if (showDebug)
            {
                Debug.Log($"Max roads reached ({maxActiveRoads}), not spawning");
            }
            return;
        }
        
        // Select next road prefab (cycle through them)
        GameObject roadPrefab = GetNextRoadPrefab();
        
        if (roadPrefab == null)
        {
            Debug.LogError("No road prefab available to spawn!");
            return;
        }
        
        // Spawn the road directly at the position (no complex adjustments)
        GameObject newRoad = Instantiate(roadPrefab, position, Quaternion.identity);
        activeRoads.Add(newRoad);
        
        if (showDebug)
        {
            Debug.Log($"Spawned road: {newRoad.name} at {position}. Total active roads: {activeRoads.Count}");
        }
        
        // Clean up null references
        activeRoads.RemoveAll(road => road == null);
    }
    
    GameObject GetNextRoadPrefab()
    {
        if (roadPrefabs.Length == 0) return null;
        
        // Cycle through road prefabs to add variety
        lastUsedRoadIndex = (lastUsedRoadIndex + 1) % roadPrefabs.Length;
        return roadPrefabs[lastUsedRoadIndex];
    }
    
    // Call this when starting a new game to clear old roads
    public void ClearAllRoads()
    {
        foreach (GameObject road in activeRoads)
        {
            if (road != null)
            {
                Destroy(road);
            }
        }
        activeRoads.Clear();
        
        if (showDebug)
        {
            Debug.Log("Cleared all active roads");
        }
    }
    
    void Update()
    {
        // Clean up destroyed roads from the list
        activeRoads.RemoveAll(road => road == null);
        
        if (showDebug && Time.frameCount % 120 == 0) // Every 2 seconds
        {
            Debug.Log($"Active roads: {activeRoads.Count}");
        }
    }
}