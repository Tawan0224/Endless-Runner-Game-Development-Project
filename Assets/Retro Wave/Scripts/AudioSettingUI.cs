using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("Volume Sliders")]
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    
    [Header("Volume Labels")]
    public TextMeshProUGUI musicVolumeLabel;
    public TextMeshProUGUI sfxVolumeLabel;
    
    [Header("Settings")]
    public string volumeLabelFormat = "{0}%";
    
    void Start()
    {
        SetupSliders();
        UpdateUI();
    }
    
    void SetupSliders()
    {
        if (AudioManager.Instance == null) return;
        
        // Setup music volume slider
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = AudioManager.Instance.GetMusicVolume();
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
        
        // Setup SFX volume slider
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = AudioManager.Instance.GetSFXVolume();
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
    }
    
    public void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
            UpdateMusicLabel(value);
        }
    }
    
    public void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
            UpdateSFXLabel(value);
        }
    }
    
    void UpdateUI()
    {
        if (AudioManager.Instance == null) return;
        
        UpdateMusicLabel(AudioManager.Instance.GetMusicVolume());
        UpdateSFXLabel(AudioManager.Instance.GetSFXVolume());
    }
    
    void UpdateMusicLabel(float volume)
    {
        if (musicVolumeLabel != null)
        {
            musicVolumeLabel.text = string.Format(volumeLabelFormat, Mathf.RoundToInt(volume * 100));
        }
    }
    
    void UpdateSFXLabel(float volume)
    {
        if (sfxVolumeLabel != null)
        {
            sfxVolumeLabel.text = string.Format(volumeLabelFormat, Mathf.RoundToInt(volume * 100));
        }
    }
    
    // Method to test SFX volume
    public void TestSFXVolume()
    {
        // You can assign a test sound clip here
        // AudioManager.Instance.PlaySFX(testSound);
    }
}