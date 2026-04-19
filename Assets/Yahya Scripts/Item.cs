using UnityEngine;

public class Item : MonoBehaviour
{
    public string item;
    [SerializeField] GameObject info;
    bool destroyed = false;

    private void Start()
    {
        info.SetActive(false);
    }

    public void InfoActivate(bool value)
    {
        if (value == info.activeSelf) return; // No need to change if already in the desired state

        info.SetActive(value);

        if (value)
        {
            Helper.DoAfterDelay(2f, () => { if (!destroyed) InfoActivate(false); }); // Automatically hide info after 3 seconds
        }
    }

    private void OnDestroy()
    {
        destroyed = true; // Mark as destroyed to prevent delayed actions from trying to access this object
    }
}
