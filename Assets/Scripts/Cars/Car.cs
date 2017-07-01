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

	// Update is called once per frame
	void FixedUpdate () 
	{

        // Debug to test if currentSpeed never goes above vehicle max speed.
        //currentSpeed = carRb.GetComponent<Rigidbody2D>().velocity.x * carRb.GetComponent<Rigidbody2D>().mass;
        //Debug.Log("Current Speed =  " + currentSpeed + "  "  );
        //float colliderWidth = gameObject.GetComponent<BoxCollider2D>().size.x;
        //float minDistance = colliderWidth;
        if (thingsInMyWay.Count > 0)
        {
            //foreach (GameObject thing in thingsInMyWay)
            //{
            //    try
            //    {
            //        float x = Mathf.Abs(transform.position.x - thing.transform.position.x);
            //        minDistance = (x < minDistance) ? x : minDistance;
            //        if (minDistance > colliderWidth)
            //        {
            //            thingsInMyWay.Remove(thing);
            //        }
            //    }
            //    catch (MissingReferenceException)
            //    {
            //        thingsInMyWay.Remove(thing);
            //    }
            //}

            //Debug.Log(minDistance + " / " + colliderWidth + " = " + (minDistance/colliderWidth));
            //brake();
            moveVehicle(-2);
        }
        else
        {
            moveVehicle();
        }

		//collisionBehavior();
		//rayCastMethod();
	}

	// Method for car behavior if it detects any collider collision with cars raycast
	//public void collisionBehavior()
	//{
	//	this.direction = Vector3.zero;
    //
	//	if(!hit)
	//	{
	//		spotted = false;
    //
	//		this.direction += new Vector3(-0.6f,0, 0);
    //
	//	}
    //
	//	// decelerate vehicle if raycast detects collider.
	//	else if(hit.collider.tag == "Car")
	//	{
	//		spotted = true;
	//		//Debug.Log("COLLISION");
    //
	//		this.direction += new Vector3(0.15f, 0, 0);
    //
	//	}
	//}

	//public void rayCastMethod()
	//{	
	//	// Right car light raycast
	//	//	hit = Physics2D.Raycast(this.transform.position + rayStart(-1.0f, 0.5f,0), Vector3.left + Vector3.up, rayLength);
	//	//	Debug.DrawRay(this.transform.position + rayStart(-1.0f, 0.5f,0), Vector3.left + Vector3.up, Color.blue);
    //
	//	if(this.transform.position.y > 0)
	//	{
	//		hit = Physics2D.Raycast(this.transform.position + rayStart(-1.0f, 0, 0), Vector3.left, rayLength);
	//		//Debug.DrawRay(this.transform.position + rayStart(-1.0f, 0, 0),rayLength * Vector3.left, Color.green);
	//	}
	//	else if(this.transform.position.y < 0)
	//	{
	//		hit = Physics2D.Raycast(this.transform.position + rayStart(1.0f, 0, 0), Vector3.right, rayLength);
	//		//Debug.DrawRay(this.transform.position + rayStart(1.0f, 0, 0),rayLength * Vector3.right, Color.green);
	//	}
	//	//left car light raycast
	//	//	hit = Physics2D.Raycast(this.transform.position + rayStart(-1.0f, -0.5f, 0), (rayLength * Vector3.left) - rayStart(-1.0f, 1.0f, 0), rayLength - 1.0f);
	//	//	Debug.DrawRay(this.transform.position + rayStart(-1.0f, -0.5f, 0), (rayLength * Vector3.left) - rayStart(-1.0f, 1.0f, 0) , Color.yellow);
	//}


	// Method for raycast starting pointing.
	//private Vector3 rayStart(float x, float y, float z)
	//{
	//	return new Vector3( x, y, z);
	//}

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

        //float horizontalSpeed = Mathf.Abs(carRb.velocity.x);
        //
        //if(Mathf.Abs(horizontalSpeed) > maxVehicleSpeed)                                       
        //{	
        //	Vector3 newVehicleVelocity = carRb.velocity;
        //	// shorthand if statement.  If Rigidbody statement returns true, multiplier = 1, if returns false multiplier = -1.
        //	float multiplier = (carRb.velocity.x > 0) ? 1 : -1;
        //
        //	// Set velocity to a fixed value if velocity reaches max velocity.
        //	newVehicleVelocity.x = maxVehicleSpeed * multiplier;
        //	carRb.velocity = newVehicleVelocity;
        //}
    }

    //private void brake()
    //{
    //    
    //}

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

    //void OnCollisionEnter2D(Collision2D other)
    //{
    //	if (this.gameObject.tag == "Taxi" && other.gameObject.GetComponent<Pedestrian> ())
    //	{
    //		Pedestrian collidedWith = other.gameObject.GetComponent<Pedestrian> ();
    //
    //		//Debug.Log (this.gameObject.tag + " is the tag");
    //
    //		if (collidedWith.GetRole() ==  Role.Coin || collidedWith.GetRole() == Role.Inspector || collidedWith.GetRole() == Role.Officer|| collidedWith.GetRole() == Role.Raver) {	
    //
    //			//Debug.Log (collidedWith.role);
    //			Destroy (other.gameObject);
    //		}
    //	} 
    //	else if (this.gameObject.tag == "Police" && other.gameObject.GetComponent<Pedestrian> ())
    //	{	
    //		Pedestrian collidedWith = other.gameObject.GetComponent<Pedestrian> ();
    //
    //		//Debug.Log (this.gameObject.tag + " is the tag");
    //
    //		if (collidedWith.GetRole() == Role.Chunky || collidedWith.GetRole() == Role.Stink || collidedWith.GetRole() == Role.Dazer) 
    //		{	
    //			//Debug.Log (collidedWith.role);
    //			Destroy (other.gameObject);
    //		}
    //	}
    //}
}
