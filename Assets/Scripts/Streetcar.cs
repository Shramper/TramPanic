using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class Streetcar : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] float acceleration = 0.001f;
	[SerializeField] float maxSpeed = 0.1f;
	public int maxPassengers;
	public float passengerLeaveRate;
	public int inspectorCount;
	public Text speedBoostUI;
	public GameObject[] CapacityCount;

	[Header("Score")]
	public static int score;
	public GUIText scoreGT;
	public GUIText capacity;
	public float scoreGTPlayer;

	[Header("Audio")]
	public AudioClip coinSound;
	public AudioClip fartSound;

	[Header("Passenger Info")]
	public List<Sprite> streetCarPassengers;
	public List<string> streetCarPassengersRole;
	public GameObject pedestrian;

	[Header("References")]
	[SerializeField] Animator effectsAnimator;

    [Header("Minimap")]
    public GameObject minimapStreetCar;

	[Header("Ability Data")]
	[SerializeField] Sprite[] abilitiesSprites;
	public SpriteRenderer FirstAbilitySprite;

    private Rigidbody2D rb2d;
	private Animator streetcarAnimator;
	private ColorStrobe colorStrobe;
	private float moveSpeed = 0;
	private bool changingAcceleration = false;
	private float counter;
	private int currentPassengers = 0;
	private bool stationUp = false;
	private bool stationDown = false;
	private bool chunkyOnBoard = false;
	private bool inspectorOnBoard = false;
	private bool canMove = true;
	private bool scoreMultiplier = false;
	private GameData gameData;

	public List<string> abilities = new List<string>(2);


	void Awake () {

		rb2d = this.GetComponent<Rigidbody2D> ();
		streetcarAnimator = this.GetComponent<Animator>();
		colorStrobe = this.GetComponentInChildren<ColorStrobe>();
		streetCarPassengers = new List<Sprite>();
		streetCarPassengersRole = new List<string>();

		speedBoostUI.text =  inspectorCount.ToString();
		FirstAbilitySprite = GameObject.Find ("AbilitySprite1").GetComponent<SpriteRenderer>();
		gameData = GameObject.Find ("GameManager").GetComponent<GameData>();



	}



	void FixedUpdate () {



		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			abilities.Add ("Speed Boost");
			inspectorCount++;

		}
		/*else if (Input.GetKeyDown (KeyCode.Alpha2)) 
		{
			abilities.Add ("Multiplier");
		}*/
        if (canMove && gameData.is_Game_Started == true)
        {

            // Give streetcar friction if not inputting acceleration
            if (!changingAcceleration)
            {
                moveSpeed *= 0.9f;
            }

            // Move the streetcar
            rb2d.MovePosition(this.transform.position + (Vector3.right * moveSpeed));

            //move minimap streetcar
            if (minimapStreetCar.transform.localPosition.x <= -6.7)
            {
                minimapStreetCar.transform.localPosition = new Vector2(-6.7f, minimapStreetCar.transform.localPosition.y);
            }

            else if (minimapStreetCar.transform.localPosition.x >= 6.7)
            {
                minimapStreetCar.transform.localPosition = new Vector2(6.7f, minimapStreetCar.transform.localPosition.y);
            }
            minimapStreetCar.transform.Translate(Vector3.right * moveSpeed * .33f);
        }

        if (stationDown) {
			DropOffPassengers (-.2f, -2);
		}
		else if (stationUp) {
			DropOffPassengers (.2f, 2);
		}

		if (Input.GetKeyDown (KeyCode.Space)) 
		{
			abilityControls ();
		}

		AbilitySpriteOrder ();
	}

	void OnCollisionEnter2D(Collision2D other) {

		if (other.gameObject.GetComponent<Pedestrian> () && currentPassengers != maxPassengers ) {

			Pedestrian collidedWith = other.gameObject.GetComponent<Pedestrian> ();

			//Debug.Log ("Hit by " + collidedWith);
			if (collidedWith.GetRole() == Role.Coin) {

				streetCarPassengers.Add(other.gameObject.GetComponent<SpriteRenderer>().sprite);
				streetCarPassengersRole.Add(other.gameObject.GetComponent<Pedestrian>().GetRoleString ());
				GetComponent<AudioSource>().clip = coinSound;
				GetComponent<AudioSource>().Play ();
				Destroy (other.gameObject);

				currentPassengers++;
				capacity.text = "P1 Capacity: " + currentPassengers + "/" + maxPassengers;
				streetcarAnimator.SetTrigger("Grow");

				for (int i = 0; i < currentPassengers; i++) {
					CapacityCount[i].active = true;
				}
			}
			else if (collidedWith.GetRole() == Role.Stink) {

				if (currentPassengers != 0)
				{
					currentPassengers--;
					streetCarPassengers.RemoveAt(currentPassengers);
					streetCarPassengersRole.RemoveAt(currentPassengers);
				} 

				GetComponent<AudioSource>().clip = fartSound;
				GetComponent<AudioSource>().Play ();
				Destroy (other.gameObject);
				capacity.text = "P1 Capacity: " + currentPassengers + "/" + maxPassengers;
				streetcarAnimator.SetTrigger("Shrink");
			}
			else if (collidedWith.GetRole() == Role.Chunky) 
			{	
				chunkyOnBoard = true;
				streetCarPassengers.Add(other.gameObject.GetComponent<SpriteRenderer>().sprite);
				streetCarPassengersRole.Add(other.gameObject.GetComponent<Pedestrian>().GetRoleString ());
				if (inspectorOnBoard == false) 
				{
					maxSpeed = 0.075f;
				} 

				else if (inspectorOnBoard == true) 
				{
					maxSpeed = 0.1f;
					acceleration = 0.001f;
					//chunkyOnBoard = false;
				}

				effectsAnimator.SetTrigger("Chunky");
				Destroy (other.gameObject);
				//Debug.Log ("chunky" + maxSpeed);
			}
			else if (collidedWith.GetRole() == Role.Inspector) 
			{
				abilities.Add ("Speed Boost");
				inspectorOnBoard = true;
				inspectorCount++;
				speedBoostUI.text =  inspectorCount.ToString();
				streetCarPassengers.Add(other.gameObject.GetComponent<SpriteRenderer>().sprite);
				streetCarPassengersRole.Add(other.gameObject.GetComponent<Pedestrian>().GetRoleString ());

				/*if (chunkyOnBoard == false) 
				{
					maxSpeed = 0.13f;
					acceleration = 0.0013f;
				}
				else if(chunkyOnBoard == true)
				{
					maxSpeed = 0.1f;
					acceleration = 0.001f;
					inspecterOnBoard = false;
				}*/
				Destroy(other.gameObject);
			}
			else if (collidedWith.GetRole() == Role.Dazer)
			{
				StartCoroutine (TempDisableMovement ());
				Destroy (other.gameObject);

			}
			else if(collidedWith.GetRole() == Role.Officer)
			{
				if(maxSpeed < 0.1f) { maxSpeed = 0.1f; }

				Camera.main.GetComponentInChildren<CameraOverlay>().ShowOverlay();

				GameObject[] allPedestrians = GameObject.FindGameObjectsWithTag("Pedestrian");
				foreach (GameObject pedestrianObject in allPedestrians) {

					Pedestrian pedestrian = pedestrianObject.GetComponent<Pedestrian>();

					if(pedestrian.GetRole() == Role.Chunky || pedestrian.GetRole() == Role.Dazer) {

						Destroy(pedestrian.gameObject);
					}
				}

				effectsAnimator.SetTrigger("Norm");

				Destroy (other.gameObject);
			}
			else if (collidedWith.GetRole() == Role.Raver)
			{
				scoreMultiplier = true;
			//	abilities.Add("Multiplier");
				streetCarPassengers.Add(other.gameObject.GetComponent<SpriteRenderer>().sprite);
				streetCarPassengersRole.Add(other.gameObject.GetComponent<Pedestrian>().GetRoleString ());
				colorStrobe.StartCoroutine(colorStrobe.RecursiveColorChange());
				Destroy(other.gameObject);
				currentPassengers++;
				capacity.text = "P1 Capacity: " + currentPassengers + "/" + maxPassengers;
			}
		}
	}

	IEnumerator TempDisableMovement () {

		canMove = false;

		effectsAnimator.SetTrigger("Dazer");
		this.GetComponent<SpriteRenderer>().color = Color.grey;
		colorStrobe.gameObject.GetComponent<SpriteRenderer>().color = Color.grey;
		Camera.main.GetComponent<CameraEffects> ().ShakeCamera ();

		yield return new WaitForSeconds (3);

		canMove = true;
		effectsAnimator.SetTrigger("Norm");
		this.GetComponent<SpriteRenderer>().color = Color.white;
		colorStrobe.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
	}

	IEnumerator speedBoost()
	{	
		if (chunkyOnBoard == false) {
			
			maxSpeed = 0.175f;
			acceleration = 0.005f;
			inspectorCount--;
			speedBoostUI.text = inspectorCount.ToString ();
			Debug.Log (maxSpeed);

		}

		else 
		{
			maxSpeed = 0.15f;
			inspectorCount--;
			speedBoostUI.text = inspectorCount.ToString ();
			Debug.Log (maxSpeed);
		
		}

		yield return new WaitForSeconds (2);

		if (chunkyOnBoard == false) {
			
			maxSpeed = 0.1f;
			acceleration = 0.001f;
		}
		else 
		{
			maxSpeed = 0.075f;
			acceleration = 0.001f;
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "StationUp")
		{
			stationUp = true;
			stationDown = false;
		}

		if (other.gameObject.tag == "StationDown")
		{
			stationUp = false;
			stationDown = true;
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "StationUp")
		{
			stationUp = false;
			stationDown = false;
		}

		if (other.gameObject.tag == "StationDown")
		{
			stationUp = false;
			stationDown = false;
		}
	}

	public void Accelerate () {

		changingAcceleration = true;
		if(moveSpeed < 0) { moveSpeed *= 0.9f; }
		moveSpeed += acceleration;
		moveSpeed = Mathf.Clamp (moveSpeed, -maxSpeed, maxSpeed);
	}

	public void Decelerate () {

		changingAcceleration = true;
		if(moveSpeed > 0) { moveSpeed *= 0.9f; }
		moveSpeed -= acceleration;
		moveSpeed = Mathf.Clamp (moveSpeed, -maxSpeed, maxSpeed);
	}

	public void EndAcceleration () {

		changingAcceleration = false;
	}

	public float GetMoveSpeed () {

		return moveSpeed;
	}
	public void abilityControls()
	{
		if (abilities.IndexOf ("Speed Boost") == 0 && inspectorCount > 0) 
		{	
			abilities.Remove ("Speed Boost");
			StartCoroutine (speedBoost ());
		}
		/*else if (abilities.IndexOf ("Multiplier") == 0)
		{	
			abilities.Remove("Multiplier");
			scoreMultiplier = true;
		}*/

    }

	public void DropOffPassengers(float yOffset, int pedestrianDirection)
	{
		if (streetCarPassengers.Count != 0)
		{
			counter += Time.deltaTime;
			int x = streetCarPassengers.Count - 1;
			if (counter > passengerLeaveRate) {
				GameObject pedestrianPrefab = Instantiate (pedestrian, this.transform.position + transform.up * yOffset, Quaternion.identity) as GameObject;
				pedestrianPrefab.GetComponent<SpriteRenderer> ().sprite = streetCarPassengers [x];
				pedestrianPrefab.GetComponent<Pedestrian> ().SetDestination(this.transform.position + new Vector3(0, pedestrianDirection, 0));
				pedestrianPrefab.GetComponent<Pedestrian>().SetMoveSpeed(1.5f);
				if (scoreMultiplier == true)
				{
					score += 2;
				}
				else
				{
					score += 1;
				}
				scoreGT.text = "P" + scoreGTPlayer.ToString () + ": " + score.ToString ();
				streetCarPassengers.RemoveAt (x);
				streetCarPassengersRole.RemoveAt(x);
				if (!streetCarPassengersRole.Contains("RAVER"))
				{
					scoreMultiplier = false;
					colorStrobe.StopAllCoroutines();
					colorStrobe.GetComponent<SpriteRenderer>().color = Color.white;
				}
				if(!streetCarPassengersRole.Contains("INSPECTOR"))
				{
					inspectorOnBoard = false;
					inspectorCount = 0;
					maxSpeed = 0.1f;
					acceleration = 0.001f;
					effectsAnimator.SetTrigger("Norm");
				}
				if (!streetCarPassengersRole.Contains("CHUNKY"))
				{
					chunkyOnBoard = false;
					maxSpeed = 0.1f;
					acceleration = 0.001f;
					effectsAnimator.SetTrigger("Norm");
				} 
					
				currentPassengers--;

				CapacityCount [currentPassengers].active = false;
				


				if(currentPassengers < 0) { currentPassengers = 0; }
				capacity.text = "P1 Capacity: " + currentPassengers + "/" + maxPassengers;
				counter = 0;
			}
		}


		/*else
		{
			chunkyOnBoard = false;
			inspecterOnBoard = false;
		}*/
	}

	public void AbilitySpriteOrder()
	{
			if (abilities.Count.Equals (0)) 
			{
				if (FirstAbilitySprite.sprite != null)
				{
				FirstAbilitySprite.sprite = null;
				Debug.Log ("List Empty");
				}
			} 
			else if (abilities.IndexOf ("Speed Boost") == 0) 
			{
				FirstAbilitySprite.sprite = abilitiesSprites [0];
			}

	}
}


/*

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class Streetcar : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] float acceleration = 0.001f;
	[SerializeField] float maxSpeed = 0.1f;
	public int maxPassengers;
	public float passengerLeaveRate;

	[Header("Score")]
	public int score;
	public GUIText scoreGT;
	public GUIText capacity;
	public float scoreGTPlayer;

	[Header("Audio")]
	public AudioClip coinSound;
	public AudioClip fartSound;

	[Header("Passenger Info")]
	public List<Sprite> streetCarPassengers;
	public List<string> streetCarPassengersRole;
	public GameObject pedestrian;

	[Header("References")]
	[SerializeField] Animator effectsAnimator;

	private Rigidbody2D rb2d;
	private Animator streetcarAnimator;
	private ColorStrobe colorStrobe;
	private float moveSpeed = 0;
	private bool changingAcceleration = false;
	private float counter;
	private int currentPassengers = 0;
	private bool stationUp = false;
	private bool stationDown = false;
	private bool chunkyOnBoard = false;
	private bool inspecterOnBoard = false;
	private bool canMove = true;
	private bool scoreMultiplier = false;


	void Awake () {

		rb2d = this.GetComponent<Rigidbody2D> ();
		streetcarAnimator = this.GetComponent<Animator>();
		colorStrobe = this.GetComponentInChildren<ColorStrobe>();
		streetCarPassengers = new List<Sprite>();
		streetCarPassengersRole = new List<string>();
	}

	void FixedUpdate () {

		if (canMove) {

			// Give streetcar friction if not inputting acceleration
			if (!changingAcceleration) {
				moveSpeed *= 0.9f;
			}

			// Move the streetcar
			rb2d.MovePosition (this.transform.position + (Vector3.right * moveSpeed));
		}

		if (stationDown) {
			DropOffPassengers (-.2f, -2);
		}
		else if (stationUp) {
			DropOffPassengers (.2f, 2);
		}
	}

	void OnCollisionEnter2D(Collision2D other) {

		if (other.gameObject.GetComponent<Pedestrian> () && currentPassengers != maxPassengers ) {

			Pedestrian collidedWith = other.gameObject.GetComponent<Pedestrian> ();

			//Debug.Log ("Hit by " + collidedWith);
			if (collidedWith.GetRole() == Role.Coin) {

				streetCarPassengers.Add(other.gameObject.GetComponent<SpriteRenderer>().sprite);
				streetCarPassengersRole.Add(other.gameObject.GetComponent<Pedestrian>().GetRoleString ());
				GetComponent<AudioSource>().clip = coinSound;
				GetComponent<AudioSource>().Play ();
				Destroy (other.gameObject);

				currentPassengers++;
				capacity.text = "P1 Capacity: " + currentPassengers + "/" + maxPassengers;
				streetcarAnimator.SetTrigger("Grow");
			}
			else if (collidedWith.GetRole() == Role.Stink) {

				if (currentPassengers != 0)
				{
					currentPassengers--;
					streetCarPassengers.RemoveAt(currentPassengers);
					streetCarPassengersRole.RemoveAt(currentPassengers);
				} 

				GetComponent<AudioSource>().clip = fartSound;
				GetComponent<AudioSource>().Play ();
				Destroy (other.gameObject);
				capacity.text = "P1 Capacity: " + currentPassengers + "/" + maxPassengers;
				streetcarAnimator.SetTrigger("Shrink");
			}
			else if (collidedWith.GetRole() == Role.Chunky) 
			{	
				chunkyOnBoard = true;
				streetCarPassengers.Add(other.gameObject.GetComponent<SpriteRenderer>().sprite);
				streetCarPassengersRole.Add(other.gameObject.GetComponent<Pedestrian>().GetRoleString ());
				if (inspecterOnBoard == false) 
				{
					maxSpeed = 0.075f;
				} 

				else if (inspecterOnBoard == true) 
				{
					maxSpeed = 0.1f;
					acceleration = 0.001f;
					chunkyOnBoard = false;
				}

				effectsAnimator.SetTrigger("Chunky");
				Destroy (other.gameObject);
				//Debug.Log ("chunky" + maxSpeed);
			}
			else if (collidedWith.GetRole() == Role.Inspector) 
			{
				inspecterOnBoard = true;
				streetCarPassengers.Add(other.gameObject.GetComponent<SpriteRenderer>().sprite);
				streetCarPassengersRole.Add(other.gameObject.GetComponent<Pedestrian>().GetRoleString ());

				if (chunkyOnBoard == false) 
				{
					maxSpeed = 0.13f;
					acceleration = 0.0013f;
				}
				else if(chunkyOnBoard == true)
				{
					maxSpeed = 0.1f;
					acceleration = 0.001f;
					inspecterOnBoard = false;
				}

				effectsAnimator.SetTrigger("Inspector");
				Destroy(other.gameObject);
			}
			else if (collidedWith.GetRole() == Role.Dazer)
			{
				StartCoroutine (TempDisableMovement ());
				Destroy (other.gameObject);

			}
			else if(collidedWith.GetRole() == Role.Officer)
			{
				if(maxSpeed < 0.1f) { maxSpeed = 0.1f; }

				Camera.main.GetComponentInChildren<CameraOverlay>().ShowOverlay();

				GameObject[] allPedestrians = GameObject.FindGameObjectsWithTag("Pedestrian");
				foreach (GameObject pedestrianObject in allPedestrians) {

					Pedestrian pedestrian = pedestrianObject.GetComponent<Pedestrian>();

					if(pedestrian.GetRole() == Role.Chunky || pedestrian.GetRole() == Role.Dazer) {

						Destroy(pedestrian.gameObject);
					}
				}

				effectsAnimator.SetTrigger("Norm");

				Destroy (other.gameObject);
			}
			else if (collidedWith.GetRole() == Role.Raver)
			{
				scoreMultiplier = true;
				streetCarPassengers.Add(other.gameObject.GetComponent<SpriteRenderer>().sprite);
				streetCarPassengersRole.Add(other.gameObject.GetComponent<Pedestrian>().GetRoleString ());
				colorStrobe.StartCoroutine(colorStrobe.RecursiveColorChange());
				Destroy(other.gameObject);
				currentPassengers++;
				capacity.text = "P1 Capacity: " + currentPassengers + "/" + maxPassengers;
			}
		}
	}

	IEnumerator TempDisableMovement () {

		canMove = false;

		effectsAnimator.SetTrigger("Dazer");
		this.GetComponent<SpriteRenderer>().color = Color.grey;
		colorStrobe.gameObject.GetComponent<SpriteRenderer>().color = Color.grey;

		yield return new WaitForSeconds (3);

		canMove = true;
		effectsAnimator.SetTrigger("Norm");
		this.GetComponent<SpriteRenderer>().color = Color.white;
		colorStrobe.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
	}
		
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "StationUp")
		{
			stationUp = true;
			stationDown = false;
		}

		if (other.gameObject.tag == "StationDown")
		{
			stationUp = false;
			stationDown = true;
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "StationUp")
		{
			stationUp = false;
			stationDown = false;
		}

		if (other.gameObject.tag == "StationDown")
		{
			stationUp = false;
			stationDown = false;
		}
	}

	public void Accelerate () {

		changingAcceleration = true;
		if(moveSpeed < 0) { moveSpeed *= 0.9f; }
		moveSpeed += acceleration;
		moveSpeed = Mathf.Clamp (moveSpeed, -maxSpeed, maxSpeed);
	}

	public void Decelerate () {

		changingAcceleration = true;
		if(moveSpeed > 0) { moveSpeed *= 0.9f; }
		moveSpeed -= acceleration;
		moveSpeed = Mathf.Clamp (moveSpeed, -maxSpeed, maxSpeed);
	}

	public void EndAcceleration () {

		changingAcceleration = false;
	}

	public float GetMoveSpeed () {

		return moveSpeed;
	}

	public void DropOffPassengers(float yOffset, int pedestrianDirection)
	{
		if (streetCarPassengers.Count != 0)
		{
			counter += Time.deltaTime;
			int x = streetCarPassengers.Count - 1;
			if (counter > passengerLeaveRate) {
				GameObject pedestrianPrefab = Instantiate (pedestrian, this.transform.position + transform.up * yOffset, Quaternion.identity) as GameObject;
				pedestrianPrefab.GetComponent<SpriteRenderer> ().sprite = streetCarPassengers [x];
				pedestrianPrefab.GetComponent<Pedestrian> ().SetDestination(this.transform.position + new Vector3(0, pedestrianDirection, 0));;
				if (scoreMultiplier == true)
				{
					score += 2;
				}
				else
				{
					score += 1;
				}
				scoreGT.text = "P" + scoreGTPlayer.ToString () + ": " + score.ToString ();
				streetCarPassengers.RemoveAt (x);
				streetCarPassengersRole.RemoveAt(x);
				if (!streetCarPassengersRole.Contains("RAVER"))
				{
					scoreMultiplier = false;
					colorStrobe.StopAllCoroutines();
					colorStrobe.GetComponent<SpriteRenderer>().color = Color.white;
				}
				if(!streetCarPassengersRole.Contains("INSPECTOR"))
				{
					inspecterOnBoard = false;
					maxSpeed = 0.1f;
					acceleration = 0.001f;
					effectsAnimator.SetTrigger("Norm");
				}
				if (!streetCarPassengersRole.Contains("CHUNKY"))
				{
					chunkyOnBoard = false;
					maxSpeed = 0.1f;
					acceleration = 0.001f;
					effectsAnimator.SetTrigger("Norm");
				} 
				currentPassengers--;
				if(currentPassengers < 0) { currentPassengers = 0; }
				capacity.text = "P1 Capacity: " + currentPassengers + "/" + maxPassengers;
				counter = 0;
			}
		}
		//else
		//{
		//	chunkyOnBoard = false;
		//	inspecterOnBoard = false;
		}
	}
} */

