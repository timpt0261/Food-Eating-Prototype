using System;
using Unity.VisualScripting;
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
    private Vector3 targetWorldPosition;
    public float handMovementSpeed = 5f;

    [Header("Sway Behavior")]
    public bool isSwaying = false;
    public float swayXOffset = 2f;
    public float swayYOffset = 2f;
    public float swayZOffset = 2f;

    private void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        targetWorldPosition = transform.position;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        isSwaying = false;
    }


    void Update()
    {
        mouseScreenPosition = Input.mousePosition;    
    }

    void FixedUpdate()
    {
        UpdateHandPositionWithSway();
    }

    private void UpdateHandPositionWithSway()
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
}
