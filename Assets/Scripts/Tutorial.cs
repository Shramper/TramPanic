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
	//public float moveSpeed = 5;
	public bool canMove = false;

    [SerializeField]
	private float maxSpeed;
    [SerializeField]
	private float acceleration;

    [Range(-1, 1)]
    private float axis = 0;

	Vector3 startPos;

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

			float h = axis == 0 ? Input.GetAxis ("Horizontal") : axis;
			Vector3 movement = new Vector3 (h, 0, 0) * -acceleration;

			//Debug.Log ("In Y Zone: " + moveY);

			if (h != 0) {

                //Debug.Log ("Horizontal");
                //Debug.Log ("Horizontal" + h);

                if (rb.velocity.sqrMagnitude < acceleration * acceleration)
                    rb.AddForce(movement);
            }
            else
            {
                if (rb.velocity.sqrMagnitude > Mathf.Epsilon)
                    rb.AddForce(-rb.velocity.normalized * acceleration);
                else
                    rb.velocity = Vector3.zero;
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
    public void MobileAcceleration(bool accel) {
    
    	accelerating = accel;
        axis += accel ? 1 : -1;
        Debug.Log(axis);
    }
    
    public void MobileDecceleration(bool accel) {

        //decelerating = accel;
        axis += accel ? -1 : 1;
        Debug.Log(axis);
    }

}
