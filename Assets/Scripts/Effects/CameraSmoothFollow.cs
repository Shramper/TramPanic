using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class CameraSmoothFollow : MonoBehaviour {

	[SerializeField] Transform cameraTarget;
	[SerializeField, Range(0f, 1f)] float cameraDelay = 0.3f;

	Vector3 cameraVelocity = Vector3.zero;

	void FixedUpdate () {

		if (cameraTarget)
		{
			Vector3 trackingTarget = new Vector3(cameraTarget.transform.position.x, this.transform.position.y, this.transform.position.z);

			this.transform.position = Vector3.SmoothDamp(this.transform.position, trackingTarget, ref cameraVelocity, cameraDelay);		
		}
	}
}
