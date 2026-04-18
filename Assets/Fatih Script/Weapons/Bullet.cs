using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Özellikler")]
    [SerializeField] private int damage;
    [SerializeField] private float attackRange;

    void Start()
    {
        
    }

    public void Initialize(int damage, float attackRange)
    {
        this.damage = damage;
        this.attackRange = attackRange;
    }

    void Update()
    {
        
    }
}
