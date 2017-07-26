using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Pedestrian : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] private float moveSpeed;
	[SerializeField] private float raycastLength = 2;
    public static Transform[] heightReferences;

    private static int[] mappedSortingLayers;
    private static int[] mappedSortingOrders;

	// Private variables
	private Rigidbody2D rb2d;
	private Animator roleAnimator;
    private SpriteRenderer spriteRenderer;
    private Role role;
    private bool raving = false;
    public bool busStopPedestrian = false;
	private float spawnTime;
	private float moveDelayTime = 0;
	private float startingY = 0;

    // For returning to sidewalk if full.
    private Streetcar streetcarRef;
    private Vector3 destination = Vector3.zero;
    private Vector3 finalDestination;
    private Vector3 returnDest = Vector3.zero;
    private bool returning = false;
    private bool boarding = false;
    private float pauseLength = 2.0f;
    public int pedestrianID;

    private void Awake ()
    {        
		// Set component references
		spriteRenderer = GetComponent<SpriteRenderer>();    
        rb2d = GetComponent<Rigidbody2D> ();
		roleAnimator = transform.FindChild ("Role").GetComponent<Animator> ();

		//when spawned, set random speed
		moveSpeed = Random.Range (0.5f * moveSpeed, 1.25f * moveSpeed);

		spawnTime = Time.time;
		startingY = transform.position.y;
	}

    /// <summary>
    /// Try to keep track of the sorting layer I should be on
    /// when I am at certain heights.
    /// </summary>
    private void Start()
    {
        if (mappedSortingOrders == null && mappedSortingLayers == null)
        {
            mappedSortingOrders = new int[heightReferences.Length];
            mappedSortingLayers = new int[heightReferences.Length];

            for (int i = 0; i < heightReferences.Length; i++)
            {
                mappedSortingLayers[i] = heightReferences[i].GetComponent<SpriteRenderer>().sortingLayerID;
                mappedSortingOrders[i] = heightReferences[i].GetComponent<SpriteRenderer>().sortingOrder;
            }
        }
    }


    /// <summary>
    /// Try to move from layer to layer as I 
    /// change height on the screen.
    /// </summary>
    private void Update ()
    {
        int minIndex = 0;
        float minDistance = Mathf.Infinity;

        for(int i = 0; i < heightReferences.Length; i++)
        {
            float distance = Mathf.Abs(transform.position.y - heightReferences[i].localPosition.y);
            if (distance < minDistance)
            {
                minDistance = distance;
                minIndex = i;
            }
        }

        spriteRenderer.sortingLayerID = mappedSortingLayers[minIndex];
        spriteRenderer.sortingOrder = mappedSortingOrders[minIndex];

        if (role == Role.Raver && !raving)
        {
            raving = true;
            StartCoroutine(ColorChange());
        }

        //Check if full constantly while boarding, if full then return to sidewalk immidiately.
        //Also return to sidewalk if pedestrian reaches destination but streetcar has moved away.
        if (boarding)
        {
            if (streetcarRef.IsFull() || Vector3.Distance(transform.position, destination) < 0.1f)
            {
                Debug.Log("Full!!!!!!!!!");
                boarding = false;
                returning = true;
                destination = returnDest;
            }
        }
        
        //If the pedestrian reaches the streetcar and it is full, they return to the sidewalk.
        //Once the reach the sidewalk, they carry on to their destination.
        if (returning)
        {
            if (Vector3.Distance(transform.position, destination) < 0.1f)
            {
                returning = false;
                returnDest = Vector3.zero;
                destination = finalDestination;
            }
        }
	}

    private void FixedUpdate()
    {
        MovePedestrian();
        avoidTraffic();
    }

    /// <summary>
    /// Cast raycasts out looking for cars in the direction I am moving
    /// If I find a car in my way, move the opposite way it's moving.
    /// Adjust my movement based on how far the car is, and make sure
    /// I end up going the same speed I started at the end.
    /// </summary>
    private void avoidTraffic()
    {
        LayerMask mask = LayerMask.GetMask("Car");
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), rb2d.velocity, raycastLength, mask.value);

        if(hit.collider != null)
        {
            //Debug.Log("Hit: " + hit.transform.gameObject.name);
            float speed = rb2d.velocity.magnitude;
            Vector2 otherVel = hit.transform.gameObject.GetComponent<Rigidbody2D>().velocity;
            rb2d.velocity -= otherVel * (1 - (Vector2.Distance(transform.position, hit.transform.position)/raycastLength));
            rb2d.velocity = rb2d.velocity.normalized * speed;
        }
    }

	void MovePedestrian ()
    {
		if(destination != Vector3.zero && (Time.time - spawnTime) > moveDelayTime) {
            if (!busStopPedestrian)
            {
                if (Vector3.Distance(transform.position, destination) > 0.1f)
                {
					Vector3 direction = (destination - transform.position).normalized;
					rb2d.velocity = direction * moveSpeed;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
			else
            {
                Vector3 direction = (destination - transform.position).normalized;
                rb2d.MovePosition(transform.position + moveSpeed * direction * Time.deltaTime);
            }
        }
	}
	
    public void CheckIfBusStopPedestrian()
    {
        busStopPedestrian = true;
    }

    #region CollisionDetection
    void OnCollisionStay2D (Collision2D other) {

		if(other.transform.CompareTag("Streetcar")) {
			
			if(other.transform.GetComponent<Streetcar>().IsFull()) 
			{
				destination = new Vector3(transform.position.x, startingY, transform.position.z);
            }
		}
	}

    //When the streetcar is close and it has stopped to pick up passengers.
	void OnTriggerStay2D(Collider2D other)
    {
        //If the pedestrian is not already returning because the streetcar was full.
        if (!returning)
        {
            if (other.CompareTag("Streetcar") && Mathf.Abs(other.GetComponentInParent<Streetcar>().GetMoveSpeed()) < 0.01f)
            {
                //Debug.Log(streetcarRef);
                //If the streetcar is not full, record the return point in case it fills up before the passenger gets to the car.
                if (other.GetComponentInParent<Streetcar>() && other.GetComponentInParent<Streetcar>().IsFull() == false)
                {
                    if (role == Role.Coin)
                    {
                        if (returnDest == Vector3.zero)
                            returnDest = gameObject.transform.position;

                        streetcarRef = other.gameObject.GetComponentInParent<Streetcar>();
                        destination = other.transform.position;
                        boarding = true;
                    }
                }
            }
        }
	}
	#endregion

	#region Setters
	// Called from PedestrianSpawner.cs
	public void SetRole(Role newRole)
    {
		role = newRole;
		roleAnimator.SetTrigger(role.ToString());
	}

	// Called from PedestrianSpawner.cs
	public void SetDestination (Vector3 newDestination)
    {
		destination = newDestination;
        finalDestination = newDestination;
	}

	public void SetMoveSpeed (float newMoveSpeed)
    {
		moveSpeed = newMoveSpeed;
	}

	public void SetMoveDelayTime (float newTime)
    {
		moveDelayTime = newTime;
	}
	#endregion

	#region Getters
	public string GetRoleString () {

		return role.ToString();
	}

	public Role GetRole () {

		return role;
	}
    #endregion

    public IEnumerator ColorChange()
    {
        while (raving)
        {
            spriteRenderer.color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
            yield return new WaitForSeconds(0.1f);
        }
    }
}