using UnityEngine;


public class PickUpInteraction : MonoBehaviour
{
	
	[SerializeField] Transform holdArea = null;
    [SerializeField] float holdAreaRadius = 5.0f;
    [SerializeField] private GameObject heldObject = null;
    private Rigidbody heldObjectRigidbody = null;
    [SerializeField] private float pickupForce = 150.0f;

    [Range(0.01f, 1f)]
    [SerializeField] private float deltaDistance = 0.1f;

    public bool IsHoldingObject()
    {
        return heldObject != null;
    }

    public void MoveObject()
	{
		// Debug.Log("Moving object");
		if (heldObjectRigidbody == null)
			return;

		if (Vector3.Distance(heldObject.transform.position, holdArea.position) > deltaDistance)
        {
            Vector3 moveDirection = holdArea.position - heldObject.transform.position;
            heldObjectRigidbody.AddForce(moveDirection * pickupForce);
        }

	}


	public void PickUpObject(GameObject pickObj)
    {
        Debug.Log("Picking Up object");
        Rigidbody pickObjRB = pickObj.GetComponent<Rigidbody>();
        CharacterController pickObjController = pickObj.GetComponent<CharacterController>();

        if (!pickObjRB) return;

        heldObjectRigidbody = pickObjRB;
        heldObjectRigidbody.useGravity = false;
        heldObjectRigidbody.linearDamping = 10;
        heldObjectRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        heldObjectRigidbody.transform.parent = holdArea;
        heldObject = pickObj;

    }

    public void DropObject()
    {
        Debug.Log("Droping Object");
        if (heldObjectRigidbody == null) return;

        heldObjectRigidbody.useGravity = true;
        heldObjectRigidbody.linearDamping = 1;
        heldObjectRigidbody.constraints = RigidbodyConstraints.None;
        heldObjectRigidbody.transform.parent = null;
        heldObject = null;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(holdArea.position, holdAreaRadius);
    }
}
