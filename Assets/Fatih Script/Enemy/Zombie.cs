using UnityEngine;

public class Zombie : BaseEnemy
{
    [Header("Temel Ýstatistikler")]
    [SerializeField] private string _entityName;
    [SerializeField] private int _health;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private int _rewardTime;
    [SerializeField] private float _damage;

    [Header("AI Ayarlarý")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackCooldown = 1f;

    private Transform _playerTransform;
    private float _nextAttackTime;

    protected override string EntityName => _entityName;
    protected override int Health { get => currentHealth; set => currentHealth = value; }
    protected override float MovementSpeed { get => currentMovementSpeed; set => currentMovementSpeed = value; }
    protected override int RewardTime { get => _rewardTime; set => _rewardTime = value; }
    protected override float AttackDamage { get => _damage; set => _damage = value; }

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) _playerTransform = playerObj.transform;
    }

    void Start()
    {
        currentHealth = _health;
        currentMovementSpeed = _movementSpeed;
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        if (_playerTransform != null)
        {
            HandleAI();
        }
    }

    private void HandleAI()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);

        if (distanceToPlayer <= attackRange)
        {
            if (Time.time >= _nextAttackTime)
            {
                Attack();
                _nextAttackTime = Time.time + attackCooldown;
            }
            _rb.linearVelocity = Vector2.zero;
        }
        else if (distanceToPlayer <= detectionRange)
        {
            FollowPlayer();
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
        }
    }

    private void FollowPlayer()
    {
        Vector2 direction = (_playerTransform.position - transform.position).normalized;
        _rb.linearVelocity = direction * currentMovementSpeed;

        if (direction.x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        Debug.Log($"{EntityName} {damage} hasar aldý! Kalan can: {Health}");
    }

    protected override void Die()
    {
        TimeManager.Instance.AddTime(_rewardTime);
        base.Die();
    }

    public override void Attack()
    {
        Debug.Log($"{EntityName} oyuncuya pençe atýyor!");

        if (_playerTransform.TryGetComponent<Player>(out Player playerScript))
        {
            playerScript.TakeDamage((int)_damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}