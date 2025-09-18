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
        
        // Store the rotation you set manually in the Scene view
        if (keepManualRotation)
        {
            initialRotation = transform.eulerAngles;
        }
    }
    
    void LateUpdate()
    {
        if (player == null) return;
        transform.position = player.position + offset;
        if (keepManualRotation)
        {
            transform.eulerAngles = initialRotation;
        }
    }
}