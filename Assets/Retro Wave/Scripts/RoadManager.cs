using UnityEngine;
using System.Collections.Generic;

public class RoadManager : MonoBehaviour
{
    public static RoadManager Instance;
    
    [Header("Road Setup")]
    public GameObject[] roadPrefabs;
    public int maxActiveRoads = 5;
    
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
        // Check if maximum roads limit reached
        if (activeRoads.Count >= maxActiveRoads)
        {
            if (showDebug)
            {
                Debug.Log($"Max roads reached ({maxActiveRoads}), not spawning");
            }
            return;
        }
        
        GameObject roadPrefab = GetNextRoadPrefab();
        
        if (roadPrefab == null)
        {
            Debug.LogError("No road prefab available to spawn!");
            return;
        }
        
        // Instantiate road at specified position
        GameObject newRoad = Instantiate(roadPrefab, position, Quaternion.identity);
        activeRoads.Add(newRoad);
        
        if (showDebug)
        {
            Debug.Log($"Spawned road: {newRoad.name} at {position}. Total active roads: {activeRoads.Count}");
        }
        
        // Remove destroyed road references
        activeRoads.RemoveAll(road => road == null);
    }
    
    GameObject GetNextRoadPrefab()
    {
        if (roadPrefabs.Length == 0) return null;
        
        // Cycle through road prefabs for variety
        lastUsedRoadIndex = (lastUsedRoadIndex + 1) % roadPrefabs.Length;
        return roadPrefabs[lastUsedRoadIndex];
    }
    
    // Clear all roads when starting new game
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
        // Clean up destroyed roads from list
        activeRoads.RemoveAll(road => road == null);
        
        // Debug info every 2 seconds
        if (showDebug && Time.frameCount % 120 == 0)
        {
            Debug.Log($"Active roads: {activeRoads.Count}");
        }
    }
}