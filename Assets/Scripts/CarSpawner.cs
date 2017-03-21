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

	public bool timerActive = false;

	void Start()
	{
		gameData = GameObject.Find ("GameManager").GetComponent<GameData>();
	}
	void Update () 
	{
		if (gameData.is_Game_Started == true)
		{
			spawnCar ();
		}
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


			if (randomSpawn <= 0.6999)
			{	
				randomCar = Random.Range (0, 4);
				Debug.Log (randomCar);

				GameObject car = Instantiate (carArray [0], carSpawnPoint.position, carSpawnPoint.rotation) as GameObject;
				car.transform.parent = GameObject.Find ("Car Container").transform;
				timerActive = false;
			} 
				
			else if (randomSpawn >= 0.7 && randomSpawn <= 0.89999)
			{
				GameObject car = Instantiate (carArray [4], carSpawnPoint.position, carSpawnPoint.rotation) as GameObject;
				car.transform.parent = GameObject.Find ("Car Container").transform;
				timerActive = false;
			}
			else if (randomSpawn >= 0.9 && randomSpawn <= 1.0) 
			{
				GameObject car = Instantiate (carArray [5], carSpawnPoint.position, carSpawnPoint.rotation) as GameObject;
				car.transform.parent = GameObject.Find ("Car Container").transform;
				timerActive = false;
			}
			else 
			{
				
				GameObject car = Instantiate (carArray [0], carSpawnPoint.position, carSpawnPoint.rotation) as GameObject;
				car.transform.parent = GameObject.Find ("Car Container").transform;
				timerActive = false;

			}

		}
		
	}

}
