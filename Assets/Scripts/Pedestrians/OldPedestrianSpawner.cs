using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class OldPedestrianSpawner : MonoBehaviour {

	//[SerializeField] int spawnerID = 0;

	public GameObject personPrefab;
	public Sprite[] personSprite;

	public float rdmSpawn; //X coordinate when spawning
	public float chanceSpawn = 0.02f; //Chance of spawn
    private float timer;

	Vector3 spawnArea;

	// Update is called once per frame
	void Update () {

        if (timer < 120)
        {
            timer += Time.deltaTime;
        }

		if (Random.value < chanceSpawn) {

			Spawn ();
		}
	}

	void OnTriggerEnter2D (Collider2D coll) {

		if (coll.gameObject.GetComponent<Pedestrian> ()) {

//			Pedestrian personScript = coll.gameObject.GetComponent<Pedestrian> ();
			//Debug.Log(personSpawn);

//			if (personScript.spawned < 1 && spawnerID > 0) { //spawned from the top (ID 0) heading for bottom (ID 1)
//
//				Destroy (coll.gameObject);
//			}
//			else if (personScript.spawned > 0 && spawnerID < 1) {
//
//				Destroy (coll.gameObject);
//			}
		}
	}

	void Spawn () {

		GameObject person = Instantiate (personPrefab) as GameObject;
		person.transform.parent = GameObject.Find("Pedestrian Container").transform;
		// Set random spawn position
		Bounds colliderBounds = this.GetComponent<BoxCollider2D> ().bounds;
		float minX = colliderBounds.min.x;
		float maxX = colliderBounds.max.x;

		Vector3 spawnPosition = new Vector3 (Random.Range (minX, maxX), this.transform.localPosition.y, this.transform.localPosition.z); //Random spawn placement on the X axis
		person.transform.position = spawnPosition;

		// Set spawn ID
		//Pedestrian personScript = person.GetComponent<Pedestrian> ();
		//personScript.spawned = spawnerID;  //sets Pedestrian spawnID based on spawner ID

		// Give random pedestrian sprite
		SpriteRenderer personSpriteRend = person.GetComponent<SpriteRenderer> (); //Getting access to the sprite renderer
		int i = Random.Range (0, personSprite.Length); //random integer i from 0 to length of sprite array
		personSpriteRend.sprite = personSprite [i]; //person's sprite renderer renders our random sprite in array

		// Give random role
		float rdmRole = Random.value; // rolls for a random number for role assignment

//		if (rdmRole >= 0.75) 
//		{
//			personScript.SetRole ("COIN");
//		}
//		else if (rdmRole >= 0.34 && rdmRole < 0.41 && timer >= 120)
//		{
//			personScript.SetRole("RAVER");
//		}
//		else if (rdmRole >= 0.27 && rdmRole <= 0.34 && timer >= 75)
//		{
//			personScript.SetRole ("CHUNKY");
//		}
//		else if (rdmRole >= 0.19 && rdmRole < 0.267 && timer >= 90) 
//		{
//			personScript.SetRole ("INSPECTOR");
//		}
//		else if (rdmRole >= 0.11 && rdmRole < 0.19 && timer >= 60)
//		{
//			personScript.SetRole ("DAZER");
//		}
//		else if (rdmRole >= 0.06 && rdmRole < 0.11 && timer >= 105)
//		{
//			personScript.SetRole("STINK");
//		}
//		else if(rdmRole < 0.06 && timer >= 45)
//		{
//			personScript.SetRole ("OFFICER");
//		}
//		else 
//		{
//			personScript.SetRole("NORM");
//		}
	}
}
