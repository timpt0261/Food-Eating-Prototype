using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RespawnFood : MonoBehaviour
{
	public Transform respawnTransform;

	public LayerMask intractableLayerMask;

	public AudioSource eatingSFX;

	public MouthMovement mouthMovement;

	public GameObject edible = null;

	public GameObject fracturedEdible = null;

	public UnityEvent onEating;


	[Serializable]
	public class FoodPrefabEntry
	{
		public string foodName;
		public GameObject prefab;
	}

	// Inspector-friendly list of entries
	[SerializeField]
	private FoodPrefabEntry[] foodPrefabEntries;

	// Runtime dictionary to quickly look up prefabs
	private Dictionary<string, GameObject> foodPrefabDict = new Dictionary<string, GameObject>();

	void Awake()
	{
		edible = null;
	}

	void Start()
	{
		// Build dictionary at runtime
		foreach (var entry in foodPrefabEntries)
		{
			if (!foodPrefabDict.ContainsKey(entry.foodName) && entry.prefab != null)
				foodPrefabDict.Add(entry.foodName, entry.prefab);
		}
	}



	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("FoodItems") && mouthMovement.CurrentPitch > 10)
		{
			edible = other.gameObject;
			onEating?.Invoke();

		}
	}

	public void OnEating()
	{
		if (edible != null)
			Destroy(edible);


	}





}
