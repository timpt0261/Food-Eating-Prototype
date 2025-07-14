using UnityEngine;
using System.Threading.Tasks;
using System.Collections;
using Unity.VisualScripting;

[RequireComponent(typeof(Rigidbody))]
public class FoodObject : MonoBehaviour
{
	private HandPickUp interactor;
	private Rigidbody rb;

	public Rigidbody Rb => rb;

	[SerializeField]
	private AudioSource eatSFX = null;

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
