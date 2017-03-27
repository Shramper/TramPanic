using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public float maxSpeed = 10;
	public float speed = 50f;

	private Rigidbody2D playerRB2D;

	public GUIText scoreGT;

	// Use this for initialization
	void Start () {

		playerRB2D = gameObject.GetComponent<Rigidbody2D> ();

		GameObject scoreGO = GameObject.Find ("ScoreCounter");
		scoreGT = scoreGO.GetComponent<GUIText> ();
		scoreGT.text = "0";

	}

	// Update is called once per frame
	void Update () {

	}


	void FixedUpdate()
	{

		Vector3 easeVelocity = playerRB2D.velocity;
		easeVelocity.y = playerRB2D.velocity.y;
		easeVelocity.z = 0.0f;
		easeVelocity.x *= 0.5f;

		float h = Input.GetAxis ("Horizontal");

		//Fake friction / Easing the x speed of the player
		playerRB2D.velocity = easeVelocity;

		//Moving the Player
		playerRB2D.AddForce((Vector2.right * speed) * h);

		//Limiting the speed of the player
		if (playerRB2D.velocity.x > maxSpeed) 
		{
			playerRB2D.velocity = new Vector2 (maxSpeed, playerRB2D.velocity.y);
		}

		if (playerRB2D.velocity.x < -maxSpeed) 
		{
			playerRB2D.velocity = new Vector2 (-maxSpeed, playerRB2D.velocity.y);
		}

	}

	//void OnCollisionEnter (Collision col) {

	void OnCollisionEnter2D (Collision2D col) {

		int score = int.Parse (scoreGT.text);
		//Debug.Log ("collision");
		//Find out what hits the streetcar. If Red, destroy and add points
		GameObject collidedWith = col.gameObject;
		//Debug.Log ("Hit by " + collidedWith);
		if (collidedWith.tag == "Blue") {
			//Debug.Log ("+1");
			Destroy (collidedWith);
			score += 1;
			scoreGT.text = score.ToString ();
		}

	}

}
