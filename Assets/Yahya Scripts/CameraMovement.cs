using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The target object for the camera to follow (usually the player)")]
    public Transform target;

    [Header("Camera Position")]
    [Tooltip("Offset from the target position")]
    public Vector3 offset = new Vector3(0f, 10f, 0f);
    public bool useStartOffset = true;

    [Tooltip("Should the camera look down at the target?")]
    public bool lookAtTarget = true;

    [Header("Follow Settings")]
    [Tooltip("How smoothly the camera follows the target (higher = smoother)")]
    [Range(0.1f, 10f)]
    public float followSpeed = 2f;

    [Tooltip("How smoothly the camera rotates to look at target")]
    [Range(0.1f, 10f)]
    public float rotationSpeed = 2f;

    [Header("Mouse Dynamic Follow")]
    [Tooltip("Enable dynamic camera movement based on mouse position")]
    public bool enableMouseFollow = true;

    [Tooltip("How much the camera should move towards mouse position (0 = stay on target, 1 = follow mouse completely)")]
    [Range(0f, 1f)]
    public float mouseInfluence = 0.3f;

    [Tooltip("Maximum distance the camera can move from target towards mouse")]
    public float maxMouseDistance = 5f;

    [Tooltip("How smoothly the camera moves towards mouse position")]
    [Range(0.1f, 10f)]
    public float mouseFollowSpeed = 3f;

    [Header("Orthographic Camera Settings")]
    [Tooltip("Enable dynamic camera size adjustment")]
    public bool enableDynamicSize = true;

    [Tooltip("Base orthographic size")]
    public float baseSize = 5f;

    [Tooltip("Minimum orthographic size")]
    public float minSize = 3f;

    [Tooltip("Maximum orthographic size")]
    public float maxSize = 10f;

    [Tooltip("Speed of size changes")]
    [Range(0.1f, 5f)]
    public float sizeChangeSpeed = 2f;

    [Header("Camera Boundaries (Optional)")]
    [Tooltip("Enable camera movement boundaries")]
    public bool useBoundaries = false;

    [Tooltip("Minimum X and Z coordinates the camera can move to")]
    public Vector2 minBounds = new Vector2(-50f, -50f);

    [Tooltip("Maximum X and Z coordinates the camera can move to")]
    public Vector2 maxBounds = new Vector2(50f, 50f);

    [Header("Advanced Settings")]
    [Tooltip("How close the camera needs to be before stopping movement (prevents jittering)")]
    public float stopDistance = 0.1f;

    [Tooltip("Layer mask for ground detection (should match PlayerControl)")]
    public LayerMask groundLayerMask = 1;

    private Vector3 currentVelocity;
    private Vector3 targetPosition;
    private Vector3 mouseWorldPosition;
    private Camera cam;
    private float targetSize;

    void Start()
    {
        // Get camera component
        cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("CameraControl: No Camera component found!");
            return;
        }

        // Apply starting offset if enabled
        if (useStartOffset && target != null)
        {
            offset = transform.position - target.position;
        }

        // Set up orthographic camera
        if (cam.orthographic)
        {
            targetSize = baseSize;
            cam.orthographicSize = baseSize;
        }

        // Auto-find target if not assigned
        if (target == null)
        {
            Debug.LogError("CameraControl: No target assigned! Attempting to find player by tag 'Player'.");
        }

        // Set initial position if target is found
        if (target != null)
        {
            SetInitialPosition();
        }
        else
        {
            Debug.LogWarning("CameraControl: No target found! Please assign a target or tag your player as 'Player'");
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        if (enableMouseFollow)
        {
            UpdateMouseWorldPosition();
        }

        FollowTarget();

        if (lookAtTarget)
        {
            LookAtTarget();
        }

        if (enableDynamicSize && cam != null && cam.orthographic)
        {
            UpdateCameraSize();
        }
    }

    void UpdateMouseWorldPosition()
    {
        Vector2 screenPosition = Mouse.current.position.ReadValue();
        // Convert screen position to world position using the camera
        mouseWorldPosition = cam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, cam.nearClipPlane));
        
        // Keep the z position at the target's z level (common for 2D setups)
        if (target != null)
        {
            mouseWorldPosition.z = target.position.z;
        }
        else
        {
            mouseWorldPosition.z = 0f;
        }
    }

    void FollowTarget()
    {
        // Calculate base target position
        Vector3 baseTargetPosition = target.position + offset;

        // Calculate final target position with mouse influence
        if (enableMouseFollow)
        {
            // Calculate direction from target to mouse
            Vector3 targetToMouse = mouseWorldPosition - target.position;
            targetToMouse.y = 0f; // Keep only horizontal movement

            // Limit the distance
            if (targetToMouse.magnitude > maxMouseDistance)
            {
                targetToMouse = targetToMouse.normalized * maxMouseDistance;
            }

            // Apply mouse influence
            Vector3 mouseOffset = targetToMouse * mouseInfluence;
            targetPosition = baseTargetPosition + mouseOffset;
        }
        else
        {
            targetPosition = baseTargetPosition;
        }

        // Apply boundaries if enabled
        if (useBoundaries)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            targetPosition.z = Mathf.Clamp(targetPosition.z, minBounds.y, maxBounds.y);
        }

        // Check if we're close enough to stop (prevents jittering)
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (distanceToTarget > stopDistance)
        {
            // Use different speeds for mouse follow vs regular follow
            float currentFollowSpeed = enableMouseFollow ? mouseFollowSpeed : followSpeed;

            // Smooth movement using SmoothDamp for natural feeling
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref currentVelocity,
                1f / currentFollowSpeed
            );
        }
    }

    void UpdateCameraSize()
    {
        // Calculate distance from target to mouse for dynamic sizing
        if (enableMouseFollow && target != null)
        {
            float distanceToMouse = Vector3.Distance(target.position, mouseWorldPosition);

            // Map distance to camera size (closer mouse = smaller size for more precision)
            float normalizedDistance = Mathf.Clamp01(distanceToMouse / maxMouseDistance);
            targetSize = Mathf.Lerp(minSize, maxSize, normalizedDistance);
        }
        else
        {
            targetSize = baseSize;
        }

        // Smoothly adjust camera size
        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            targetSize,
            sizeChangeSpeed * Time.fixedDeltaTime
        );
    }

    void LookAtTarget()
    {
        // Calculate direction to target
        Vector3 directionToTarget = target.position - transform.position;

        // Create rotation that looks at target
        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            // Smoothly rotate towards target
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }
    }

    void SetInitialPosition()
    {
        // Set initial position without smoothing
        Vector3 initialPosition = target.position + offset;

        if (useBoundaries)
        {
            initialPosition.x = Mathf.Clamp(initialPosition.x, minBounds.x, maxBounds.x);
            initialPosition.z = Mathf.Clamp(initialPosition.z, minBounds.y, maxBounds.y);
        }

        transform.position = initialPosition;

        // Look at target initially
        if (lookAtTarget && target != null)
        {
            transform.LookAt(target.position);
        }

        // Set initial camera size
        if (cam != null && cam.orthographic)
        {
            cam.orthographicSize = baseSize;
            targetSize = baseSize;
        }
    }


}
