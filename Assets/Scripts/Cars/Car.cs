using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Car : MonoBehaviour {

	[Header ("Speed Parameters")]
	[SerializeField] private float vehicleSpeed;
	[SerializeField] private float maxVehicleSpeed;

	private List<GameObject> thingsInMyWay;

    	private int carFacing = 0;
	private float triggerWidth;
	
	private Rigidbody2D carRb = null;
	
	// Use this for initialization
	void Start () 
	{
		vehicleSpeed = Random.Range(3500.0f,4000.0f);
        carRb = GetComponent<Rigidbody2D>();
        thingsInMyWay = new List<GameObject>();
        triggerWidth = GetComponent<BoxCollider2D>().size.x;

		// rotate vehicle if spawned on bottom lane.  Move right instead of left
		if (transform.position.y < 0) 
		{	
			transform.eulerAngles = new Vector3 (0, -180, 0);
			carFacing = 1;
			vehicleSpeed = -vehicleSpeed;
		}
		else
		{
			carFacing = -1;
		}
	}

    private void Update()
    {
		// Check all obstacles
        foreach(GameObject thing in thingsInMyWay)
        {
	    	// to see if they are deleted or missed the trigger exit somehow
            if (thing == null ||
                Vector3.Distance(transform.position, thing.transform.position) > triggerWidth)
            {
                thingsInMyWay.Remove(thing);
                break; // do not continue iterating a container that's length has changed
            }
        }
    }

    // Update is called once per frame
    private void FixedUpdate () 
	{
        if (thingsInMyWay.Count > 0)
        {
			// brake
            moveVehicle(-2);
        }
        else
        {
			// drive
            moveVehicle();
        }
	}

	private void moveVehicle(float mod = 1)
	{
		// Grabs rigidbody and applies a force and direction.
		carRb.AddForce(transform.right * carFacing * vehicleSpeed * mod);

        float xVel = carRb.velocity.x;

        if (carFacing > 0)
        {
            xVel = Mathf.Clamp(xVel, maxVehicleSpeed/5, maxVehicleSpeed);
        }
        else
        {
            xVel = Mathf.Clamp(xVel, -maxVehicleSpeed, -maxVehicleSpeed/5);
        }

        carRb.velocity = Vector3.right * xVel;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pedestrian"))
        {
            thingsInMyWay.Add(collision.gameObject);
        }
        else if (collision.CompareTag("Car") ||
            collision.CompareTag("Taxi") ||
            collision.CompareTag("Police"))
        {
            if (carFacing * collision.transform.position.x >
                carFacing * transform.position.x)
            {
                thingsInMyWay.Add(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        thingsInMyWay.Remove(collision.gameObject);
    }
}
