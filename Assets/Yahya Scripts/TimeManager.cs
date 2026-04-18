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

        //Death condition
        if (currentTime <= 0)
        {
            currentTime = 0;
            GameManager.Instance.GameOver();
        }
        else if (currentTime <= 10)
        {
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
    #endregion
}
