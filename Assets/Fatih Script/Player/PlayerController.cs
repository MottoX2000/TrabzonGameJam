using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    private Player _player;
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] float infoRange = 6f;

    void Awake()
    {
        _player = GetComponent<Player>();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        float x = 0;
        float y = 0;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) y = 1;
        else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) y = -1;

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x = 1;
        else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) x = -1;

        _player.SetMovementInput(new Vector2(x, y));

        CheckInteraction(true); // Detect nearby interactables for UI feedback

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            CheckInteraction();
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            _player.Attack();
        }
    }

    private void CheckInteraction(bool detectOnly = false)
    {
        Collider2D[] hitColliders = detectOnly ? Physics2D.OverlapCircleAll(transform.position, infoRange) 
            : Physics2D.OverlapCircleAll(transform.position, interactionRange);

        foreach (var hitCollider in hitColliders)
        {
            var item = hitCollider.GetComponent<Item>();
            if (item != null)
            {
                if (detectOnly)
                {
                    item.InfoActivate(true);
                }
                else
                {

                    // Buy Pistol system can be implemented here, for now we just log the interaction
                    CollectItem(item);
                    Debug.Log(hitCollider.gameObject.name + " itemýyla etkileþime geçildi!");
                    break;
                }
            }
        }
    }

    public void CollectItem(Item item)
    {
        switch (item.item)
        {
            case "Key":
                GameManager.Instance.OpenRedDoor();
                break;
            case "Knife":
                WeaponSystem.Instance.GrantWeapon(WeaponType.Knife);
                break;
            case "Pistol":
                WeaponSystem.Instance.GrantWeapon(WeaponType.Pistol);
                break;
            case "Exit Door":
                GameManager.Instance.Win();
                return;
            default:
                break;
        }

        Destroy(item.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, infoRange);
    }
}