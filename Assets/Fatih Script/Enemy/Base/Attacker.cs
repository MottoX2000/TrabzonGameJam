using UnityEngine;

public abstract class Attacker : Entity
{
    protected abstract float AttackDamage { get; set; }

    public abstract void Attack();
}
