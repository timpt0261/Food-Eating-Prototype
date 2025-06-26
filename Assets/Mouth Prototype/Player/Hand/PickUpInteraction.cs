using UnityEngine;
public class PickUpInteraction : MonoBehaviour
{

    [SerializeField]
    private HandMovement handMovement;

    [Header("Hold Object")]
    [SerializeField] Transform holdArea = null;
    [SerializeField] float holdAreaRadius = 5.0f;
    [SerializeField] private GameObject heldObject = null;
    private Rigidbody heldObjectRigidbody = null;

    [Range(0.01f, 1f)]
    [SerializeField] private float range = 0.1f;


    [Range(0, 1000)]
    [SerializeField] private int pickupForce = 150;

    void Awake()
    {
        handMovement = GetComponent<HandMovement>();
    }

    public bool IsHoldingObject()
    {
        return heldObject != null;
    }

    public void MoveObject()
    {
        // Debug.Log("Moving object");
        if (heldObjectRigidbody == null)
            return;

        if (Vector3.Distance(heldObject.transform.position, holdArea.position) > range)
        {
            Vector3 moveDirection = holdArea.position - heldObject.transform.position;
            heldObjectRigidbody.AddForce(moveDirection * pickupForce);
            // Vector3 targetPostion = Vector3.Lerp(holdArea.position, heldObject.transform.position, Time.deltaTime * pickupForce);
            // heldObjectRigidbody.MovePosition(targetPostion);
        }

    }


    public void PickUpObject(GameObject pickObj)
    {
        Debug.Log("Picking Up object");
        pickObj.TryGetComponent<Rigidbody>(out Rigidbody pickObjRB);

        if (!pickObjRB) return;
        heldObjectRigidbody = pickObjRB;
        updateHeldObjectRigidBody(pickObj, true);

    }

    public void DropObject()
    {
        Debug.Log("Droping Object");
        if (heldObjectRigidbody == null) return;
        updateHeldObjectRigidBody(null, false);
    }

    private void updateHeldObjectRigidBody(GameObject pickObj, bool handle)
    {

        heldObjectRigidbody.useGravity = !handle;
        heldObjectRigidbody.linearDamping = handle ? 10 : 1;
        heldObjectRigidbody.constraints = handle ? RigidbodyConstraints.FreezeRotation : RigidbodyConstraints.None;
        heldObjectRigidbody.transform.parent = handle ? holdArea : null;
        heldObject = handle ? pickObj : null;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(holdArea.position, holdAreaRadius);
    }
}
