using UnityEngine;

public class Zombie : BaseZombie
{
    [Header("Temel Ýstatistikler")]
    [SerializeField] private string _entityName;
    [SerializeField] private int _health;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private int _rewardTime;

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

    void Start()
    {
        currentHealth = _health;
        currentMovementSpeed = _movementSpeed;
    }

    /*protected override void Die()
    {
        base.Die();
        TimeManager.Instance.AddTime((flaot)rewardTime);
    }*/

    public override void Attack()
    {
        Debug.Log($"{EntityName} saldýrýyor!");
    }
}
