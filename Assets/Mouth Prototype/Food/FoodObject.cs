using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class FoodObject : MonoBehaviour
{
	private HandPickUp interactor;
	private Rigidbody rb;

	public Rigidbody Rb => rb;


	// Stats

	// Mesh

	// SFX

	// Effects when PickedUp

	// Effects when Eating

	// Reference to breakable mesh

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		rb.mass = Random.Range(1.0f, 4.0f);
	}

	public void Interact(HandPickUp interactor)
	{
		this.interactor = interactor;
	}

}
