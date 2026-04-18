using UnityEngine;
using System.Collections;

public class Knife : MonoBehaviour
{
    [Header("Özellikler")]
    [SerializeField] private int _damage = 2;
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private float _attackDuration = 0.15f;

    private bool _isAttacking = false;
    private Vector3 _originalLocalPos;

    void Start()
    {
        _originalLocalPos = transform.localPosition;
    }

    public void Attack()
    {
        if (_isAttacking) return;
        StartCoroutine(KnifeAttackRoutine());
    }

    private IEnumerator KnifeAttackRoutine()
    {
        _isAttacking = true;

        Vector3 targetPos = _originalLocalPos + Vector3.right * _attackRange;
        float elapsed = 0f;

        while (elapsed < _attackDuration)
        {
            transform.localPosition = Vector3.Lerp(_originalLocalPos, targetPos, elapsed / _attackDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        CheckForDamage();

        elapsed = 0f;
        while (elapsed < _attackDuration)
        {
            transform.localPosition = Vector3.Lerp(targetPos, _originalLocalPos, elapsed / _attackDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = _originalLocalPos;
        _isAttacking = false;
    }

    private void CheckForDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircle(transform.position, 0.5f).GetComponents<Collider2D>();

        foreach (var col in Physics2D.OverlapCircleAll(transform.position, 0.8f))
        {
            if (col.CompareTag("Enemy") && col.TryGetComponent<BaseEnemy>(out BaseEnemy zombie))
            {
                zombie.TakeDamage(_damage);
                Debug.Log("Býçak saplandý!");
                break;
            }
        }
    }
}