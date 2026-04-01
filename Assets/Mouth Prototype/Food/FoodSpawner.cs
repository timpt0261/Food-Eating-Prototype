using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class FoodSpawner : MonoBehaviour
{

	[SerializeField] private Transform spawnPoint;

	[SerializeField] private Vector3 desiredScale;
	[SerializeField] private float spawnInterval = 5f;
	[SerializeField] private int maxSpawnedItems = 10;
	[SerializeField] private GameObject[] foodPrefabs;

	private float lastSpawnTime = 0f;
	private int currentSpawnCount = 0;

	void Start()
	{
		lastSpawnTime = Time.time;
	}

	void Update()
	{
		// Only spawn if the interval has passed and we haven't reached the limit
		if (Time.time - lastSpawnTime >= spawnInterval && currentSpawnCount < maxSpawnedItems)
		{
			SpawnFood();
			lastSpawnTime = Time.time;
		}
	}

	public void SpawnFood()
	{
		if (foodPrefabs == null || foodPrefabs.Length == 0)
		{
			Debug.LogWarning("No food prefabs assigned!");
			return;
		}

		int randomIndex = Random.Range(0, foodPrefabs.Length);
		Vector3 spawnPosition = transform.position + transform.up;
		GameObject spawned = Instantiate(foodPrefabs[randomIndex], spawnPosition, Quaternion.identity, transform);
		spawned.transform.localScale = desiredScale;
		currentSpawnCount++;
	}


	void OnDrawGizmos()
	{
		float radius = .5f;
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(spawnPoint.position, radius);
	}
}
