using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {

	private enum systemType { Desktop, Mobile };
	private systemType system;

	[HideInInspector] public bool accelerating = false;
	[HideInInspector] public bool decelerating = false;
	bool thrusting = false;

	//Stuff
	private Rigidbody rb;
	public float moveSpeed = 5;
	public bool canMove = false;

	private float maxSpeed;
	private float acceleration;

	Vector3 startPos;

	//public float baseAcceleration;
	//public float baseMaxSpeed;

	[SerializeField] Animator leftButtonAnimator;
	[SerializeField] Animator rightButtonAnimator;

	[SerializeField] Animator anim;


	void Awake()
	{

		//CheckDeviceType ();
		rb = GetComponent<Rigidbody> ();

		//startPos = this.transform.position; //For resetting position when done with tutorial

		//maxSpeed = baseMaxSpeed;
		//acceleration = baseAcceleration;

	}
	
	// Update is called once per frame
	void Update () {

		//CheckInput ();

		if (anim.GetBool("tutorial")) { //If player is looking at the tutorial

			float h = Input.GetAxis ("Horizontal");
			Vector3 movement = new Vector3 (h, 0, 0) * -moveSpeed * Time.deltaTime;

			//Debug.Log ("In Y Zone: " + moveY);

			if (Input.GetAxis ("Horizontal") != 0) {

				//Debug.Log ("Horizontal");
				//Debug.Log ("Horizontal" + h);

				rb.MovePosition (transform.position + movement);

			}

		}

	}
		

//	void CheckDeviceType()
//	{
//
//		if (SystemInfo.deviceType == DeviceType.Desktop)
//			system = systemType.Desktop;
//		else if (SystemInfo.deviceType == DeviceType.Handheld)
//			system = systemType.Mobile;
//
//	}
//
//	void CheckInput()
//	{
//
//		//If user is on desktop.
//		if (system == systemType.Desktop)
//		{
//			//Presses movement key.
//			if (Input.GetKeyDown(KeyCode.D))
//			{
//				accelerating = true;
//			}
//			else if (Input.GetKeyDown(KeyCode.A))
//			{
//				decelerating = true;
//			}
//
//			//Releases movement key.
//			if (Input.GetKeyUp(KeyCode.D))
//			{
//				accelerating = false;
//			}
//			else if (Input.GetKeyUp(KeyCode.A))
//			{
//				decelerating = false;
//			}
//		}
//
//	}
//
//	public void MobileAcceleration(bool accel) {
//
//		accelerating = accel;
//
//	}
//
//	public void MobileDecceleration(bool accel) {
//
//		decelerating = accel;
//
//	}

}
