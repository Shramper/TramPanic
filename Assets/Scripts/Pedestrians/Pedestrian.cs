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

	void Awake () {
        spriteRenderer = this.GetComponent<SpriteRenderer>();    
    // Set component references
        rb2d = this.GetComponent<Rigidbody2D> ();
		roleAnimator = this.transform.FindChild ("Role").GetComponent<Animator> ();

		//when spawned, set random speed
		moveSpeed = Random.Range (0.5f * moveSpeed, 1.25f * moveSpeed);
		avoidanceSpeed = 0.75f * moveSpeed;

		// Calculate raycast parameters
		// *note: using 0.25f because of the in-game scale of the pedestrian
		//colliderHalfWidth = 0.25f * this.GetComponentInChildren<CircleCollider2D> ().radius;
		//colliderHalfWidth *= 1.1f;

		//Destroy (this.gameObject, 15f);
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

		if(destination != Vector3.zero) {

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
	void OnTriggerStay2D(Collider2D other) {

		if(other.CompareTag("Streetcar") && Mathf.Abs(other.GetComponentInParent<Streetcar>().GetMoveSpeed()) < 0.01f) {

			if(role == Role.Coin) {

				// Move towards streetcar
				SetDestination(other.transform.position);
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
