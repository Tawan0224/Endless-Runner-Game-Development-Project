using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PauseButton : MonoBehaviour
{
    private PauseManager pauseManager;
    private Button pauseButton;
    
    void Start()
    {
        // Find pause manager in scene
        pauseManager = FindObjectOfType<PauseManager>();
        
        if (pauseManager == null)
        {
            Debug.LogError("PauseManager not found in scene!");
            return;
        }
        
        // Setup button click event
        pauseButton = GetComponent<Button>();
        pauseButton.onClick.AddListener(OnPauseButtonClicked);
    }
    
    void OnPauseButtonClicked()
    {
        if (pauseManager != null)
        {
            pauseManager.PauseGame();
        }
    }
    
    void Update()
    {
        // Disable button when game is already paused
        if (pauseManager != null && pauseButton != null)
        {
            pauseButton.interactable = !pauseManager.IsGamePaused();
        }
    }
}