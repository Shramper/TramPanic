using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Car : MonoBehaviour {

	[Header ("Speed Parameters")]
	public float vehicleSpeed;
	public float maxVehicleSpeed;
    //public float brakeMod = 3;
	//private float currentSpeed;
	[Header("Raycast Length")]
	public float rayLength;
	RaycastHit2D hit;

	public bool spotted = false;

	private Vector3 direction = Vector2.zero;
	public Rigidbody2D carRb = null;
	public int carFacing = 0;

    private List<GameObject> thingsInMyWay;

	// Use this for initialization
	void Start () 
	{
		vehicleSpeed = Random.Range(3500.0f,4000.0f);
        carRb = GetComponent<Rigidbody2D>();
        thingsInMyWay = new List<GameObject>();

		// rotate vehicle if spawned on bottom lane.  Move right instead of left
		if (this.transform.position.y < 0) {	
			transform.eulerAngles = new Vector3 (0, -180, 0);
			carFacing = 1;
			vehicleSpeed = -vehicleSpeed;
		}
		else
		{
			carFacing = -1;
		}

        direction = transform.right * carFacing;

	}

    private void Update()
    {
        foreach(GameObject thing in thingsInMyWay)
        {
            if (thing == null)
            {
                thingsInMyWay.Remove(thing);
                break;
            }
        }
    }

    // Update is called once per frame
    private void FixedUpdate () 
	{
        if (thingsInMyWay.Count > 0)
        {
            moveVehicle(-2);
        }
        else
        {
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
