using UnityEngine;
using System.Collections;

public class CarSpawner : MonoBehaviour {

	// Array for placing car prefabs;
	public GameObject[] carArray;
	//  Spawn location
	public Transform carSpawnPoint = null;
	private GameData gameData;

	public float minSpawnTime;
	public float maxSpawnTime;
	private float timer;
	private float randomSpawn;
	private float randomCar;

	public int layerOrderShift = 0;
	public bool timerActive = false;

	void Start()
	{
		gameData = GameObject.Find ("GameManager").GetComponent<GameData>();
	}
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

			if (randomSpawn <= 0.6999)
			{	
				randomCar = Random.Range (0, 4);
				Debug.Log (randomCar);

				car = Instantiate (carArray [0], carSpawnPoint.position, carSpawnPoint.rotation) as GameObject;
				car.transform.parent = GameObject.Find ("Car Container").transform;
				timerActive = false;
			} 
				
			else if (randomSpawn >= 0.7 && randomSpawn <= 0.89999)
			{
				car = Instantiate (carArray [4], carSpawnPoint.position, carSpawnPoint.rotation) as GameObject;
				car.transform.parent = GameObject.Find ("Car Container").transform;
				timerActive = false;
			}
			else if (randomSpawn >= 0.9 && randomSpawn <= 1.0) 
			{
				car = Instantiate (carArray [5], carSpawnPoint.position, carSpawnPoint.rotation) as GameObject;
				car.transform.parent = GameObject.Find ("Car Container").transform;
				timerActive = false;
			}
			else 
			{
				
				car = Instantiate (carArray [0], carSpawnPoint.position, carSpawnPoint.rotation) as GameObject;
				car.transform.parent = GameObject.Find ("Car Container").transform;
				timerActive = false;

			}

			// Shift sprite layer order for cars that go behind or in front of the streetcar
			if(layerOrderShift != 0) {

				SpriteRenderer[] spriteRenderers = car.GetComponentsInChildren<SpriteRenderer>();
				foreach (SpriteRenderer spriteRenderer in spriteRenderers) {

					int newSortingOrder = spriteRenderer.sortingOrder + layerOrderShift;
					spriteRenderer.sortingOrder = newSortingOrder;
				}
			}
		}
	}
}
