using UnityEngine;

public class HandPickUp : MonoBehaviour
{

	[Header("References")]
	private PickUpInteraction _playerPickUpInteraction;
	private bool isPlayerHoldingObject;

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


	private void Awake()
	{
		_playerPickUpInteraction = GetComponent<PickUpInteraction>();
	}


	private void Update()
	{
		interact = Input.GetMouseButton(0);
		_numfound = Physics.OverlapBoxNonAlloc(_interactionPoint.position, _interactionPointSize, _colliders, Quaternion.identity, _interactableMask);
		isPlayerHoldingObject = _playerPickUpInteraction.IsHoldingObject();

		Interaction();
	}

	private void Interaction()
	{
		if (_numfound == 0 && !isPlayerHoldingObject)
		{
			ClearInteract();
			return;
		}

		if (_interactable == null && !isPlayerHoldingObject)
		{
			SetInteractable(_colliders[0]);
			return;
		}

		if (_interactable != null)
		{
			HandleInteractable(isPlayerHoldingObject);
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
		if (interact) { HandleInteractOn(isPlayerHoldingObject); return; }
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
			_playerPickUpInteraction.DropObject();
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