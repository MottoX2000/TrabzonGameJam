using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Özellikler")]
    private int _damage;

    public int Damage => _damage;

    void Start()
    {
        
    }

    public void Initialize(int damage)
    {
        this._damage = damage;
    }

    void Update()
    {
        
    }
}
