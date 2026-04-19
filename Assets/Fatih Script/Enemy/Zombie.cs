using UnityEngine;

public class Zombie : BaseEnemy
{
    [Header("Temel Ýstatistikler")]
    [SerializeField] Animator animator;
    [SerializeField] private string _entityName;
    [SerializeField] private int _health;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private int _rewardTime;
    [SerializeField] private float _damage = 5;

    [Header("AI Ayarlarý")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 0.8f;
    [SerializeField] private float attackCooldown = 1f;

    private Transform _playerTransform;
    private float _nextAttackTime;

    private ZombieSoundController _soundController; // FATÝH

    bool died = false;

    protected override int Health { get => currentHealth; set => currentHealth = value; }
    protected override float MovementSpeed { get => currentMovementSpeed; set => currentMovementSpeed = value; }
    protected override int RewardTime { get => _rewardTime; set => _rewardTime = value; }
    protected override float AttackDamage { get => _damage; set => _damage = value; }
    public bool IsDead => died;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _soundController = GetComponent<ZombieSoundController>(); // FATÝH
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
        if (died)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

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
            animator.SetBool("walking", false);
            if (Time.time >= _nextAttackTime)
            {
                Attack();
                _nextAttackTime = Time.time + attackCooldown;
            }
            _rb.linearVelocity = Vector2.zero;
        }
        else if (distanceToPlayer <= detectionRange)
        {
            animator.SetBool("walking", true);
            FollowPlayer();
            _soundController?.PlayWalkSound(); // FATÝH
        }
        else
        {
            animator.SetBool("walking", false);
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
        if (died) return;

        base.TakeDamage(damage);
        animator.SetTrigger("getDamage");
        _soundController?.PlayHurtSound(); // FATÝH
        Debug.Log($"{_entityName} {damage} hasar aldý! Kalan can: {Health}");
    }

    protected override void Die()
    {
        if (died) return;

        died = true;
        animator.SetTrigger("death");
        _soundController?.PlayDeathSound(); // FATÝH
        TimeManager.Instance.AddTime(_rewardTime);
        Destroy(gameObject, 3f);
    }

    public override void Attack()
    {
        Debug.Log($"{_entityName} oyuncuya pence atiyor!");

        animator.SetTrigger("attack");
        _soundController?.PlayAttackSound(); // FATÝH
        Helper.DoAfterDelay(0.3f, () =>
        {
            float distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);
            if (_playerTransform.TryGetComponent<Player>(out Player playerScript) && distanceToPlayer <= attackRange)
            {
                playerScript.TakeDamage((int)_damage);
            }
        });
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}