using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    [Header("References")]
    [SerializeField] SpriteRenderer redDoor;
    [SerializeField] Sprite openDoorSprite;

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
    public void Win()
    {
        // Win effect
        Debug.Log("You Win!");
        Time.timeScale = 0; // For now
    }
    public void StartCriticalWarning()
    {
        // Critical warning effect
        Debug.Log("Critical Warning!");
    }
    public void OpenRedDoor()
    {
        // Play open sound
        redDoor.sprite = openDoorSprite;
        redDoor.GetComponent<Collider2D>().enabled = false;
    }
    #endregion
}
