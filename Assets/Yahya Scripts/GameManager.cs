using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    #region Public Methods
    public void GameOver()
    {
        // Game Over effect
        TimeManager.Instance.SetTimer(0); // Ensure timer is at 0
        Debug.Log("Game Over!");
        Time.timeScale = 0; // For now
    }
    public void StartCriticalWarning()
    {
        // Critical warning effect
        Debug.Log("Critical Warning!");
    }
    #endregion
}
