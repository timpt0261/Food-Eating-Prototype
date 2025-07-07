using System.Collections;
using UnityEngine;

public class HandPickUp : MonoBehaviour
{

	[Header("References")]

	[SerializeField] private HandMovement _handMovement;
	[SerializeField] private PickUpInteraction _playerPickUpInteraction;
	private bool canPlayerHoldObject;

	[Header("Collider Handling")]
	[SerializeField] private Transform _interactionPoint;
	[SerializeField] private Vector3 _interactionPointSize;
	[SerializeField] private LayerMask _interactableMask;

	private readonly Collider[] _colliders = new Collider[3];
	private int _numfound;

	[Header("Input Handling")]
	[SerializeField] private bool interact;

	public bool Interact => interact;

	private FoodObject _interactable;
	private GameObject _interactable_Obj;

	private Rigidbody _interactable_rb;

	private bool isCoolDownActive = false;

	private void Start()
	{
		_handMovement = GetComponent<HandMovement>();
		_playerPickUpInteraction = GetComponent<PickUpInteraction>();
	}


	private void Update()
	{
		interact = Input.GetMouseButton(0) && !isCoolDownActive && _handMovement.Stamina > 0;
		_numfound = Physics.OverlapBoxNonAlloc(_interactionPoint.position, _interactionPointSize, _colliders, Quaternion.identity, _interactableMask);
		canPlayerHoldObject = _playerPickUpInteraction.IsHoldingObject();
		// Debug.Log($"Can Hold {canPlayerHoldObject}");
		Interaction();
	}

	private void Interaction()
	{
		if (_numfound == 0 && !canPlayerHoldObject)
		{
			Debug.Log("Clear");
			ClearInteract();
			return;
		}

		if (_interactable == null && !canPlayerHoldObject)
		{
			Debug.Log("Set");
			SetInteractable(_colliders[0]);
			return;
		}

		if (_interactable != null)
		{
			Debug.Log("Handle");
			HandleInteractable(canPlayerHoldObject);
			return;
		}

	}

	private void ClearInteract()
	{
		if (_interactable != null)
		{
			_interactable = null;
			_interactable_Obj = null;
		}
	}

	private void SetInteractable(Collider collider)
	{
		_interactable = collider.GetComponent<FoodObject>();
		_interactable_Obj = collider.gameObject;
	}


	private void HandleInteractable(bool isPlayerHoldingObject)
	{
		_interactable.Interact(this);
		// is able to interact and have enough stamina
		if (interact && _handMovement.Stamina > 0) { HandleInteractOn(isPlayerHoldingObject); return; }
		HandleInteractOff(isPlayerHoldingObject);

	}

	private void HandleInteractOn(bool isPlayerHoldingObject)
	{
		if (!isPlayerHoldingObject)
			_playerPickUpInteraction.PickUpObject(_interactable_Obj);
		else
			_playerPickUpInteraction.MoveObject();
	}

	private void HandleInteractOff(bool isPlayerHoldingObject)
	{
		if (isPlayerHoldingObject)
		{
			_playerPickUpInteraction.DropObject();
			StartCoroutine(CoolDownPeriod());
		}

	}

	private IEnumerator CoolDownPeriod()
	{
		isCoolDownActive = true;
		interact = false;
		yield return new WaitForSeconds(5f);
		isCoolDownActive = false;
	}

	private void OnDrawGizmosSelected()
	{
		if (_interactionPoint == null) return;

		Gizmos.color = Color.yellow;

		// Draw a wire cube representing the OverlapBox
		Gizmos.matrix = Matrix4x4.TRS(_interactionPoint.position, Quaternion.identity, Vector3.one);
		Gizmos.DrawWireCube(Vector3.zero, _interactionPointSize * 2); // Multiply by 2 because size is half-extents in OverlapBox
	}


}