using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    [Header("Görsel Ayarlarý")]
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite clickedSprite;

    private Sprite _originalSprite;

    void Start()
    {
        if (buttonImage == null)
            buttonImage = GetComponent<Image>();

        if (buttonImage != null)
            _originalSprite = buttonImage.sprite;
    }

    public void OnStartButtonClick()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX("ButtonClick");
        }

        if (buttonImage != null && clickedSprite != null)
        {
            buttonImage.sprite = clickedSprite;
        }
    }

    public void ResetSprite()
    {
        if (buttonImage != null && _originalSprite != null)
        {
            buttonImage.sprite = _originalSprite;
        }
    }
}