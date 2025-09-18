using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    
    [Header("Animation Settings")]
    public bool animateScoreChange = true;
    public float animationDuration = 0.5f;
    
    [Header("Score Display Format")]
    public string scorePrefix = "Score: ";
    public string highScorePrefix = "Best: ";
    
    private Vector3 originalScale;
    private Coroutine animationCoroutine;
    
    void Start()
    {
        // Store original scale for animation
        if (scoreText != null)
        {
            originalScale = scoreText.transform.localScale;
        }
        
        // Subscribe to score events (simplified - no events needed)
        if (ScoreManager.Instance != null)
        {
            // Initialize display
            UpdateScoreDisplay(ScoreManager.Instance.GetCurrentScore());
            UpdateHighScoreDisplay(ScoreManager.Instance.GetHighScore());
        }
        
        // Update UI every frame (simple approach)
        InvokeRepeating("UpdateUI", 0f, 0.1f);
    }
    
    void UpdateUI()
    {
        if (ScoreManager.Instance != null)
        {
            UpdateScoreDisplay(ScoreManager.Instance.GetCurrentScore());
            UpdateHighScoreDisplay(ScoreManager.Instance.GetHighScore());
        }
    }
    
    void UpdateScoreDisplay(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = scorePrefix + newScore.ToString();
            
            if (animateScoreChange)
            {
                AnimateScoreText();
            }
        }
    }
    
    void UpdateHighScoreDisplay(int newHighScore)
    {
        if (highScoreText != null)
        {
            highScoreText.text = highScorePrefix + newHighScore.ToString();
        }
    }
    
    void AnimateScoreText()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        animationCoroutine = StartCoroutine(AnimateTextScale(scoreText.transform));
    }
    
    System.Collections.IEnumerator AnimateTextScale(Transform textTransform)
    {
        float elapsedTime = 0f;
        Vector3 targetScale = originalScale * 1.2f;
        
        // Scale up
        while (elapsedTime < animationDuration / 2f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (animationDuration / 2f);
            textTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        
        elapsedTime = 0f;
        
        // Scale back down
        while (elapsedTime < animationDuration / 2f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (animationDuration / 2f);
            textTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }
        
        textTransform.localScale = originalScale;
    }
    
    // Method to manually refresh the UI
    public void RefreshUI()
    {
        if (ScoreManager.Instance != null)
        {
            UpdateScoreDisplay(ScoreManager.Instance.GetCurrentScore());
            UpdateHighScoreDisplay(ScoreManager.Instance.GetHighScore());
        }
    }
    
    void OnDestroy()
    {
        CancelInvoke("UpdateUI");
    }
}