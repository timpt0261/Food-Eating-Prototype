using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
	public float spawnInterval = 5f;
	public int maxSpawnedItems = 10;
	public GameObject[] foodPrefabs;

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

	void SpawnFood()
	{
		if (foodPrefabs == null || foodPrefabs.Length == 0)
		{
			Debug.LogWarning("No food prefabs assigned!");
			return;
		}

		int randomIndex = Random.Range(0, foodPrefabs.Length);
		Vector3 spawnPosition = transform.position + transform.up;
		Instantiate(foodPrefabs[randomIndex], spawnPosition, Quaternion.identity, transform);
		currentSpawnCount++;
	}
}
