using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TODO:
// Re-add avoidance collision detection

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Pedestrian : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] private float moveSpeed;
	[SerializeField] private float raycastLength = 2;
    public Transform[] heightReferences;

    private int[] mappedSortingLayers;
    private int[] mappedSortingOrders;

	// Private variables
	private Rigidbody2D rb2d;
	private Animator roleAnimator;
    private SpriteRenderer spriteRenderer;
    private Role role;
	private Vector3 destination = Vector3.zero;
	private float avoidanceSpeed;
    private bool raving = false;
    public bool busStopPedestrian = false;
	private float spawnTime;
	private float moveDelayTime = 0;
	private float startingY = 0;

	private void Awake () {
            
		// Set component references
		spriteRenderer = this.GetComponent<SpriteRenderer>();    
        rb2d = this.GetComponent<Rigidbody2D> ();
		roleAnimator = this.transform.Find ("Role").GetComponent<Animator> ();

		//when spawned, set random speed
		moveSpeed = Random.Range (0.5f * moveSpeed, 1.25f * moveSpeed);
		avoidanceSpeed = 0.75f * moveSpeed;

		// Calculate raycast parameters
		// *note: using 0.25f because of the in-game scale of the pedestrian
		//colliderHalfWidth = 0.25f * this.GetComponentInChildren<CircleCollider2D> ().radius;
		//colliderHalfWidth *= 1.1f;

		spawnTime = Time.time;
		startingY = this.transform.position.y;
	}

    private void Start()
    {
        mappedSortingOrders = new int[heightReferences.Length];
        mappedSortingLayers = new int[heightReferences.Length];

        for(int i = 0; i < heightReferences.Length; i++)
        {
            mappedSortingLayers[i] = heightReferences[i].GetComponent<SpriteRenderer>().sortingLayerID;
            mappedSortingOrders[i] = heightReferences[i].GetComponent<SpriteRenderer>().sortingOrder;
        }
    }

    private void Update () {

		MovePedestrian ();

        for(int i = 0; i < heightReferences.Length; i++)
        {
            if (transform.position.y < heightReferences[i].position.y)
            {
                spriteRenderer.sortingLayerID = mappedSortingLayers[i];
                spriteRenderer.sortingOrder = mappedSortingOrders[i];
            }
        }

        if (role == Role.Raver && !raving)
        {
            raving = true;
            StartCoroutine(ColorChange());
        }
	}

	void MovePedestrian ()
    {
		if(destination != Vector3.zero && (Time.time - spawnTime) > moveDelayTime) {
            if (busStopPedestrian == false)
            {
                if (Vector3.Distance(this.transform.position, destination) > 0.1f)
                {
                    Vector3 direction = (destination - this.transform.position).normalized;
                    rb2d.MovePosition(this.transform.position + moveSpeed * direction * Time.deltaTime);
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }
            if (busStopPedestrian == true)
            {
                Vector3 direction = (destination - this.transform.position).normalized;
                rb2d.MovePosition(this.transform.position + moveSpeed * direction * Time.deltaTime);
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
			
			if(other.transform.GetComponent<Streetcar>().IsFull()) {
				destination = new Vector3(this.transform.position.x, startingY, this.transform.position.z);
                //Destroy(this.gameObject);
            }
		}
	}
    void OnTriggerStay2D(Collider2D other) {

		if(other.CompareTag("Streetcar") && Mathf.Abs(other.GetComponentInParent<Streetcar>().GetMoveSpeed()) < 0.01f) {

			if(other.GetComponentInParent<Streetcar>() && other.GetComponentInParent<Streetcar>().IsFull() == false) {

				if(role == Role.Coin) {
                    // Move towards streetcar
                    SetDestination(other.transform.position);
                }
			}
		}
    }
	#endregion

	#region Setters
	// Called from PedestrianSpawner.cs
	public void SetRole(Role newRole) {

		role = newRole;
		roleAnimator.SetTrigger(role.ToString());
	}

	// Called from PedestrianSpawner.cs
	public void SetDestination (Vector3 newDestination) {

		destination = newDestination;
	}

	public void SetMoveSpeed (float newMoveSpeed) {

		moveSpeed = newMoveSpeed;
	}

	public void SetMoveDelayTime (float newTime) {

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
