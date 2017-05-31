/*
using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(Streetcar))]
public class KeyboardInput : MonoBehaviour {

	[SerializeField] KeyCode moveLeft = KeyCode.A;
	[SerializeField] KeyCode moveRight = KeyCode.D;

	Streetcar streetcar;
    public int direction = 0;
	void Awake () {

		streetcar = this.GetComponentInChildren<Streetcar> ();
	}

	void Update () {
	
		if (SystemInfo.deviceType == DeviceType.Desktop) {

			if (Input.GetKey (moveLeft)) {

				streetcar.Decelerate ();
                direction = -1;
			}
			else if (Input.GetKey (moveRight)) {

				streetcar.Accelerate ();
                direction = 1;
			}

			if (Input.GetKeyUp (moveLeft) || Input.GetKeyUp (moveRight)) {

				streetcar.EndAcceleration ();
			}
		}
	}
}
*/