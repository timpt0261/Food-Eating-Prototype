using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class HandMovement : MonoBehaviour
{
    [Header("References")]
    private Camera mainCamera;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider planeCollider;

    [SerializeField] private HandPickUp handPickUp;
    [SerializeField] private PickUpInteraction pickUpInteraction;

    [Header("Input & Raycasting")]
    private Vector3 mouseScreenPosition;
    private Ray mouseRay;
    public LayerMask raycastLayerMask;

    [Header("Hand Positioning")]
    [SerializeField] private Vector3 targetWorldPosition;
    [SerializeField] private float baseHandSpeed = 5f;

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


    [Header("Stamina")]

    [SerializeField] private float stamina = 1.0f;

    public float Stamina => stamina;

    [SerializeField] private float strength = 1f;

    [SerializeField] private Slider slider;

    private bool IsPickingUp = false;
    private Vector3 initialPosition;
    
    [SerializeField] private AnimationCurve pickUpCurve;

    [SerializeField] private float maxPickUpHeight = 8f;

    [SerializeField] private float minPickUpHeight;

    [SerializeField] private float pickUpSpeed = 2.5f;

    [SerializeField] private float dropOffSpeed = 5f;

    
    [Header("UI")]
    [SerializeField] private Image _staminaBar;
    



    private float minHandSpeed = 1f;
    private float maxHandspeed = 15f;
    private Vector3 calculatedSway;

    bool interactOnFrame = false;
    float mass;
    Vector3 direction;





    private void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        minPickUpHeight = rb.position.y;

        handPickUp = GetComponent<HandPickUp>();
        pickUpInteraction = GetComponent<PickUpInteraction>();

        targetWorldPosition = transform.position;
        lastMousePosition = Input.mousePosition;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

    }


    void Update()
    {
        mouseScreenPosition = Input.mousePosition;

        interactOnFrame = !handPickUp.Interact;
        calculatedSway = mouseDelta;
        IsPickingUp = Input.GetMouseButton(0) && Input.GetMouseButton(1) && pickUpInteraction.HoldingObject;
        lastMousePosition = Input.mousePosition;

    }

    void FixedUpdate()
    {
        UpdateStamina();
        UpdateHandSway();
        MoveHandUp();
        UpdateHandPosition();
        MoveHandDown();
    }

    private void MoveHandUp()
    {
        if (!IsPickingUp) return;
        UpdateHandHeight(true);
    }


    private void MoveHandDown()
    {
        if (IsPickingUp) return;
        UpdateHandHeight(false);
    }

    private void UpdateHandHeight(bool direction)
    {
        float heightStep = 2.5f;
        float speed = direction ? pickUpSpeed : dropOffSpeed;
        float targetHeight = direction ? rb.position.y + heightStep : rb.position.y - heightStep;
        float evY = pickUpCurve.Evaluate(targetHeight);
        float clampedY = Mathf.Clamp(targetHeight, minPickUpHeight, maxPickUpHeight);

        Vector3 targetPosition = new Vector3(rb.position.x, clampedY, rb.position.z);
        Vector3 smoothedPosition = Vector3.Lerp(rb.position, targetPosition, Time.fixedDeltaTime * speed);
        rb.MovePosition(smoothedPosition);


    }

    private void UpdateHandPosition()
    {
        mouseRay = mainCamera.ScreenPointToRay(mouseScreenPosition);
        float raycastDistance = 100f;

        if (!Physics.Raycast(mouseRay, out RaycastHit hitInfo, raycastDistance, raycastLayerMask)) return;
        if (hitInfo.collider != planeCollider) return;

        float speed = CalculateSpeed();
        targetWorldPosition = new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z);
        Vector3 smoothedPosition = Vector3.Lerp(rb.position, targetWorldPosition, speed * Time.fixedDeltaTime);
        rb.MovePosition(smoothedPosition);
    }

    private void UpdateHandSway()
    {
        Quaternion calculatedRotation = Quaternion.Euler(calculatedSway);

        // Quaternion calculatedRotation = Quaternion.Euler(calculatedSway);
        rb.MoveRotation(calculatedRotation);

    }

    private float CalculateSpeed()
    {
        Rigidbody holdingObj_RB = pickUpInteraction.RB_HeldObject;
        if (holdingObj_RB == null) { return baseHandSpeed; }

        int roundToDecimal = 3;
        int offset = 1;
        float speed = baseHandSpeed * stamina;
        float roundedSpeed = round(speed, roundToDecimal);
        float weightOfObject = holdingObj_RB.mass / strength;

        // Debug.Log($" Rounded Speed: {roundedSpeed}");
        // Debug.Log($" Weight of Object: {weightOfObject}");

        // Debug.Log($"Speed : {roundedSpeed / weightOfObject}");
        return roundedSpeed / weightOfObject;
    }

    private void UpdateStamina()
    {

        float rate = .1f;
        float change = 0;
        if (IsPickingUp)
        {
            float mass = pickUpInteraction.RB_HeldObject.mass;
            change = mass * rate * Time.fixedDeltaTime;
            stamina -= change;

        }
        else
        {
            // change = rate * Time.deltaTime;
            // stamina += change;

            stamina = Mathf.Lerp(stamina, stamina + rate, Time.fixedDeltaTime);
        }

        stamina = Mathf.Clamp01(stamina);
        // sliderChangeColor();
        slider.value = stamina;


    }

    public void sliderChangeColor()
    {
        Color newColor = _staminaBar.color;
        newColor.a = Mathf.Clamp01(slider.value);
        _staminaBar.color = newColor;
    }

    private float round(float a, int b)
    {
        return Mathf.Round(a * Mathf.Pow(10, b) / Mathf.Pow(10, b));
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, direction);
    }
}
