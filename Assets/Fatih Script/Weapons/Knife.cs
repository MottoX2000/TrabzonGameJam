using UnityEngine;

public class Knife : MonoBehaviour
{
    [Header("Özellikler")]
    [SerializeField] private int damage;
    [SerializeField] private float attackRange;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Attack()
    {
        Debug.Log("Knife saldýrýsý yapýldý!");
    }
}
