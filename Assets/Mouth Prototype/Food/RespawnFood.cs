using System;
using UnityEngine;
using UnityEngine.Events;

public class RespawnFood : MonoBehaviour
{
	public Transform respawnTransform;

	public LayerMask intractableLayerMask;

	public UnityEvent onEating;

	public AudioSource eatingSFX;



	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("FoodItems"))
		{
			onEating?.Invoke();
		}
	}

	public void OnEating()
	{
		// is mouth open
		
		
	}





}
