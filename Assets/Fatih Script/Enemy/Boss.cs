using UnityEngine;

public class Boss : BaseEnemy
{
    [Header("Core Stats")]
    [SerializeField] private int _health;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _damage;
    [SerializeField] private int _rewardTime;
    [SerializeField] private float _durationOfStunned = 2f;

    [Header("AI Settings")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float chargeAcceleration = 10f;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private GameObject stunEffect;

    public float Accelerate { get; private set; }
    private Vector2 _lastUpdatePosition;
    private float _lastSpeed;

    bool died = false;
    private bool _isAttacking = false;
    private bool _isStunned = false;
    private float _nextAttackTime = 0f;
    private Vector2 _attackDirection;
    private float _attackStartTime;


    protected override int Health
    {
        get => currentHealth;
        set => currentHealth = value;
    }
    protected override float MovementSpeed
    {
        get => currentMovementSpeed;
        set => currentMovementSpeed = value;
    }
    protected override int RewardTime
    {
        get => _rewardTime;
        set => _rewardTime = value;
    }

    protected override float AttackDamage
    {
        get => _damage;
        set => _damage = value;
    }


    void Start()
    {
        currentHealth = _health;
        currentMovementSpeed = _movementSpeed;
        _rb = GetComponent<Rigidbody2D>();
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Helps prevent passing through walls at high speeds
        
        _lastUpdatePosition = transform.position;
        InvokeRepeating(nameof(TrackAcceleration), 0f, 0.2f);
    }

    private void TrackAcceleration()
    {
        Vector2 currentPosition = transform.position;
        float distance = Vector2.Distance(currentPosition, _lastUpdatePosition);
        float currentSpeed = distance / 0.2f;
        
        Accelerate = (currentSpeed - _lastSpeed) / 0.2f;
        
        _lastSpeed = currentSpeed;
        _lastUpdatePosition = currentPosition;
    }

    private void Update()
    {
        if (currentHealth <= 0 && !died)
        {
            died = true;
            if (animator != null) animator.SetTrigger("death");
            Die();
            return;
        }

        if (animator != null)
        {
            stunEffect.SetActive(_isStunned);
            animator.SetBool("walking", _isAttacking);
        }

        if (died || _isStunned || _playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);

        if (Accelerate < 0.2f && Time.time - _attackStartTime > 0.5f)
            EndAttack(); // If acceleration drops too low, end the attack (e.g., hit a wall or obstacle)

        if (_isAttacking)
        {
            // Being handled in Attack coroutine
            return;
        }

        if (distanceToPlayer <= detectionRange)
        {
            if (Time.time >= _nextAttackTime)
            {
                Attack();
            }
            else
            {
                // Optionally face the player while waiting
                Vector2 direction = (_playerTransform.position - transform.position).normalized;
                if (direction.x > 0) transform.localScale = new Vector3(1, 1, 1);
                else if (direction.x < 0) transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isAttacking && collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent<Player>(out Player playerScript))
            {
                playerScript.TakeDamage((int)_damage);
            }
            EndAttack();
        }
        else if (_isAttacking && !collision.gameObject.CompareTag("Enemy")) // Assumed hit wall
        {
            EndAttack();
        }
    }

    private void EndAttack()
    {
        _isAttacking = false;
        _rb.linearVelocity = Vector2.zero;
        _isStunned = true;
        _nextAttackTime = Time.time + attackCooldown + _durationOfStunned;

        Helper.DoAfterDelay(_durationOfStunned, () =>
        {
            _isStunned = false;
        });
    }

    private void FixedUpdate()
    {
        if (_isAttacking && !_isStunned && !died)
        {
            float t = Time.time - _attackStartTime;
            // Parabolic speed increase: v = v0 + a * t^2
            float currentChargeSpeed = currentMovementSpeed + (chargeAcceleration * t * t);
            
            // MovePosition recalculates physics constraints to prevent going through colliders
            _rb.MovePosition(_rb.position + _attackDirection * currentChargeSpeed * Time.fixedDeltaTime);
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        TimeManager.Instance.AddTime(3);
        animator.SetTrigger("getDamage");
        Debug.Log($"{name} {damage} hasar aldi! Kalan can: {Health}");
    }

    protected override void Die()
    {
        TimeManager.Instance.AddTime(_rewardTime);
        Destroy(gameObject, 3f); // Delay to let death animation play
    }

    public override void Attack()
    {
        if (_isAttacking) return;

        SoundManager.Instance.PlayMusic("BossStage");
        Debug.Log($"{name} ulti saldýrýsý yapýyor!");
        _isAttacking = true;

        _attackDirection = (_playerTransform.position - transform.position).normalized;
        _attackStartTime = Time.time;

        if (_attackDirection.x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (_attackDirection.x < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}