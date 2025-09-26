
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource; // For background music
    [SerializeField] private AudioSource sfxSource;   // For sound effects

    // We use a custom class to make it easy to manage clips in the Inspector
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
    }

    [Header("Audio Clips")]
    public Sound[] musicTracks;
    public Sound[] sfxClips;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // This method is called whenever a new scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;

        // Play the correct background music based on the scene
        if (sceneName == "VehicleSelectionUI" || sceneName == "LevelSelectUI")
        {
            PlayMusic("MainBackground");
        }
        else if (sceneName == "EndlessScene" || sceneName == "LevelScene")
        {
            PlayMusic("GameBackground");
        }
    }

    public void PlayMusic(string trackName)
    {
        Sound s = Array.Find(musicTracks, sound => sound.name == trackName);
        if (s == null)
        {
            Debug.LogWarning("Music track: " + trackName + " not found!");
            return;
        }

        // Avoid restarting the music if it's already playing
        if (musicSource.clip == s.clip) return;

        musicSource.clip = s.clip;
        musicSource.Play();
    }

    public void PlaySFX(string sfxName)
    {
        Sound s = Array.Find(sfxClips, sound => sound.name == sfxName);
        if (s == null)
        {
            Debug.LogWarning("SFX clip: " + sfxName + " not found!");
            return;
        }
        // PlayOneShot allows multiple sound effects to play without cutting each other off
        sfxSource.PlayOneShot(s.clip);
    }
}
