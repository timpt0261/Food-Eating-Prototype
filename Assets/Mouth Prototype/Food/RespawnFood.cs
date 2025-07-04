using UnityEngine;

public class RespawnFood : MonoBehaviour
{
	public Transform respawnTransform;

	public LayerMask intractableLayerMask;

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("FoodItems"))
		{
			other.gameObject.transform.position = respawnTransform.position;
		}
	}

}
