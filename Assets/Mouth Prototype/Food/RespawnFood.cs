using UnityEngine;
using UnityEngine.Events;

public class RespawnFood : MonoBehaviour
{
	[SerializeField] private Transform respawnTransform;

	[SerializeField] private LayerMask intractableLayerMask;

	[SerializeField] private MouthMovement mouthMovement;

	[SerializeField] private GameObject edible = null;

	[SerializeField] private UnityEvent onEating;

	void Awake()
	{
		edible = null;
	}

	void Start()
	{
	
	}



	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("FoodItems"))
		{
			edible = other.gameObject;
			onEating?.Invoke();

		}
	}

	public void DestroyEdible()
	{
		if (edible != null)
			Destroy(edible);


	}





}
