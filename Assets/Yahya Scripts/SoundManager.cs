using UnityEngine;
using System;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    public SoundClip[] musicClips;
    public SoundClip[] sfxClips;

    private void Awake()
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

    public void PlayMusic(string name)
    {
        SoundClip s = Array.Find(musicClips, x => x.name == name);

        if (s == null || musicSource == null)
        {
            Debug.LogWarning("Müzik bulunamadý veya MusicSource atanmadý: " + name);
            return;
        }

        if (musicSource.clip == s.clip && musicSource.isPlaying) return;

        musicSource.clip = s.clip;

        musicSource.volume = s.volume > 0 ? s.volume : 1.0f;

        musicSource.loop = s.loop;

        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.enabled = false;
        }
    }

    public void PlaySFX(string name)
    {
        SoundClip s = Array.Find(sfxClips, x => x.name == name);

        if (s == null || sfxSource == null)
        {
            Debug.LogWarning("Ses bulunamadý veya SfxSource atanmadý!");
            return;
        }

        float volumeToPlay = s.volume > 0 ? s.volume : 1.0f;
        sfxSource.PlayOneShot(s.clip, volumeToPlay);
    }
}

[Serializable]
public class SoundClip
{
    public string name;
    public AudioClip clip;
    public float volume;
    public bool loop;
}