using UnityEngine;

public abstract class BaseZombie : Entity
{
    protected abstract int RewardTime { get; set; }

    public abstract void Attack();

    protected override void Die()
    {
        TimeManager.Instance.AddTime((float)RewardTime);
        base.Die();
    }
}

