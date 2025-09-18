using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[DefaultExecutionOrder(1000)]
public class MenuUIHandler : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI highScoreText;
    public string highScoreFormat = "Best Score: {0}";
    
    void Start()
    {
        Debug.Log("MenuUIHandler started");
        UpdateHighScoreDisplay();
    }
    
    void UpdateHighScoreDisplay()
    {
        if (highScoreText != null)
        {
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            highScoreText.text = string.Format(highScoreFormat, highScore.ToString("N0"));
        }
    }

    public void PlayGame()
    {
        Debug.Log("PlayGame button clicked!");
        try
        {
            SceneManager.LoadScene("LevelScene");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load LevelScene: " + e.Message);
        }
    }

    public void Exit()
    {
        Debug.Log("Exit button clicked!");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    // Call this when returning to menu from game
    void OnEnable()
    {
        UpdateHighScoreDisplay();
    }
}