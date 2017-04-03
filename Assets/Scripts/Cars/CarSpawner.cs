using UnityEngine;
using System.Collections;

public class CarSpawner : MonoBehaviour {

	// Array for placing car prefabs;
	public GameObject[] carArray;
	public Color[] carColorOptions;

	//  Spawn location
	public Transform carSpawnPoint = null;

	public float minSpawnTime;
	public float maxSpawnTime;
	public string layerName;
	public int layerOrderShift = 0;
	public bool timerActive = false;

	[SerializeField] Transform carContainerTransform;

	private float timer;
	private float randomSpawn;

	void Update () 
	{
		spawnCar ();
	}

	void spawnCar()
	{	
		if(timerActive == false)
		{	
			// Random time generator between set values.
			timer = Random.Range(minSpawnTime,maxSpawnTime);
			timerActive = true;

			//Debug.Log(timer);
		}

		else if(timerActive == true)
		{
			timer -= Time.deltaTime;
		}

		if(timer < 0)
		{	
			randomSpawn = Random.value;

			GameObject car = null;

			if (randomSpawn <= 0.17f)
			{	
				car = Instantiate (carArray [0], carSpawnPoint.position, carSpawnPoint.rotation) as GameObject;
				//Debug.Log ("Car1");
				// Color car randomly
				SpriteRenderer spriteRenderer = car.transform.GetChild(2).GetComponent<SpriteRenderer>();
				spriteRenderer.color = carColorOptions[Random.Range(0, carColorOptions.Length)];
			} 
			else if (randomSpawn > 0.17f && randomSpawn <= 0.34f )
			{	
				car = Instantiate (carArray [1], carSpawnPoint.position, carSpawnPoint.rotation) as GameObject;
				//Debug.Log ("Car2");
				// Color car randomly
				SpriteRenderer spriteRenderer = car.transform.GetChild(2).GetComponent<SpriteRenderer>();
				spriteRenderer.color = carColorOptions[Random.Range(0, carColorOptions.Length)];
			} 
			else if (randomSpawn > 0.34f && randomSpawn <= 0.51f)
			{	
				car = Instantiate (carArray [2], carSpawnPoint.position, carSpawnPoint.rotation) as GameObject;
				//Debug.Log ("Car3");
				// Color car randomly
				SpriteRenderer spriteRenderer = car.transform.GetChild(2).GetComponent<SpriteRenderer>();
				spriteRenderer.color = carColorOptions[Random.Range(0, carColorOptions.Length)];
			} 
			else if (randomSpawn > 0.51f && randomSpawn <= 0.7f)
			{	
				car = Instantiate (carArray [3], carSpawnPoint.position, carSpawnPoint.rotation) as GameObject;
				//Debug.Log ("Car4");
				// Color car randomly
				SpriteRenderer spriteRenderer = car.transform.GetChild(2).GetComponent<SpriteRenderer>();
				spriteRenderer.color = carColorOptions[Random.Range(0, carColorOptions.Length)];
			} 

			else if (randomSpawn > 0.7f && randomSpawn <= 0.9f) // Taxi
			{	
				//Debug.Log ("Taxi");
				car = Instantiate (carArray [4], carSpawnPoint.position, carSpawnPoint.rotation) as GameObject;
				timerActive = false;
			}
			else // Police
			{	
				//Debug.Log ("Cop");
				car = Instantiate (carArray [5], carSpawnPoint.position, carSpawnPoint.rotation) as GameObject;
			}

			timerActive = false;
			car.transform.SetParent(carContainerTransform);

			// Shift sprite layer order for cars that go behind or in front of the streetcar
			if(layerOrderShift != 0) {

				SpriteRenderer[] spriteRenderers = car.GetComponentsInChildren<SpriteRenderer>();
				foreach (SpriteRenderer spriteRenderer in spriteRenderers) {

					spriteRenderer.sortingLayerName = layerName;
					int newSortingOrder = spriteRenderer.sortingOrder + layerOrderShift;
					spriteRenderer.sortingOrder = newSortingOrder;
				}
			}
		}
	}
}
