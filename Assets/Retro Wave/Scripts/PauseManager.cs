using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [Header("Pause Menu UI")]
    public GameObject pauseMenuPanel;
    public Button resumeButton;
    public Button restartButton;
    public Button mainMenuButton;
    
    [Header("Pause Settings")]
    public KeyCode pauseKey = KeyCode.Escape;
    public KeyCode alternatePauseKey = KeyCode.P;
    
    [Header("UI Text (Optional)")]
    public TextMeshProUGUI pauseTitle;
    public string pauseTitleText = "PAUSED";
    
    private bool isPaused = false;
    private float originalTimeScale = 1f;
    
    void Start()
    {
        // Initialize
        originalTimeScale = Time.timeScale;
        
        // Make sure pause menu is hidden at start
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        
        // Set up button listeners
        SetupButtonListeners();
        
        // Set pause title if available
        if (pauseTitle != null)
        {
            pauseTitle.text = pauseTitleText;
        }
    }
    
    void Update()
    {
        // Don't allow pausing if game is over
        if (IsGameOver()) return;
        
        // Check for pause input
        if (Input.GetKeyDown(pauseKey) || Input.GetKeyDown(alternatePauseKey))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    
    bool IsGameOver()
    {
        // Check if GameOverManager exists and game is over
        GameOverManager gameOverManager = FindObjectOfType<GameOverManager>();
        if (gameOverManager != null && gameOverManager.IsGameOver())
        {
            return true;
        }
        return false;
    }
    
    void SetupButtonListeners()
    {
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }
        
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(BackToMainMenu);
        }
    }
    
    public void PauseGame()
    {
        // Don't pause if game is over
        if (IsGameOver()) return;
        
        isPaused = true;
        Time.timeScale = 0f; // Freeze the game
        
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }
        
        // Enable cursor for menu navigation
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        Debug.Log("Game Paused");
    }
    
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = originalTimeScale; // Restore normal time
        
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        
        // Hide cursor during gameplay (optional)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Debug.Log("Game Resumed");
    }
    
    public void RestartGame()
    {
        // Reset time scale before loading scene
        Time.timeScale = originalTimeScale;
        
        // Reset current score
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetCurrentScore();
        }
        
        // Reload current scene
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
        
        Debug.Log("Game Restarted");
    }
    
    public void BackToMainMenu()
    {
        // Reset time scale before loading scene
        Time.timeScale = originalTimeScale;
        
        // End current game session (save high score if needed)
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.EndGame();
        }
        
        // Load main menu scene
        SceneManager.LoadScene("TitleScene");
        
        Debug.Log("Returning to Main Menu");
    }
    
    // Public method to check if game is paused (useful for other scripts)
    public bool IsGamePaused()
    {
        return isPaused;
    }
    
    // Public method to toggle pause (can be called by UI buttons)
    public void TogglePause()
    {
        if (IsGameOver()) return; // Don't toggle if game is over
        
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    
    // Ensure time scale is reset when the script is destroyed
    void OnDestroy()
    {
        Time.timeScale = originalTimeScale;
    }
    
    // Handle application focus (pause when window loses focus - optional)
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && !isPaused && !IsGameOver())
        {
            PauseGame();
        }
    }
    
    // Handle application pause (for mobile - optional)
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && !isPaused && !IsGameOver())
        {
            PauseGame();
        }
    }
}