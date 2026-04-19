using Unity.VisualScripting;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    public void OnStartButtonClick()
    {
        if (SoundManager.Instance != null && SoundManager.Instance.sfxClips.Length > 0)
        {
            SoundManager.Instance.PlaySFX("ButtonClick");
            /*SoundManager.Instance.PlayMusic("MenuTheme");*/
        }
    }
}