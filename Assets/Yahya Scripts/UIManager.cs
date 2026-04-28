using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Elements")]
    [SerializeField] GameObject blackPanel;
    [SerializeField] GameObject GameOverPanel, PausePanel, WinPanel;
    [SerializeField] TextMeshProUGUI timerTXT;
    [SerializeField] GameObject knifeIMG, pistolIMG;

    private Color originalTimerColor;
    private Vector3 originalTimerScale;
    private Coroutine timerPulseCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (timerTXT != null)
        {
            originalTimerColor = timerTXT.color;
            originalTimerScale = timerTXT.transform.localScale;
        }
    }

    #region Public Methods
    public void UpdateTimer(float time)
    {
        int minutes = Mathf.FloorToInt(Mathf.Max(0, time) / 60);
        int seconds = Mathf.FloorToInt(Mathf.Max(0, time) % 60);
        timerTXT.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void UpdateCurrentWeapon(WeaponType weaponType)
    {
        knifeIMG.SetActive(weaponType == WeaponType.Knife);
        pistolIMG.SetActive(weaponType == WeaponType.Pistol);
    }

    public void ShowTimeAddedEffect(float amount)
    {
        if (GameManager.gameOver) return; // Don't show if game is already over
        ShowFloatingText($"+{Mathf.RoundToInt(amount)}", Color.green);
        TriggerTimerPulse(Color.green);
    }

    public void ShowTimeRemovedEffect(float amount)
    {
        if(GameManager.gameOver) return; // Don't show if game is already over
        ShowFloatingText($"-{Mathf.RoundToInt(amount)}", Color.red);
        TriggerTimerPulse(Color.red);
    }
    public void ShowGameOverPanel()
    {
        blackPanel.SetActive(true);
        GameOverPanel.SetActive(true);
    }

    public void ShowWinPanel()
    {
        blackPanel.SetActive(true);
        WinPanel.SetActive(true);
    }
    #endregion

    #region Private Methods
    private void TriggerTimerPulse(Color pulseColor)
    {
        if (timerTXT == null) return;

        if (timerPulseCoroutine != null)
        {
            StopCoroutine(timerPulseCoroutine);
            timerTXT.transform.localScale = originalTimerScale;
            timerTXT.color = originalTimerColor;
        }
        timerPulseCoroutine = StartCoroutine(TimerPulseRoutine(pulseColor));
    }

    private IEnumerator TimerPulseRoutine(Color pulseColor)
    {
        float duration = 0.3f;
        float elapsed = 0f;

        // Scale up and change color
        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration / 2);
            timerTXT.transform.localScale = Vector3.Lerp(originalTimerScale, originalTimerScale * 1.3f, t);
            timerTXT.color = Color.Lerp(originalTimerColor, pulseColor, t);
            yield return null;
        }

        elapsed = 0f;
        // Scale down and revert color
        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration / 2);
            timerTXT.transform.localScale = Vector3.Lerp(originalTimerScale * 1.3f, originalTimerScale, t);
            timerTXT.color = Color.Lerp(pulseColor, originalTimerColor, t);
            yield return null;
        }

        timerTXT.transform.localScale = originalTimerScale;
        timerTXT.color = originalTimerColor;
    }

    private void ShowFloatingText(string text, Color color)
    {
        if (timerTXT == null) return;

        // Create a new GameObject for the floating text
        GameObject floatingTextObj = new GameObject("FloatingTimeText");
        floatingTextObj.transform.SetParent(timerTXT.transform.parent, false);

        // Position it relative to the timer text
        RectTransform rt = floatingTextObj.AddComponent<RectTransform>();
        rt.localPosition = timerTXT.rectTransform.localPosition + new Vector3(Random.Range(-20f, 20f), Random.Range(-10f, 10f), 0);
        rt.localScale = Vector3.one;

        // Setup TextMeshProUGUI
        TextMeshProUGUI floatingText = floatingTextObj.AddComponent<TextMeshProUGUI>();
        floatingText.text = text;
        floatingText.font = timerTXT.font;
        floatingText.fontSize = timerTXT.fontSize * 0.8f;
        floatingText.color = color;
        floatingText.alignment = TextAlignmentOptions.Center;

        // Start floating animation
        StartCoroutine(FloatingTextRoutine(floatingText, rt));
    }

    private IEnumerator FloatingTextRoutine(TextMeshProUGUI textComponent, RectTransform rt)
    {
        float duration = 1.2f;
        float elapsed = 0f;
        Vector3 startPos = rt.localPosition;
        Vector3 endPos = startPos + new Vector3(0, 60f, 0); // Move upwards locally
        Color startColor = textComponent.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // Use an ease-out curve for smoother motion
            float t = elapsed / duration;
            float easeOutT = 1f - Mathf.Pow(1f - t, 3f);

            // Move upwards
            rt.localPosition = Vector3.Lerp(startPos, endPos, easeOutT);

            // Fade out
            Color newColor = startColor;
            newColor.a = Mathf.Lerp(1f, 0f, t);
            textComponent.color = newColor;

            yield return null;
        }

        Destroy(rt.gameObject);
    }
    #endregion

    #region Buttons
    public void RestartBTN()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void MainMenuBTN()
    {
        SceneManager.LoadScene(0); // Assuming main menu is at index 0
    }
    #endregion
}
