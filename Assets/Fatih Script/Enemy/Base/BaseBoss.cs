using UnityEngine;

public abstract class BaseBoss : Entity
{
    protected abstract int RewardTime { get; set; }

    protected abstract void Ulti();

    protected override void Die()
    {
        TimeManager.Instance.AddTime((float)RewardTime);
        base.Die();
    }
}
