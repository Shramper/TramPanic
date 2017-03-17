using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(Streetcar))]
public class MobileInput : MonoBehaviour {

	Streetcar streetcar;

	void Awake () {

		streetcar = this.GetComponentInChildren<Streetcar> ();
	}

	void Update () {
	
		if (SystemInfo.deviceType == DeviceType.Handheld) {

			if (Input.touchCount > 0) {

				float xTouchPos = Input.GetTouch (0).position.x;

				if (xTouchPos < 0.25f * Screen.width) {

					streetcar.Decelerate ();
				}
				else if (xTouchPos > 0.75f * Screen.width) {

					streetcar.Accelerate ();
				}

				if (Input.GetTouch (0).phase == TouchPhase.Ended) {

					streetcar.EndAcceleration ();
				}
			}
		}
	}

	void OnMouseDown () {

		streetcar.abilityControls();
	}
}
