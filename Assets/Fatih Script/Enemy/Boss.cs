using UnityEngine;

public class Boss : BaseBoss
{
    [Header("Temel Ýstatistikler")]
    [SerializeField] private string _entityName;
    [SerializeField] private int _health;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private int _rewardTime;
    [SerializeField] private float _durationOfStunned;

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

    protected override void Ulti()
    {
        Debug.Log($"{EntityName} ulti saldýrýsý yapýyor!");
    }
}