using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    private Player _player;
    [SerializeField] private float interactionRange = 2f;

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

        /*if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            _player.TakeDamage(10);
        }*/

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            CheckInteraction();
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            _player.Attack();
        }
    }

    private void CheckInteraction()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, interactionRange);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Item"))
            {
                // Buy Pistol system can be implemented here, for now we just log the interaction

                Debug.Log(hitCollider.gameObject.name + " itemýyla etkileþime geçildi!");
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}