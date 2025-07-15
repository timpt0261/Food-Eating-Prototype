using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class FoodObject : MonoBehaviour
{
	private HandPickUp interactor;
	private Rigidbody rb;

	public Rigidbody Rb => rb;

	public GameObject fracturedRef;

	// Stats

	// Mesh

	// SFX

	// Effects when PickedUp

	// Effects when Eating

	// Reference to breakable mesh

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	public void Interact(HandPickUp interactor)
	{
		this.interactor = interactor;
	}

}
