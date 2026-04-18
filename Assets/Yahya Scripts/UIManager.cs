using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI timerTXT;

    private void Awake()
    {
        Instance = this;
    }

    #region Public Methods
    public void UpdateTimer(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerTXT.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    #endregion
}
