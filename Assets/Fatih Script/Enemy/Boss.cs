using UnityEngine;

public class Boss : BaseEnemy
{
    [Header("Temel Ýstatistikler")]
    [SerializeField] private string _entityName;
    [SerializeField] private int _health;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _damage;
    [SerializeField] private int _rewardTime;
    [SerializeField] private float _durationOfStunned = 2f;

    [Header("AI Ayarlarý")]
    [SerializeField] private float detectionRange = 10f; 
    [SerializeField] private float attackRange = 2f;    
    [SerializeField] private float attackCooldown = 2f; 

    protected override string EntityName => _entityName;
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
    }

    private void Update()
    {
        if (currentHealth <= 0) Die();
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
        Debug.Log($"{EntityName} ulti saldýrýsý yapýyor!");
    }
}