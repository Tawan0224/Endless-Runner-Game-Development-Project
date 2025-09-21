using UnityEngine;

public class PositionOnlyFollow : MonoBehaviour
{
    [Header("Setup")]
    public Transform player;
    
    [Header("Offset from Player")]
    public Vector3 offset = new Vector3(0, 25, -40);
    
    [Header("Keep Manual Rotation")]
    public bool keepManualRotation = true;
    
    private Vector3 initialRotation;
    
    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
        
        // Store manually set rotation from Scene view
        if (keepManualRotation)
        {
            initialRotation = transform.eulerAngles;
        }
    }
    
    void LateUpdate()
    {
        if (player == null) return;
        
        // Follow player position with offset
        transform.position = player.position + offset;
        
        // Keep initial rotation if enabled
        if (keepManualRotation)
        {
            transform.eulerAngles = initialRotation;
        }
    }
}