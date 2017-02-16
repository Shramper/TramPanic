using UnityEngine;
using System.Collections;

public class BasicMovementScript : MonoBehaviour {

	public float speed;
	public float deathTimer = 22.5f;
	void Start () 
	{

		StartCoroutine(DestroyObject(this.deathTimer));

		if(gameObject.transform.position.y > 0)
		{
			speed =-speed;
		}
	}

	private IEnumerator DestroyObject(float delay)
	{
		yield return new WaitForSeconds(delay);
		Destroy(this.gameObject);
	}

	void Update ()
	{
		this.transform.Translate (new Vector3(speed * Time.deltaTime, 0 ,0));    
	}

}
