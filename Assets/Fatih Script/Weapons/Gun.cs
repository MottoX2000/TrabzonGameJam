using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Özellikler")]
    [SerializeField] private int damage;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;
    [SerializeField] private int ammoCapacity;
    [SerializeField] private float reloadTime;

    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;


    void Start()
    {

    }

    void Update()
    {

    }

    public void Attack()
    {
        Debug.Log("Gun saldýrýsý yapýldý!");

        GameObject obje = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = obje.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Initialize(damage);
        }
    }
}
