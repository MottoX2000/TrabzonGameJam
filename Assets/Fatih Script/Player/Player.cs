using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Temel istatistikler")]
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float cooldown = 1f;

    [Header("Dash Settings")]
    [SerializeField] private float _dashMultiplier = 3f;
    [SerializeField] private float _dashDuration = 0.2f;
    [SerializeField] private float _dashCooldown = 1f;
    [SerializeField] private int _dashTimeCost = 2;
    private float _dashTimeCounter;
    private float _dashCooldownCounter;

    [Header("Componenets")]
    [SerializeField] private Animator animator;
    private Rigidbody2D _rb;

    [Header("Dependencies")]
    [SerializeField] Transform gfx;
    [SerializeField] WeaponSystem _weaponSystem;

    private Vector2 _moveInput;
    private float cooldownCounter;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (TimeManager.Instance.currentTime <= 0) Die();

        TrackDashSystem();
        CooldownCounterSystem();
    }

    void FixedUpdate()
    {
        Move();
    }

    public void SetMovementInput(Vector2 input)
    {
        _moveInput = input;
    }

    void TrackDashSystem()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && _dashCooldownCounter <= 0)
        {
            TimeManager.Instance?.RemoveTime(_dashTimeCost); // Dash time cost
            _dashTimeCounter = _dashDuration;
            _dashCooldownCounter = _dashCooldown;
        }

        if (_dashTimeCounter > 0)
        {
            _dashTimeCounter -= Time.deltaTime;
        }

        if (_dashCooldownCounter > 0)
        {
            _dashCooldownCounter -= Time.deltaTime;
        }
    }

    void CooldownCounterSystem()
    {
        if (cooldownCounter > 0)
        {
            cooldownCounter -= Time.deltaTime;
            animator.SetBool("inDamage", true);
        }
        else
        {
            animator.SetBool("inDamage", false);
        }
    }

    private void Move()
    {
        float currentSpeed = (_dashTimeCounter > 0) ? _movementSpeed * _dashMultiplier : _movementSpeed;
        Vector2 movement = _moveInput.normalized * currentSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(_rb.position + movement);

        if (_moveInput.x != 0 && !_weaponSystem.IsAttacking) gfx.localScale = new Vector3(_moveInput.x >= 0 ? 1 : -1, 1, 1);
        animator.SetBool("running", _moveInput.magnitude > 0);

        /*if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX("PlayerDeath");*/
    }

    public void Attack()
    {
        _weaponSystem?.Attack();
    }

    public void TakeDamage(int damage)
    {
        if (cooldownCounter > 0) return; // Ignore damage if still in cooldown

        TimeManager.Instance?.RemoveTime(damage);
        Debug.Log($"{name} {damage} hasar aldý! Kalan can: {(int)TimeManager.Instance.currentTime}");
        cooldownCounter = cooldown;

        /*if (SoundManager.Instance != null && )
            SoundManager.Instance.PlaySFX("PlayerHurt");*/
    }

    protected void Die()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX("PlayerDeath");
        GameManager.Instance?.GameOver();
    }

    private void OnDestroy()
    {
        Die();
    }
}