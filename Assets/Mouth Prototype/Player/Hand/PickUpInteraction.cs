using UnityEngine;
public class PickUpInteraction : MonoBehaviour
{

	[SerializeField]
	private HandMovement handMovement;

	[Header("Hold Object")]
	[SerializeField] UnityEngine.Transform holdArea = null;
	[SerializeField] float holdAreaRadius = 5.0f;
	[SerializeField] private GameObject go_heldObject = null;
	private Rigidbody rb_heldObject = null;
	private FoodObject _foodObject = null;

	public Rigidbody RB_HeldObject => rb_heldObject;

	[Range(0.01f, 1f)]
	[SerializeField] private float range = 0.1f;


	[Range(0, 1000)]
	[SerializeField] private int pickupForce = 150;

	public bool HoldingObject
	{
		get
		{
			return go_heldObject != null;
		}
	}

	void Awake()
	{
		handMovement = GetComponent<HandMovement>();
	}

	public bool IsHoldingObject()
	{
		return go_heldObject != null;
	}

	public void MoveObject()
	{
		// Debug.Log("Moving object");
		if (rb_heldObject == null)
			return;

		if (Vector3.Distance(go_heldObject.transform.position, holdArea.position) > range)
		{
			Vector3 moveDirection = holdArea.position - go_heldObject.transform.position;
			rb_heldObject.AddForce(moveDirection * pickupForce);
			// Vector3 targetPostion = Vector3.Lerp(holdArea.position, go_heldObject.transform.position, Time.deltaTime * 10f);
			// rb_heldObject.MovePosition(targetPostion);
		}

	}


	public void PickUpObject(GameObject pickObj, HandPickUp interactor)
	{
		// Debug.Log("Picking Up object");
		pickObj.TryGetComponent<Rigidbody>(out Rigidbody pickObjRB);
		pickObj.TryGetComponent<FoodObject>(out FoodObject pickUpFoodObject);

		if (!pickObjRB) return;
		rb_heldObject = pickObjRB;
		
		if(!pickUpFoodObject) return;
		_foodObject = pickUpFoodObject;
		_foodObject.Interactor = interactor;
		updateHeldObjectRigidBody(pickObj, true);
		


	}

	public void DropObject()
	{
		// Debug.Log("Droping Object");
		if (rb_heldObject == null || _foodObject == null) return;
		
		_foodObject.Interactor = null;
		updateHeldObjectRigidBody(null, false);
		
	}

	private void updateHeldObjectRigidBody(GameObject pickObj, bool handle)
	{
		// // update food state
		// FOOD_OBJ_STATE foodState = handle ? FOOD_OBJ_STATE.GRABBED : FOOD_OBJ_STATE.DROPPED;
		// FoodObject foodObject = pickObj.GetComponent<FoodObject>();
		// foodObject.UpdateFoodObjectState(foodState);
		
		// update rigidbody
		rb_heldObject.useGravity = !handle;
		rb_heldObject.linearDamping = handle ? 10 : 1;
		rb_heldObject.constraints = handle ? RigidbodyConstraints.FreezeRotation : RigidbodyConstraints.None;
		rb_heldObject.transform.parent = handle ? holdArea : null;
		
		
		
		
		
		go_heldObject = handle ? pickObj : null;

	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(holdArea.position, holdAreaRadius);
	}
}
