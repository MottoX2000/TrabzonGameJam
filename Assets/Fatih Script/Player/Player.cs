using UnityEngine;

public class Player : BasePlayer
{
    [Header("Temel Ýstatistikler")]
    [SerializeField] private string _entityName;
    [SerializeField] private int _health;
    [SerializeField] private float _movementSpeed;

    [Header("Saldýrý Eþyalarý")]
    [SerializeField] private GameObject knife;
    [SerializeField] private GameObject gun;

    private Rigidbody2D _rb;
    private Vector2 _moveInput;
    private Vector2 _mousePos;

    protected override string EntityName => _entityName;
    protected override int Health
    {
        get => currentHealth;
        set => currentHealth = value;
    }
    protected override float MovementSpeed
    {
        get => currentMovementSpeed;
        set => currentMovementSpeed = value;
    }

    void Start()
    {
        currentHealth = _health;
        currentMovementSpeed = _movementSpeed;

        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //currentHealth -= Time.deltaTime;
        if (currentHealth <= 0) Die(); // manager ý çaðýr

        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");
        _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0)) Attack();
        if (Input.GetKeyDown(KeyCode.E)) Interaction();
    }

    private void FixedUpdate()
    {
        Move();
    }

    public override void Attack()
    {
        Debug.Log("Saldýrý yapýldý!");
    }

    private void Interaction()
    {
    }

    private void Move()
    {
        _rb.MovePosition(_rb.position + _moveInput.normalized * _movementSpeed * Time.fixedDeltaTime);

        Vector2 lookDir = _mousePos - _rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        _rb.rotation = angle;
    }

    protected override void Die()
    {
        base.Die();
    }
}