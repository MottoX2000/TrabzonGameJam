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

    [Header("Knife Settings")]
    [SerializeField] private float knifeRange = 1.5f;
    [SerializeField] private float knifeFireRate = 0.5f;
    [SerializeField] private int knifeDamage = 10;
    [SerializeField] private ParticleSystem knifeEffect;
    [SerializeField] Animator knifeAnimator;
    [SerializeField] Transform knifePoint;

    [Header("Pistol Settings")]
    [SerializeField] private float pistolRange = 15f;
    [SerializeField] private float pistolFireRate = 0.2f;
    [SerializeField] private int pistolDamage = 20;
    [SerializeField] private int pistolBulletCost = 3;
    [SerializeField] private ParticleSystem pistolEffectPrefab;
    [SerializeField] LayerMask enemyLayerMask;
    [SerializeField] private Transform firePoint;
    [SerializeField] Transform pistolTrans;

    private float nextAttackTime = 0f;
    public bool IsAttacking => Time.time < nextAttackTime;

    private void Start()
    {
        EquipWeapon(currentWeapon);
    }

    private void Update()
    {
        HandleWeaponSwitching();
        WeaponDirection();
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

    void WeaponDirection()
    {
        Vector2 mousePosScreen = Mouse.current.position.ReadValue();
        Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePosScreen);
        mousePosWorld.z = 0f;

        Vector2 direction = (mousePosWorld - pistolTrans.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        pistolTrans.rotation = Quaternion.Euler(0, 0, angle);

        // Flip the weapon vertically when aiming left so it doesn't appear upside down
        Vector3 localScale = pistolTrans.localScale;
        if (angle > 90 || angle < -90)
        {
            localScale.y = -Mathf.Abs(localScale.y);
        }
        else
        {
            localScale.y = Mathf.Abs(localScale.y);
        }
        pistolTrans.localScale = localScale;
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

        // Optional: Add number key switching (1 for Knife, 2 for Pistol)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame && hasKnife)
            {
                EquipWeapon(WeaponType.Knife);
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame && hasPistol)
            {
                EquipWeapon(WeaponType.Pistol);
            }
        }
    }

    private void EquipWeapon(WeaponType weapon)
    {
        currentWeapon = weapon;
        UIManager.Instance.UpdateCurrentWeapon(currentWeapon);

        pistolTrans.gameObject.SetActive(weapon == WeaponType.Pistol); 
        knifeAnimator.gameObject.SetActive(weapon == WeaponType.Knife); 
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
        if (SoundManager.Instance != null) // FATÝH
            SoundManager.Instance.PlaySFX("KnifeSound");

        // 2D Near-range directional attack from the knifePoint
        Vector2 attackPos = knifePoint != null ? knifePoint.position : transform.position;
        knifeAnimator.SetTrigger("attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPos, knifeRange, enemyLayerMask);
        foreach (Collider2D enemy in hitEnemies)
        {
            print($"Hit {enemy.name} with knife attack.");
            if (enemy.CompareTag("Enemy"))
            {
                // Apply damage to enemy
                Helper.DoAfterDelay(knifeAnimator.GetCurrentAnimatorStateInfo(0).length / 3, () =>
                {
                    enemy.GetComponent<Entity>()?.TakeDamage(knifeDamage);
                    Debug.Log("Slashed Enemy!");
                });
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

        if (SoundManager.Instance != null) // FATÝH
            SoundManager.Instance.PlaySFX("PistolSound");

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
        Vector3 attackPos = knifePoint != null ? knifePoint.position : transform.position;
        Vector3 attackDir = firePoint != null ? firePoint.right : transform.right;

        // Knife
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos, knifeRange);

        // Pistol
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(attackPos, attackPos + attackDir * pistolRange);

    }
}
