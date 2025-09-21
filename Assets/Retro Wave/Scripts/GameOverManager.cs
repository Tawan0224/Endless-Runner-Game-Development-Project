using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;
    
    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public Button restartButton;
    public Button mainMenuButton;
    
    [Header("Score Display")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;
    public string finalScoreFormat = "Score: {0}";
    public string highScoreFormat = "Best: {0}";
    
    [Header("Game Over Settings")]
    public float gameOverDelay = 1f;
    
    [Header("Audio (Optional)")]
    public AudioClip gameOverSound;
    private AudioSource audioSource;
    
    private bool isGameOver = false;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        // Initialize panel as hidden
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        // Setup audio component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && gameOverSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Setup button click events
        SetupButtonListeners();
    }
    
    void SetupButtonListeners()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(BackToMainMenu);
        }
    }
    
    public void TriggerGameOver()
    {
        // Prevent multiple calls
        if (isGameOver) return;
        
        isGameOver = true;
        
        Debug.Log("Game Over!");
        
        // Disable pause manager to prevent conflicts
        PauseManager pauseManager = FindObjectOfType<PauseManager>();
        if (pauseManager != null)
        {
            pauseManager.enabled = false;
            Debug.Log("Disabled PauseManager to prevent conflicts");
        }
        
        // Stop game time
        Time.timeScale = 0f;
        
        // Play game over sound
        if (audioSource != null && gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }
        
        // Enable cursor for UI navigation
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Save final score and update high score
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.EndGame();
        }
        
        // Show game over panel immediately
        ShowGameOverPanel();
    }
    
    void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            // Ensure canvas can receive input during pause
            Canvas canvas = gameOverPanel.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = 100;
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
        }
        
        // Update score displays
        UpdateScoreDisplays();
    }
    
    void UpdateScoreDisplays()
    {
        if (ScoreManager.Instance == null) return;
        
        int finalScore = ScoreManager.Instance.GetCurrentScore();
        int highScore = ScoreManager.Instance.GetHighScore();
        
        if (finalScoreText != null)
        {
            finalScoreText.text = string.Format(finalScoreFormat, finalScore.ToString("N0"));
        }
        
        if (highScoreText != null)
        {
            highScoreText.text = string.Format(highScoreFormat, highScore.ToString("N0"));
        }
    }
    
    public void RestartGame()
    {
        Debug.Log("Restart button clicked!");
        
        // Re-enable pause manager
        PauseManager pauseManager = FindObjectOfType<PauseManager>();
        if (pauseManager != null)
        {
            pauseManager.enabled = true;
        }
        
        // Reset time scale
        Time.timeScale = 1f;
        
        // Reset score
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetCurrentScore();
        }
        
        // Reload current scene
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }
    
    public void BackToMainMenu()
    {
        Debug.Log("Main menu button clicked!");
        
        // Re-enable pause manager for other scenes
        PauseManager pauseManager = FindObjectOfType<PauseManager>();
        if (pauseManager != null)
        {
            pauseManager.enabled = true;
        }
        
        // Reset time scale
        Time.timeScale = 1f;
        
        // Ensure score is saved
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.EndGame();
        }
        
        // Load main menu scene
        SceneManager.LoadScene("TitleScene");
    }
    
    // Check if game is over
    public bool IsGameOver()
    {
        return isGameOver;
    }
    
    // Reset game over state for continue functionality
    public void ResetGameOverState()
    {
        isGameOver = false;
        Time.timeScale = 1f;
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        // Hide cursor during gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    // Ensure time scale is reset when destroyed
    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}