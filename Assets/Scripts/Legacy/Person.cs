using UnityEngine;
using System.Collections;

public class Person : MonoBehaviour {

	public static float bottomY = -6f;
	public float speed;

	//Trying to figure out pathing solution
	private bool streetcarAvoid;

	// Use this for initialization
	void Start () {

		//referencing Spawner Class
		GameObject spawnerGO = GameObject.Find("Spawner");
		Spawner spawnerScript = spawnerGO.GetComponent<Spawner> ();


		//when spawned, set random speed
		speed = Random.Range (0.1f, 0.5f);

		//determining direction from Spawner Class with speed
		if (spawnerScript.ySpawn < 0) { //Spawning Bottom

			speed = -speed;

		}

		//when spawned, set tag and colour
		if (Random.value >= 0.75)
		{
			this.tag = "Blue";
			this.GetComponent<SpriteRenderer> ().color = Color.blue;

		}
	}

	// Update is called once per frame
	void Update () {

		Vector3 pos = transform.position;

		//Vector3 dwn = transform.TransformDirection (Vector3.down); //getting variable for downward direction
		//if (Physics.Raycast (pos, dwn, 8)) { // Attempt at Raycast collision
		//	Debug.Log ("Raycast Collision!");
		//}

		//Vector3 dwn = new Vector2 (pos.x, 3);
		//RaycastHit2D hit = Physics2D.Raycast (pos, dwn, 0.25f);

		//if (hit.collider.tag != null) {

			//Debug.DrawLine (pos, dwn, Color.white, 0.25f); //drawing line simulating Raycast
		//	Debug.DrawLine (pos, dwn, Color.white, 0.25f);
		//	Debug.Log ("Raycast Collision!");

		//}


			

		if (streetcarAvoid == true) {

			//Debug.Log ("SHOULD BE AVOIDING");

			//pos.y += speed / 8; //going against prior y movement
			pos.x += speed / 8; //moving over until no longer colliding with streetcar
			transform.position = pos;

		} else {
			
			//basic movement based on randomized speed
			pos.y -= speed/8;
			transform.position = pos;
		}
	

		//if hits bottom of screen
		if (transform.position.y < bottomY) {
			Destroy (this.gameObject);
		}

	}



	void OnCollisionEnter2D (Collision2D col) {
		GameObject collidedWith = col.gameObject;
		Debug.Log ("Hitting " + collidedWith);
		if (collidedWith.tag == "Player") {
			streetcarAvoid = true;
			//Debug.Log ("streetcarAvoid = " + streetcarAvoid);
		}

	}

	void OnCollisionExit2D (Collision2D col) {

		//Debug.Log ("LEAVING COLLISION");
		streetcarAvoid = false;

	}
		
}
