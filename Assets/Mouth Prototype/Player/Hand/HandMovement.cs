using System;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HandMovement : MonoBehaviour
{
    [Header("References")]
    private Camera mainCamera;
    private Rigidbody rb;
    public Collider planeCollider;

    [Header("Input & Raycasting")]
    private Vector3 mouseScreenPosition;

    private Ray mouseRay;
    public LayerMask raycastLayerMask;

    [Header("Hand Positioning")]
    [SerializeField] private Vector3 targetWorldPosition;
    [SerializeField] private float handMovementSpeed = 5f;

    [Header("Sway Behavior")]
    private Vector3 lastMousePosition;

    private Vector3 mouseDelta
    {
        get { return Input.mousePosition - lastMousePosition; }
    }


    [Range(0f, 1000f)]
    [SerializeField] public float swayScalar = 10f;

    [SerializeField]
    private float minRangeOfSway = 0f;

    [SerializeField]
    private float maxRangeOfSway = 10f;


    private Vector3 calculatedSway;
    private void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        targetWorldPosition = transform.position;
        lastMousePosition = Input.mousePosition;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

    }


    void Update()
    {
        mouseScreenPosition = Input.mousePosition;

        float sqrMagnitudePosition = lastMousePosition.sqrMagnitude;
        calculatedSway = mouseDelta;

        Debug.Log($"Last Mouse Velocity : {calculatedSway}");
        lastMousePosition = Input.mousePosition;

    }

    void FixedUpdate()
    {
        UpdateHandSway();
        UpdateHandPosition();
    }

    private void UpdateHandPosition()
    {
        mouseRay = mainCamera.ScreenPointToRay(mouseScreenPosition);
        float raycastDistance = 100f;

        if (!Physics.Raycast(mouseRay, out RaycastHit hitInfo, raycastDistance, raycastLayerMask)) return;
        if (hitInfo.collider != planeCollider) return;

        // float targetX = isSwaying ? transform.position.x + swayXOffset : hitInfo.point.x;
        // float targetY = isSwaying ? Mathf.Lerp(transform.localPosition.y, hitInfo.point.z + swayYOffset, Time.fixedDeltaTime * 2.4f) : transform.position.y;
        // float targetZ = isSwaying ? transform.position.z + swayZOffset : hitInfo.point.z;

        targetWorldPosition = new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z);
        Vector3 smoothedPosition = Vector3.Lerp(rb.position, targetWorldPosition, handMovementSpeed * Time.fixedDeltaTime);
        rb.MovePosition(smoothedPosition);
    }

    private void UpdateHandSway()
    {
        float swayPitch = Mathf.Clamp(calculatedSway.y, minRangeOfSway, maxRangeOfSway);  // Look up/down → X
        float swayYaw = Mathf.Clamp(calculatedSway.x, minRangeOfSway, maxRangeOfSway);    // Turn left/right → Y
        float swayRoll = Mathf.Clamp(calculatedSway.z, minRangeOfSway, maxRangeOfSway);   // Tilt head → Z

        Vector3 rotateTo = new Vector3(swayPitch, swayYaw, swayRoll);
        rb.MoveRotation(Quaternion.Euler(calculatedSway));

    }

    private float calculateHandSpeed()
    {
        // More heavy the object the slower the movement
        return 0f;
    }
}
