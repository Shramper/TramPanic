using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class OldPedestrian : MonoBehaviour {

	[SerializeField] float speed;
	[SerializeField] public float spawned;
	[SerializeField] public string role;
	[SerializeField] float avoidanceSpeed;
	[SerializeField] float raycastLength = 2;

	Rigidbody2D rb2d;
	Animator roleAnimator;

	// Raycast stuff
	float colliderHalfWidth;
	Vector3 leftRayStart;
	Vector3 rightRayStart;
	RaycastHit2D leftHit;
	RaycastHit2D rightHit;
	Vector3 direction;

	private int speedChange = 0;

	// Use this for initialization
	void Awake () {

		// Set component references
		rb2d = this.GetComponent<Rigidbody2D> ();
		roleAnimator = this.transform.FindChild ("Role").GetComponent<Animator> ();

		//when spawned, set random speed
		speed = Random.Range (0.025f, 0.045f);
		avoidanceSpeed = 0.75f * speed;


		// Calculate raycast parameters
		// *note: using 0.25f because of the in-game scale of the pedestrian
		colliderHalfWidth = 0.25f * this.GetComponentInChildren<CircleCollider2D> ().radius;
		colliderHalfWidth *= 1.1f;
	}

	// Update is called once per frame
	void FixedUpdate () {


		if (spawned < 1) { //Spawned from Top

			//MOVEMENT
			rb2d.MovePosition (this.transform.position + speed * Vector3.down);

			if (role == "NORM") {

				leftRayStart = this.transform.position - colliderHalfWidth * this.transform.right;
				Debug.DrawRay (leftRayStart, -raycastLength * Vector3.up, Color.red);
				leftHit = Physics2D.Raycast (leftRayStart, -this.transform.up, raycastLength);

				rightRayStart = this.transform.position + colliderHalfWidth * this.transform.right;
				Debug.DrawRay (rightRayStart, -raycastLength * Vector3.up, Color.red);
				rightHit = Physics2D.Raycast (rightRayStart, -this.transform.up, raycastLength);

				RaycastHit2D hit = new RaycastHit2D ();
				if (leftHit.collider != null && leftHit.collider.name.Contains ("Streetcar")) {

					hit = leftHit;
					//this.GetComponentInChildren<SpriteRenderer> ().color = Color.red;
					//Debug.Log ("hit left");
					//Debug.Break ();
				} else if (rightHit.collider != null && rightHit.collider.name.Contains ("Streetcar")) {

					hit = rightHit;
					//this.GetComponentInChildren<SpriteRenderer> ().color = Color.red;
					//Debug.Log ("hit right");
					//Debug.Break ();
				} else if (leftHit.collider != null && leftHit.collider.tag == "Car") {
					//Debug.Log ("hit1");
					hit = leftHit;

				} else if (rightHit.collider != null && rightHit.collider.tag == "Car") {
					//Debug.Log ("hit2");
					hit = rightHit;
				} else if (leftHit.collider != null && leftHit.collider.tag == "Police") {
					//Debug.Log ("hit1");
					hit = leftHit;

				} else if (rightHit.collider != null && rightHit.collider.tag == "Police") {
					//Debug.Log ("hit2");
					hit = rightHit;
				} else if (leftHit.collider != null && leftHit.collider.tag == "Taxi") {
					//Debug.Log ("hit1");
					hit = leftHit;

				} else if (rightHit.collider != null && rightHit.collider.tag == "Taxi") {
					//Debug.Log ("hit2");
					hit = rightHit;
				}


				// Check if either raycast hit a streetcar
				if (hit) {

					// Calculate additional speed
					//float streetcarSpeed = hit.collider.GetComponentInChildren<Streetcar>().GetMoveSpeed();
					// float totalAvoidanceSpeed = avoidanceSpeed + 2f * Mathf.Abs(streetcarSpeed);
					float totalAvoidanceSpeed = avoidanceSpeed + .05f;
					//Debug.Log ("streetcarSpeed: " + streetcarSpeed);
					//Debug.Log ("Avoiding " + hit.collider.name);

					if (hit.collider.name == "Streetcar 1") {
						if (this.transform.position.x >= hit.collider.transform.position.x) {

                            if (hit.collider.GetComponent<Streetcar>().accelerating == true) {
								rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.left + speed * Vector3.down);
							} else
								rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.right + speed * Vector3.down);
						} else if (this.transform.position.x < hit.collider.transform.position.x) {

							if (hit.collider.GetComponent<Streetcar>().decelerating == true) {
								rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.right + speed * Vector3.down);
							} else
								rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.left + speed * Vector3.down);

						}  
					}

					//changed
					else if (hit.collider.tag == "Car") {

						if (hit.collider.GetComponent<Car> ().carFacing < 0) {
							//Debug.Log ("Triggered1");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.right + speed * Vector3.down);
						} else if (hit.collider.GetComponent<Car> ().carFacing > 0) {
							//Debug.Log ("Triggered2");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.left + speed * Vector3.down);
						}
					} else if (hit.collider.tag == "Police") {

						if (hit.collider.GetComponent<Car> ().carFacing < 0) {
							//Debug.Log ("Triggered1");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.right + speed * Vector3.down);
						} else if (hit.collider.GetComponent<Car> ().carFacing > 0) {
							//Debug.Log ("Triggered2");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.left + speed * Vector3.down);
						}
					} else if (hit.collider.tag == "Taxi") {

						if (hit.collider.GetComponent<Car> ().carFacing < 0) {
							//Debug.Log ("Triggered1");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.right + speed * Vector3.down);
						} else if (hit.collider.GetComponent<Car> ().carFacing > 0) {
							//Debug.Log ("Triggered2");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.left + speed * Vector3.down);
						}
					}
				}
			} else if (role == "COIN" || role == "OFFICER" || role == "INSPECTOR" || role == "RAVER" /*|| role == "DAZER" || role == "STINK" || role == "CHUNKY"*/) {

				leftRayStart = this.transform.position - colliderHalfWidth * this.transform.right;
				Debug.DrawRay (leftRayStart, -raycastLength * Vector3.up, Color.green);
				leftHit = Physics2D.Raycast (leftRayStart, -this.transform.up, raycastLength);

				rightRayStart = this.transform.position + colliderHalfWidth * this.transform.right;
				Debug.DrawRay (rightRayStart, -raycastLength * Vector3.up, Color.green);
				rightHit = Physics2D.Raycast (rightRayStart, -this.transform.up, raycastLength);

				RaycastHit2D hit = new RaycastHit2D ();

				if (leftHit.collider != null && leftHit.collider.tag == "Car") {
					//Debug.Log ("hit1");
					hit = leftHit;

				} else if (rightHit.collider != null && rightHit.collider.tag == "Car") {
					//Debug.Log ("hit2");
					hit = rightHit;
				} else if (leftHit.collider != null && leftHit.collider.tag == "Police") {
					//Debug.Log ("hit1");
					hit = leftHit;

				} else if (rightHit.collider != null && rightHit.collider.tag == "Police") {
					//Debug.Log ("hit2");
					hit = rightHit;
				}


				// Check if either raycast hit a streetcar
				if (hit) {

					// Calculate additional speed
					//float streetcarSpeed = hit.collider.GetComponentInChildren<Streetcar>().GetMoveSpeed();
					// float totalAvoidanceSpeed = avoidanceSpeed + 2f * Mathf.Abs(streetcarSpeed);
					float totalAvoidanceSpeed = avoidanceSpeed + .05f;
					//Debug.Log ("streetcarSpeed: " + streetcarSpeed);
					//Debug.Log ("Avoiding " + hit.collider.name);


					if (hit.collider.tag == "Car") {

						if (hit.collider.GetComponent<Car> ().carFacing < 0) {
							//Debug.Log ("Triggered1");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.right + speed * Vector3.down);
						} else if (hit.collider.GetComponent<Car> ().carFacing > 0) {
							//Debug.Log ("Triggered2");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.left + speed * Vector3.down);
						}
					} else if (hit.collider.tag == "Police") {
						if (hit.collider.GetComponent<Car> ().carFacing < 0) {
							//Debug.Log ("Triggered1");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.right + speed * Vector3.down);
						} else if (hit.collider.GetComponent<Car> ().carFacing > 0) {
							//Debug.Log ("Triggered2");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.left + speed * Vector3.down);
						}
					}
				}
			} else if (role == "STINK" || role == "CHUNKY" || role == "DAZER") {
				leftRayStart = this.transform.position - colliderHalfWidth * this.transform.right;
				Debug.DrawRay (leftRayStart, -raycastLength * Vector3.up, Color.green);
				leftHit = Physics2D.Raycast (leftRayStart, -this.transform.up, raycastLength);

				rightRayStart = this.transform.position + colliderHalfWidth * this.transform.right;
				Debug.DrawRay (rightRayStart, -raycastLength * Vector3.up, Color.green);
				rightHit = Physics2D.Raycast (rightRayStart, -this.transform.up, raycastLength);

				RaycastHit2D hit = new RaycastHit2D ();

				if (leftHit.collider != null && leftHit.collider.tag == "Car") {
					//Debug.Log ("hit1");
					hit = leftHit;

				} else if (rightHit.collider != null && rightHit.collider.tag == "Car") {
					//Debug.Log ("hit2");
					hit = rightHit;
				} else if (leftHit.collider != null && leftHit.collider.tag == "Taxi") {
					//Debug.Log ("hit1");
					hit = leftHit;

				} else if (rightHit.collider != null && rightHit.collider.tag == "Taxi") {
					//Debug.Log ("hit2");
					hit = rightHit;
				}


				// Check if either raycast hit a streetcar
				if (hit) {

					// Calculate additional speed
					//float streetcarSpeed = hit.collider.GetComponentInChildren<Streetcar>().GetMoveSpeed();
					// float totalAvoidanceSpeed = avoidanceSpeed + 2f * Mathf.Abs(streetcarSpeed);
					float totalAvoidanceSpeed = avoidanceSpeed + .05f;
					//Debug.Log ("streetcarSpeed: " + streetcarSpeed);
					//Debug.Log ("Avoiding " + hit.collider.name);


					if (hit.collider.tag == "Car") {

						if (hit.collider.GetComponent<Car> ().carFacing < 0) {
							//Debug.Log ("Triggered1");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.right + speed * Vector3.down);
						} else if (hit.collider.GetComponent<Car> ().carFacing > 0) {
							//Debug.Log ("Triggered2");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.left + speed * Vector3.down);
						}
					} else if (hit.collider.tag == "Taxi") {

						if (hit.collider.GetComponent<Car> ().carFacing < 0) {
							//Debug.Log ("Triggered1");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.right + speed * Vector3.down);
						} else if (hit.collider.GetComponent<Car> ().carFacing > 0) {
							//Debug.Log ("Triggered2");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.left + speed * Vector3.down);
						}
					}

				}  
			}
		}
		else if (spawned > 0) { //Spawned from Bottom

			//MOVEMENT
			rb2d.MovePosition(this.transform.position + speed * Vector3.up);

			if (role == "NORM") {

				leftRayStart = this.transform.position - colliderHalfWidth * this.transform.right;
				Debug.DrawRay (leftRayStart, raycastLength * Vector3.up, Color.red);
				leftHit = Physics2D.Raycast (leftRayStart, this.transform.up, raycastLength);

				rightRayStart = this.transform.position + colliderHalfWidth * this.transform.right;
				Debug.DrawRay (rightRayStart, raycastLength * Vector3.up, Color.red);
				rightHit = Physics2D.Raycast (rightRayStart, this.transform.up, raycastLength);

				RaycastHit2D hit = new RaycastHit2D();
				if (leftHit.collider != null && leftHit.collider.name.Contains ("Streetcar")) {

					hit = leftHit;
					//this.GetComponentInChildren<SpriteRenderer> ().color = Color.red;
					//Debug.Log ("hit left");
					//Debug.Break ();
				}

				else if (rightHit.collider != null && rightHit.collider.name.Contains ("Streetcar")) {

					hit = rightHit;
					//this.GetComponentInChildren<SpriteRenderer> ().color = Color.red;
					//Debug.Log ("hit right");
					//Debug.Break ();
				}

				else if (leftHit.collider != null && leftHit.collider.tag == "Car")
				{
					//Debug.Log ("hit3");
					hit = leftHit;

				}
				else if (rightHit.collider != null && rightHit.collider.tag == "Car") 
				{
					//Debug.Log ("hit4");
					hit = rightHit;

				}
				else if (leftHit.collider != null && leftHit.collider.tag == "Police")
				{
					//Debug.Log ("hit1");
					hit = leftHit;

				}
				else if (rightHit.collider != null && rightHit.collider.tag == "Police") 
				{
					//Debug.Log ("hit2");
					hit = rightHit;
				}
				else if (leftHit.collider != null && leftHit.collider.tag == "Taxi")
				{
					//Debug.Log ("hit1");
					hit = leftHit;

				}
				else if (rightHit.collider != null && rightHit.collider.tag == "Taxi") 
				{
					//Debug.Log ("hit2");
					hit = rightHit;
				}


				// Check if either raycast hit a streetcar
				if (hit) {

					// Calculate additional speed
					//float streetcarSpeed = hit.collider.GetComponentInChildren<Streetcar>().GetMoveSpeed();
					// float totalAvoidanceSpeed = avoidanceSpeed + 2f * Mathf.Abs(streetcarSpeed);
					float totalAvoidanceSpeed = avoidanceSpeed + .05f;
					//Debug.Log ("streetcarSpeed: " + streetcarSpeed);
					//Debug.Log ("Avoiding " + hit.collider.name);

					if (hit.collider.name == "Streetcar 1" ) {
						if (this.transform.position.x >= hit.collider.transform.position.x) {

							if (hit.collider.GetComponent<Streetcar>().accelerating == true) {
								rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.left + speed * Vector3.up);
							}

							else
								rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.right + speed * Vector3.up);
						}  



						else if ( this.transform.position.x < hit.collider.transform.position.x) {

							if (hit.collider.GetComponent<Streetcar>().decelerating == true) {
								rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.right + speed * Vector3.up);
							}

							else
								rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.left + speed * Vector3.up);

						}  


					}

					else if (hit.collider.tag == "Car") 
					{
						if (hit.collider.GetComponent<Car> ().carFacing < 0) {
							//Debug.Log ("Triggered3");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.right + speed * Vector3.up);
						}  else if (hit.collider.GetComponent<Car> ().carFacing > 0) {
							//Debug.Log ("Triggered4");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.left + speed * Vector3.up);
						}
					}
					else if(hit.collider.tag == "Police")
					{

						if (hit.collider.GetComponent<Car>().carFacing < 0) 
						{
							//Debug.Log ("Triggered1");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.right + speed * Vector3.down);
						}  

						else if(hit.collider.GetComponent<Car>().carFacing > 0)
						{
							//Debug.Log ("Triggered2");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.left + speed * Vector3.down);
						}
					}
					else if(hit.collider.tag == "Taxi")
					{

						if (hit.collider.GetComponent<Car>().carFacing < 0) 
						{
							//Debug.Log ("Triggered1");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.right + speed * Vector3.down);
						}  

						else if(hit.collider.GetComponent<Car>().carFacing > 0)
						{
							//Debug.Log ("Triggered2");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.left + speed * Vector3.down);
						}
					}
				}           
			}

			else if (role == "COIN" || role == "INSPECTOR" || role == "RAVER" || role == "OFFICER" /* || role == "DAZER" || role == "STINK" || role == "CHUNKY"*/ ) 
			{

				leftRayStart = this.transform.position - colliderHalfWidth * this.transform.right;
				Debug.DrawRay (leftRayStart, raycastLength * Vector3.up, Color.green);
				leftHit = Physics2D.Raycast (leftRayStart, this.transform.up, raycastLength);

				rightRayStart = this.transform.position + colliderHalfWidth * this.transform.right;
				Debug.DrawRay (rightRayStart, raycastLength * Vector3.up, Color.green);
				rightHit = Physics2D.Raycast (rightRayStart, this.transform.up, raycastLength);

				RaycastHit2D hit = new RaycastHit2D();
				if (leftHit.collider != null && leftHit.collider.tag == "Car")
				{
					//Debug.Log ("hit3");
					hit = leftHit;

				}
				else if (rightHit.collider != null && rightHit.collider.tag == "Car") 
				{
					//Debug.Log ("hit4");
					hit = rightHit;

				}
				else if (leftHit.collider != null && leftHit.collider.tag == "Police")
				{
					//Debug.Log ("hit3");
					hit = leftHit;

				}
				else if (rightHit.collider != null && rightHit.collider.tag == "Police") 
				{
					//Debug.Log ("hit4");
					hit = rightHit;

				}


				// Check if either raycast hit a streetcar
				if (hit) {

					// Calculate additional speed
					//float streetcarSpeed = hit.collider.GetComponentInChildren<Streetcar>().GetMoveSpeed();
					// float totalAvoidanceSpeed = avoidanceSpeed + 2f * Mathf.Abs(streetcarSpeed);
					float totalAvoidanceSpeed = avoidanceSpeed + .05f;
					//Debug.Log ("streetcarSpeed: " + streetcarSpeed);
					//Debug.Log ("Avoiding " + hit.collider.name);


					if (hit.collider.tag == "Car") {
						if (hit.collider.GetComponent<Car> ().carFacing < 0) {
							//Debug.Log ("Triggered3");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.right + speed * Vector3.up);
						}  else if (hit.collider.GetComponent<Car> ().carFacing > 0) {
							//Debug.Log ("Triggered4");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.left + speed * Vector3.up);
						}
					}
					else if (hit.collider.tag == "Police") {
						if (hit.collider.GetComponent<Car> ().carFacing < 0) {
							//Debug.Log ("Triggered3");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.right + speed * Vector3.up);
						}  else if (hit.collider.GetComponent<Car> ().carFacing > 0) {
							//Debug.Log ("Triggered4");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.left + speed * Vector3.up);
						}
					}
				}           
			}
			else if(role == "STINK" || role == "CHUNKY" || role == "DAZER")
			{

				leftRayStart = this.transform.position - colliderHalfWidth * this.transform.right;
				Debug.DrawRay (leftRayStart, raycastLength * Vector3.up, Color.green);
				leftHit = Physics2D.Raycast (leftRayStart, this.transform.up, raycastLength);

				rightRayStart = this.transform.position + colliderHalfWidth * this.transform.right;
				Debug.DrawRay (rightRayStart, raycastLength * Vector3.up, Color.green);
				rightHit = Physics2D.Raycast (rightRayStart, this.transform.up, raycastLength);

				RaycastHit2D hit = new RaycastHit2D();
				if (leftHit.collider != null && leftHit.collider.tag == "Car")
				{
					//Debug.Log ("hit3");
					hit = leftHit;

				}
				else if (rightHit.collider != null && rightHit.collider.tag == "Car") 
				{
					//Debug.Log ("hit4");
					hit = rightHit;

				}
				else if (leftHit.collider != null && leftHit.collider.tag == "Taxi")
				{
					//Debug.Log ("hit3");
					hit = leftHit;

				}
				else if (rightHit.collider != null && rightHit.collider.tag == "Taxi") 
				{
					//Debug.Log ("hit4");
					hit = rightHit;

				}


				// Check if either raycast hit a streetcar
				if (hit) {

					// Calculate additional speed
					//float streetcarSpeed = hit.collider.GetComponentInChildren<Streetcar>().GetMoveSpeed();
					// float totalAvoidanceSpeed = avoidanceSpeed + 2f * Mathf.Abs(streetcarSpeed);
					float totalAvoidanceSpeed = avoidanceSpeed + .05f;
					//Debug.Log ("streetcarSpeed: " + streetcarSpeed);
					//Debug.Log ("Avoiding " + hit.collider.name);


					if (hit.collider.tag == "Car") {
						if (hit.collider.GetComponent<Car> ().carFacing < 0) {
							//Debug.Log ("Triggered3");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.right + speed * Vector3.up);
						}  else if (hit.collider.GetComponent<Car> ().carFacing > 0) {
							//Debug.Log ("Triggered4");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.left + speed * Vector3.up);
						}
					}
					else if (hit.collider.tag == "Taxi") {
						if (hit.collider.GetComponent<Car> ().carFacing < 0) {
							//Debug.Log ("Triggered3");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.right + speed * Vector3.up);
						}  else if (hit.collider.GetComponent<Car> ().carFacing > 0) {
							//Debug.Log ("Triggered4");
							rb2d.MovePosition (this.transform.position + totalAvoidanceSpeed * Vector3.left + speed * Vector3.up);
						}
					}
				}         
			}
		}

		destroyPedestrian();
	}



	public void SetRole(string newRole) {

		role = newRole;

		if (role == "COIN")
		{
			Destroy (GetComponent< ParticleSystem> ());
			roleAnimator.SetTrigger ("Coin");
		} 
		else if (role == "STINK")
		{
			Destroy (GetComponent< ParticleSystem> ());
			roleAnimator.SetTrigger ("Stink");
		} 
		else if (role == "NORM") 
		{
			Destroy (GetComponent< ParticleSystem>());
		}
		else if (role == "CHUNKY")
		{
			roleAnimator.SetTrigger ("Chunky");
		} 
		else if (role == "INSPECTOR") 
		{
			roleAnimator.SetTrigger ("Inspector");
		}
		else if (role == "DAZER")
		{	
			roleAnimator.SetTrigger ("Dazer");
		}
		else if(role == "OFFICER")
		{
			roleAnimator.SetTrigger ("Officer");
		}
		else if (role == "RAVER")
		{
			roleAnimator.SetTrigger("Raver");
		}
	}

	public void destroyPedestrian()
	{
		if(transform.position.y > 7.5f || transform.position.y < -7.5f)
		{
			Destroy(this.gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag.Contains("Station"))
		{
			Destroy(this.gameObject);
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.tag == "Streetcar" && speedChange == 0 && role == "NORM")
		{
			speed *= 2;
			speedChange += 1;
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "Streetcar" && speedChange == 1 && role == "NORM")
		{
			speed /= 2f;
			speedChange += 1;
		}

	}
}