using System.Threading;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Temel istatistikler")]
    [SerializeField] private float _movementSpeed;

    [Header("Componenets")]
    private Rigidbody2D _rb;

    [Header("Dependencies")]
    [SerializeField] WeaponSystem _weaponSystem;

    private Vector2 _moveInput;


    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (TimeManager.Instance.currentTime <= 0) Die();
    }

    void FixedUpdate()
    {
        Move();
    }

    public void SetMovementInput(Vector2 input)
    {
        _moveInput = input;
    }

    private void Move()
    {
        Vector2 movement = _moveInput.normalized * _movementSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(_rb.position + movement);
        /*if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX("PlayerDeath");*/
    }

    public void Attack()
    {
        _weaponSystem?.Attack();
    }

    public void TakeDamage(int damage)
    {
        TimeManager.Instance?.RemoveTime(damage);
        Debug.Log($"{name} {damage} hasar aldý! Kalan can: {(int)TimeManager.Instance.currentTime}");

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