using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class CameraEffects : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField, Range(0,0.5f)] float maxXAxisShake = 0.1f;
	[SerializeField, Range(0,0.5f)] float maxYAxisShake = 0.1f;
	[SerializeField, Range(0,0.5f)] float shakeDuration = 0.25f;

	float shakeTimer = 0;
	Vector3 startPosition;


	void Update () {

		if (shakeTimer > 0) {
			
			this.transform.position = new Vector3 (this.transform.position.x + Random.Range (-maxXAxisShake, maxXAxisShake), 
				this.transform.position.y + Random.Range (-maxYAxisShake, maxYAxisShake), 
				this.transform.position.z);

			shakeTimer -= Time.deltaTime;

			// Check if need to reset camera position
			if(shakeTimer <= 0) { this.transform.position = startPosition; }
		}
	}

	public void ShakeCamera () {

		startPosition = this.transform.position;
		shakeTimer = shakeDuration;
	}

	public void ShakeCamera(float newShakeDuration) {

		startPosition = this.transform.position;
		shakeTimer = newShakeDuration;
	}
}
