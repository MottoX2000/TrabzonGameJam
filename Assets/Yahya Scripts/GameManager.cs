using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public static bool gameOver = false;

    [Header("References")]
    [SerializeField] SpriteRenderer redDoor;
    [SerializeField] Sprite openDoorSprite;

    private void Awake()
    {
        Instance = this;
        gameOver = false; // Reset game over state when the game starts
    }

    #region Public Methods
    public void GameOver()
    {
        // Game Over effect
        gameOver = true;
        SoundManager.Instance.StopMusic();
        SoundManager.Instance.PlaySFX("GameOver");
        TimeManager.Instance.SetTimer(0); // Ensure timer is at 0
        Debug.Log("Game Over!");
        Helper.DoAfterDelay(2f, () => UIManager.Instance.ShowGameOverPanel());
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
        SoundManager.Instance.PlaySFX("DoorOpen");
    }
    #endregion
}
