using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Time Settings")]
    [SerializeField] private float startTime = 45;

    [Header("Time Status")]
    public float currentTime = 0;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentTime = startTime;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimer();
    }

    #region Private Methods
    void UpdateTimer()
    {
        currentTime -= Time.deltaTime;     
        UIManager.Instance.UpdateTimer(currentTime);

        if (currentTime >= 60)
        {
            SoundManager.Instance.PlayMusic("ThemeSound1");
        }
        else if (currentTime < 60 && currentTime > 30)
        {
            SoundManager.Instance.PlayMusic("ThemeSound2");
        } 
        else if (currentTime <= 10)
        {
            SoundManager.Instance.PlayMusic("ThemeSound3");
            GameManager.Instance.StartCriticalWarning();
        }
    }
    #endregion

    #region Public Methods
    public void AddTime(float timeToAdd)
    {
        // Add effect
        currentTime += timeToAdd;
    }
    public void RemoveTime(float timeToRemove) 
    {
        // Remove effect
        currentTime -= timeToRemove;
    }
    public void SetTimer(float newTime)
    {
        currentTime = newTime;
    }
    #endregion
}
