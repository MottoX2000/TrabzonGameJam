using UnityEngine;

public abstract class BaseEnemy : Attacker
{
    protected abstract int RewardTime { get; set; }
}
