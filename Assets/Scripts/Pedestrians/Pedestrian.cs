using UnityEngine;
using System.Collections;

// TODO:
// Re-add avoidance collision detection

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Pedestrian : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] float moveSpeed;
	[SerializeField] float raycastLength = 2;

	// Private variables
	Rigidbody2D rb2d;
	Animator roleAnimator;
    SpriteRenderer spriteRenderer;
    Role role;
	Vector3 destination = Vector3.zero;
	float avoidanceSpeed;
    bool raving = false;
	float spawnTime;
	float moveDelayTime = 0;
	float startingY = 0;

	void Awake () {
            
		// Set component references
		spriteRenderer = this.GetComponent<SpriteRenderer>();    
        rb2d = this.GetComponent<Rigidbody2D> ();
		roleAnimator = this.transform.FindChild ("Role").GetComponent<Animator> ();

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

	void Update () {

		MovePedestrian ();

        if (role == Role.Raver && !raving)
        {
            raving = true;
            StartCoroutine("RecursiveColorChange");
        }
	}

	void MovePedestrian () {

		if(destination != Vector3.zero && (Time.time - spawnTime) > moveDelayTime) {

			if(Vector3.Distance(this.transform.position, destination) > 0.1f) {

				Vector3 direction = (destination - this.transform.position).normalized;
				rb2d.MovePosition(this.transform.position + moveSpeed * direction * Time.deltaTime);
			}
			else {

				Destroy(this.gameObject);
			}
		}
	}

	#region CollisionDetection
	void OnCollisionStay2D (Collision2D other) {

		if(other.transform.CompareTag("Streetcar")) {
			
			if(other.transform.GetComponent<Streetcar>().IsFull()) {

				destination = new Vector3(this.transform.position.x, startingY, this.transform.position.z);
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

    public IEnumerator RecursiveColorChange()
    {

        spriteRenderer.color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));

        yield return new WaitForSeconds(0.1f);

        StartCoroutine(RecursiveColorChange());
    }
}
