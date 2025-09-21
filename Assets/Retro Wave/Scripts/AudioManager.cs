using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    
    [Header("Music Clips")]
    public AudioClip menuMusic;
    public AudioClip easyLevelMusic;
    public AudioClip hardLevelMusic;
    
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float musicVolume = 0.7f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    
    [Header("Fade Settings")]
    public float fadeInDuration = 1f;
    public float fadeOutDuration = 0.5f;
    
    [Header("Debug")]
    public bool debugAudio = true;
    
    private AudioClip currentMusic;
    private bool isFading = false;
    
    void Awake()
    {
        // Singleton pattern - persist across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Setup audio sources if not assigned
            SetupAudioSources();
            
            // Subscribe to scene loading
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            if (debugAudio)
                Debug.Log("AudioManager initialized");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Set initial volumes
        UpdateVolumes();
        
        // Play appropriate music for current scene
        PlayMusicForCurrentScene();
    }
    
    void SetupAudioSources()
    {
        // Create music source if not assigned
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("Music Source");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
        
        // Create SFX source if not assigned
        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFX Source");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (debugAudio)
            Debug.Log($"Scene loaded: {scene.name}");
            
        // Play appropriate music for the new scene
        PlayMusicForCurrentScene();
    }
    
    void PlayMusicForCurrentScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        AudioClip targetMusic = null;
        
        // Determine which music to play based on scene name
        switch (sceneName.ToLower())
        {
            case "titlescene":
            case "menuscene":
            case "levelscene":
                targetMusic = menuMusic;
                break;
                
            case "easyscene":
                targetMusic = easyLevelMusic;
                break;
                
            case "hardscene":
                targetMusic = hardLevelMusic;
                break;
                
            default:
                if (debugAudio)
                    Debug.Log($"No music assigned for scene: {sceneName}");
                break;
        }
        
        // Only change music if it's different from current
        if (targetMusic != null && targetMusic != currentMusic)
        {
            if (debugAudio)
                Debug.Log($"Switching music for scene: {sceneName}");
                
            PlayMusic(targetMusic);
        }
    }
    
    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip == null || musicSource == null) return;
        
        // Don't restart the same music
        if (currentMusic == musicClip && musicSource.isPlaying) return;
        
        if (isFading) return; // Prevent overlapping fade operations
        
        currentMusic = musicClip;
        
        if (musicSource.isPlaying)
        {
            // Fade out current music, then fade in new music
            StartCoroutine(FadeOutThenIn(musicClip));
        }
        else
        {
            // Start new music with fade in
            musicSource.clip = musicClip;
            musicSource.Play();
            StartCoroutine(FadeIn(musicSource));
        }
        
        if (debugAudio)
            Debug.Log($"Playing music: {musicClip.name}");
    }
    
    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            StartCoroutine(FadeOut(musicSource, true));
            currentMusic = null;
        }
    }
    
    public void PlaySFX(AudioClip sfxClip)
    {
        if (sfxClip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(sfxClip, sfxVolume);
        }
    }
    
    // Convenience method for UI sounds
    public void PlayUISound(AudioClip uiSound)
    {
        PlaySFX(uiSound);
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
        
        // Save to PlayerPrefs
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
        
        // Save to PlayerPrefs
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }
    
    void UpdateVolumes()
    {
        // Load saved volumes
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", musicVolume);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", sfxVolume);
        
        if (musicSource != null)
            musicSource.volume = musicVolume;
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }
    
    // Coroutines for smooth fading
    System.Collections.IEnumerator FadeIn(AudioSource audioSource)
    {
        isFading = true;
        audioSource.volume = 0f;
        
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.unscaledDeltaTime; // Use unscaled time to work during pause
            audioSource.volume = Mathf.Lerp(0f, musicVolume, elapsedTime / fadeInDuration);
            yield return null;
        }
        
        audioSource.volume = musicVolume;
        isFading = false;
    }
    
    System.Collections.IEnumerator FadeOut(AudioSource audioSource, bool stopAfterFade = false)
    {
        isFading = true;
        float startVolume = audioSource.volume;
        
        float elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeOutDuration);
            yield return null;
        }
        
        audioSource.volume = 0f;
        if (stopAfterFade)
        {
            audioSource.Stop();
        }
        isFading = false;
    }
    
    System.Collections.IEnumerator FadeOutThenIn(AudioClip newClip)
    {
        // Fade out current music
        yield return StartCoroutine(FadeOut(musicSource));
        
        // Switch to new music
        musicSource.clip = newClip;
        musicSource.Play();
        
        // Fade in new music
        yield return StartCoroutine(FadeIn(musicSource));
    }
    
    void OnDestroy()
    {
        // Unsubscribe from scene events
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    // Public methods for external control
    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }
    
    public void ResumeMusic()
    {
        if (musicSource != null && !musicSource.isPlaying && currentMusic != null)
        {
            musicSource.UnPause();
        }
    }
    
    public bool IsMusicPlaying()
    {
        return musicSource != null && musicSource.isPlaying;
    }
    
    public float GetMusicVolume()
    {
        return musicVolume;
    }
    
    public float GetSFXVolume()
    {
        return sfxVolume;
    }
}