using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    protected abstract string EntityName { get; }
    protected abstract int Health { get; set; }
    protected abstract float MovementSpeed { get; set; }

    protected int currentHealth;
    protected float currentMovementSpeed;

    protected Rigidbody2D _rb;

    public virtual void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0) Die();
    }

    protected virtual void Die()
    {
        Debug.Log($"{EntityName} öldü.");
        Destroy(gameObject);
    }
}
