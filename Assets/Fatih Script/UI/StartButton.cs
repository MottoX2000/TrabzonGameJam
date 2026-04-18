using UnityEngine;

public class StartButton : MonoBehaviour
{
    public void OnStartButtonClick()
    {
        if (SoundManager.Instance != null && SoundManager.Instance.sfxClips.Length > 0)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.sfxClips[0].name);
            SoundManager.Instance.PlayMusic(SoundManager.Instance.musicClips[1].name);
        }
    }
}