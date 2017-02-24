using UnityEngine;
using System.Collections;

public class StreetcarInput : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] float moveSpeed = 50;
	[SerializeField] float maxSpeed = 10;

	[Header("Keyboard Input")]
	[SerializeField] KeyCode moveLeft = KeyCode.A;
	[SerializeField] KeyCode moveRight = KeyCode.D;

	Rigidbody2D rb2d;
	private GameData gameData;


	void Awake () {

		gameData = GameObject.Find ("GameManager").GetComponent<GameData>();

		rb2d = this.GetComponent<Rigidbody2D> ();
	}

	void Update () {

		if (gameData.is_Game_Started == true) 
		{
			Vector3 easeVelocity = rb2d.velocity;
			easeVelocity.y = rb2d.velocity.y;
			easeVelocity.z = 0.0f;
			easeVelocity.x *= 0.9f;

			//Fake friction / Easing the x speed of the player
			rb2d.velocity = easeVelocity;

			float horizontalInput = 0;

			// Get control input
			if (Input.GetKey (moveLeft)) {

				horizontalInput = -1;
			} else if (Input.GetKey (moveRight)) {

				horizontalInput = 1;
			}
			//else if (Input.GetAxis ("Horizontal") != 0) {

			//	horizontalInput = Input.GetAxis ("Horizontal");
			//}
			

			//Moving the Player
			rb2d.AddForce ((Vector2.right * moveSpeed) * horizontalInput);

			//Limiting the speed of the player
			if (rb2d.velocity.x > maxSpeed) {
			
				rb2d.velocity = new Vector2 (maxSpeed * 2, rb2d.velocity.y);
			} else if (rb2d.velocity.x < -maxSpeed) {
			
				rb2d.velocity = new Vector2 (-maxSpeed * 2, rb2d.velocity.y);
			}
		}
	}
}
