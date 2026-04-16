using UnityEngine;
using System;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class FoodObject : MonoBehaviour
{
	
	[Header("Internal Components")]
	private HandPickUp interactor;
	private Rigidbody rb;
	[field: SerializeField] 
	private FoodStats stats;
	// Stats

	// Mesh

	// SFX

	// Effects when PickedUp

	// Effects when Eating


	public static event Action OnGrab;
	public static event Action OnEaten;
	public static event Action OnDropped;

	void Awake()
	{
		
		// if stats has more than one effect -> for each effect in stats enable
		// if(stats.effects.Count <= 0)
		// 	return;
		// foreach (FoodEffect effect in stats.effects)
		// {
		// 	switch (effect.activation)
		// 	{
		// 		case  FoodEffectActivation.ONGRAB:
		// 			// subscribe
		// 			break;
		// 		case FoodEffectActivation.ONEATEN :
		// 			break;
		// 		case FoodEffectActivation.ONDROPPED:
		// 			break;
		// 		default:
		// 			break;
		// 	}
		// }
	}

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	public void Interact(HandPickUp interactor)
	{
		this.interactor = interactor;
	}

}
