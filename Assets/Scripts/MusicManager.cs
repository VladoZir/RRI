using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    public AudioSource musicSource;

    public AudioClip mainMenuMusic;
    public AudioClip level1Music;
    public AudioClip level2Music;
    public AudioClip level3Music;

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
            return;
        }

        // Primijeni mute stanje iz PlayerPrefs
        bool isMuted = PlayerPrefs.GetInt("musicMuted", 0) == 1;
        musicSource.mute = isMuted;
    }

    public void SetMute(bool mute)
    {
        musicSource.mute = mute;
        PlayerPrefs.SetInt("musicMuted", mute ? 1 : 0);
    }

    public bool IsMuted()
    {
        return musicSource.mute;
    }

    public void PlayMusicForScene(string sceneName)
    {
        AudioClip newClip = null;

        switch (sceneName)
        {
            case "MainMenu":
                newClip = mainMenuMusic;
                break;
            case "Level1":
                newClip = level1Music;
                break;
            case "Level2":
                newClip = level2Music;
                break;
            case "Level3":
                newClip = level3Music;
                break;
            case "Intro":
                newClip = null;
                break;
            case "YouWin!":
                newClip = null;
                break;
        }


        if (newClip == null)
        {
            if (musicSource.isPlaying)
                musicSource.Stop();
            musicSource.clip = null;
        }
        else if (musicSource.clip != newClip)
        {
            musicSource.clip = newClip;
            musicSource.Play();
        }
    }
}
