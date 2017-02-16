using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public GameObject personPrefab; //Prefab for instantiating people
	Vector3 spawnArea;

	public float ySpawn = 5.5f;

	public float rdmSpawn; //X coordinate when spawning
	public float chanceSpawn = 0.02f; //Chance of spawn

	// Use this for initialization
	void Start () {
		
	}

	void Spawn (Vector3 xSpawn) {
		GameObject person = Instantiate (personPrefab) as GameObject;
		person.transform.position = xSpawn; //person spawns on xSpawn input
	}
	
	// Update is called once per frame
	void Update () {
		//Spawn either bottom or top
		if (Random.value < 0.5) {

			ySpawn = -ySpawn; //switches spawn area from top/down

		}

		//Random Spawn
		if (Random.value < chanceSpawn) {
			spawnArea = new Vector3 (Random.Range(-6f, 6f), ySpawn); //Random spawn placement
			Spawn(spawnArea);
		}
	
		// random X coordinate for spawning per tick


	}
}
