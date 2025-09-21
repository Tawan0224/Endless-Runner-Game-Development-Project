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
        
        // Hide pause menu at start
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        
        // Setup button click events
        SetupButtonListeners();
        
        // Set pause title
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
        Time.timeScale = 0f;
        
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
        Time.timeScale = originalTimeScale;
        
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        
        // Hide cursor during gameplay
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
        
        // End current game session
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.EndGame();
        }
        
        // Load main menu scene
        SceneManager.LoadScene("TitleScene");
        
        Debug.Log("Returning to Main Menu");
    }
    
    // Check if game is paused
    public bool IsGamePaused()
    {
        return isPaused;
    }
    
    // Toggle pause state
    public void TogglePause()
    {
        if (IsGameOver()) return;
        
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    
    // Ensure time scale is reset when destroyed
    void OnDestroy()
    {
        Time.timeScale = originalTimeScale;
    }
    
    // Pause when window loses focus
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && !isPaused && !IsGameOver())
        {
            PauseGame();
        }
    }
    
    // Pause when application is paused (mobile)
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && !isPaused && !IsGameOver())
        {
            PauseGame();
        }
    }
}