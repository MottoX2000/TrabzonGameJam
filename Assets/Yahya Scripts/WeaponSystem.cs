using UnityEngine;
using UnityEngine.InputSystem;

public enum WeaponType
{
    None,
    Knife,
    Pistol
}

public class WeaponSystem : MonoBehaviour
{
    [Header("Weapon Ownership")]
    public bool hasKnife = false;
    public bool hasPistol = false;

    [Header("Current State")]
    public WeaponType currentWeapon = WeaponType.None;
    [SerializeField] private Transform firePoint;

    [Header("Knife Settings")]
    [SerializeField] private float knifeRange = 1.5f;
    [SerializeField] private float knifeFireRate = 0.5f;
    [SerializeField] private int knifeDamage = 10;
    [SerializeField] private ParticleSystem knifeEffect;

    [Header("Pistol Settings")]
    [SerializeField] private float pistolRange = 15f;
    [SerializeField] private float pistolFireRate = 0.2f;
    [SerializeField] private int pistolDamage = 20;
    [SerializeField] private int pistolBulletCost = 3;
    [SerializeField] private ParticleSystem pistolEffectPrefab;
    [SerializeField] LayerMask enemyLayerMask;

    private float nextAttackTime = 0f;

    private void Update()
    {
        HandleWeaponSwitching();
    }

    /// <summary>
    /// Grant access to a specific weapon.
    /// </summary>
    public void GrantWeapon(WeaponType weapon)
    {
        if (weapon == WeaponType.Knife) hasKnife = true;
        if (weapon == WeaponType.Pistol) hasPistol = true;

        // Auto-equip if the player has no current weapon
        if (currentWeapon == WeaponType.None)
        {
            EquipWeapon(weapon);
        }
    }

    private void HandleWeaponSwitching()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (currentWeapon == WeaponType.Knife && hasPistol)
            {
                EquipWeapon(WeaponType.Pistol);
            }
            else if (currentWeapon == WeaponType.Pistol && hasKnife)
            {
                EquipWeapon(WeaponType.Knife);
            }
            else if (currentWeapon == WeaponType.None)
            {
                if (hasKnife)
                    EquipWeapon(WeaponType.Knife);
                else if (hasPistol)
                    EquipWeapon(WeaponType.Pistol);
            }
        }
    }

    private void EquipWeapon(WeaponType weapon)
    {
        currentWeapon = weapon;
        // Logic to swap weapon graphics can go here
    }

    public void Attack()
    {
        if (Time.time < nextAttackTime) return;
        if (currentWeapon == WeaponType.None) return;

        switch (currentWeapon)
        {
            case WeaponType.Knife:
                nextAttackTime = Time.time + knifeFireRate;
                KnifeAttack();
                break;
            case WeaponType.Pistol:
                nextAttackTime = Time.time + pistolFireRate;
                TimeManager.Instance.RemoveTime(pistolBulletCost); // Consume bullets as time
                PistolAttack();
                break;
        }
    }

    private void KnifeAttack()
    {
        if (knifeEffect != null) knifeEffect.Play();

        // 2D Near-range directional attack from the firePoint
        Vector2 attackPos = firePoint != null ? firePoint.position : transform.position;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPos, knifeRange);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                // Apply damage to enemy
                enemy.GetComponent<Entity>()?.TakeDamage(knifeDamage);
                Debug.Log("Slashed Enemy!");
            }
        }
    }

    private void PistolAttack()
    {
        // 2D Raycast attack from the firePoint towards the mouse
        Vector2 attackPos = firePoint != null ? firePoint.position : transform.position;

        Vector2 mousePosScreen = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(mousePosScreen);
        mousePos.z = 0f;
        Vector2 attackDir = ((Vector2)mousePos - attackPos).normalized;

        // Visual
        if (pistolEffectPrefab != null)
        {
            ParticleSystem particleSystem = Instantiate(pistolEffectPrefab, firePoint.position + Vector3.back, Quaternion.identity);
            particleSystem.transform.forward = attackDir;
            particleSystem.Play();
            Destroy(particleSystem.gameObject, particleSystem.main.duration);
        }

        RaycastHit2D hit = Physics2D.Raycast(attackPos, attackDir, pistolRange, enemyLayerMask);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                // Apply damage to enemy
                Helper.DoAfterDelay(0.14f, () =>
                {
                    hit.collider.GetComponent<Entity>()?.TakeDamage(pistolDamage);
                    Debug.Log($"Shot Enemy {hit.collider.name}!");
                });
            }
            else
            {
                Debug.Log($"Shot at {hit.collider.name} but it was not an enemy.");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the attack ranges in the editor
        Vector3 attackPos = firePoint != null ? firePoint.position : transform.position;
        Vector3 attackDir = firePoint != null ? firePoint.right : transform.right;

        // Knife
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos, knifeRange);

        // Pistol
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(attackPos, attackPos + attackDir * pistolRange);

    }
}
