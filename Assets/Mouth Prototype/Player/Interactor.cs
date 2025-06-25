using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionPointRadius = 0.5f;
    [SerializeField] private LayerMask _interactableMask;

    private readonly Collider[] _colliders = new Collider[3];
    [SerializeField] private int _numfound;

    private IInteractable _interactable;
    private GameObject _interactable_Obj;

    private GameObject _interaction_Ui;

    private bool interact;



    private PickUpInteraction _playerPickUpInteraction;
    

    private void Start()
    {
        
        _playerPickUpInteraction = GetComponent<PickUpInteraction>();
    }


    private void Update()
    {
        interact = Input.GetMouseButton(0);
        _numfound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders, _interactableMask);
        bool isPlayerHoldingObject = _playerPickUpInteraction.IsHoldingObject();
        /*Debug.Log("NumFound: " + _numfound + " Is Holding Object: "  + isPlayerHoldingObject );*/

        if (_numfound == 0 && !isPlayerHoldingObject)
        {
            ClearInteract();
            return;
        }

        if (_interactable == null && !isPlayerHoldingObject)
        {
            SetInteractable(_colliders[0]);

        }

        if (_interactable != null)
        {
            HandleInteractable(isPlayerHoldingObject);
        }

    }

    private void SetInteractable( Collider collider)
    {
        _interactable = collider.GetComponent<IInteractable>();
        _interactable_Obj = collider.gameObject;
    }

    private void ClearInteract()
    {
        if (_interactable != null)
        {
            _interactable = null;
        }
    }

    private void HandleInteractable(bool isPlayerHoldingObject)
    {
        _interactable.Interact(this);
        Debug.Log("Interactable is not null");

        if (interact)
        {
            HandleInteractOn(isPlayerHoldingObject);
        }
        else
        {
            HandleInteractOff(isPlayerHoldingObject);
        }
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
        Debug.Log("Off");
        if (_interactable_Obj.CompareTag("Toys") && isPlayerHoldingObject)
            _playerPickUpInteraction.DropObject();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
    }

}