using System.Threading;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Attacker
{
    [Header("Temel Ýstatistikler")]
    [SerializeField] private string _entityName;
    [SerializeField] private int _health;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _damage;
    [SerializeField] private float _attackRange;

    [Header("Silahlar")]
    [SerializeField] private GameObject knifePrefab;
    private GameObject _currentWeapon;

    private Vector2 _moveInput;

    protected override string EntityName => _entityName;
    protected override int Health { get => currentHealth; set => currentHealth = value; }
    protected override float MovementSpeed { get => currentMovementSpeed; set => currentMovementSpeed = value; }
    protected override float AttackDamage { get => _damage; set => _damage = value; }

    public GameObject CurrentWeapon => _currentWeapon;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        currentHealth = _health;
        currentMovementSpeed = _movementSpeed;

        if (knifePrefab != null)
        {
            GameObject spawnedKnife = Instantiate(knifePrefab, transform);
            spawnedKnife.transform.localPosition = Vector3.zero;
            _currentWeapon = spawnedKnife;
        }
    }

    void Update()
    {
        if (currentHealth <= 0) Die();
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
        Vector2 movement = _moveInput.normalized * currentMovementSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(_rb.position + movement);
        /*if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX("PlayerDeath");*/
    }

    public override void Attack()
    {
        Debug.Log("Saldýrý yapýldý!");

        if(_currentWeapon != null)
        {
            if (_currentWeapon.TryGetComponent<Knife>(out Knife knife))
            {
                knife.Attack();
            }
            else if (_currentWeapon.TryGetComponent<Gun>(out Gun gun))
            {
                gun.Attack();
            }
        }

        if (!_currentWeapon)
        {
            ApplyDirectDamageToNearestEnemy();
        }
    }

    private void ApplyDirectDamageToNearestEnemy()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, _attackRange);
        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (var enemyCollider in hitEnemies)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                float distance = Vector2.Distance(transform.position, enemyCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemyCollider.gameObject;
                }
            }
        }

        if (closestEnemy != null)
        {
            if (closestEnemy.TryGetComponent<BaseEnemy>(out BaseEnemy enemyScript))
            {
                enemyScript.TakeDamage((int)_damage);
                Debug.Log(closestEnemy.name + " isimli düþmana " + _damage + " hasar verildi!");
                // SoundManager.Instance.PlaySFX("Punch");
            }
        }
    }

    public override void TakeDamage(int damage)
    {
        TimeManager.Instance?.RemoveTime(damage);

        Debug.Log($"{EntityName} {damage} hasar aldý! Kalan can: {Health}");

        /*if (SoundManager.Instance != null && )
            SoundManager.Instance.PlaySFX("PlayerHurt");*/

        if (TimeManager.Instance != null)
            TimeManager.Instance.RemoveTime(damage);
    }

    protected override void Die()
    {
        base.Die();
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX("PlayerDeath");
    }

    private void OnDestroy()
    {
        Die();
    }
}