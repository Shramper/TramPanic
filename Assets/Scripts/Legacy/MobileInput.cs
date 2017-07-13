/*
using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(Streetcar))]
public class MobileInput : MonoBehaviour {

	Streetcar streetcar;
	bool accelerating = false;
	bool decelerating = false;

	void Awake () {

		streetcar = this.GetComponentInChildren<Streetcar> ();
	}

	void Update () {
	
		if(accelerating && !decelerating) {

			streetcar.Accelerate();
		}
		else if(decelerating && !accelerating) {

			streetcar.Decelerate();
		}
	}

	public void StartAccelerating () {

		accelerating = true;
	}

	public void EndAccelerating () {

		accelerating = false;
		streetcar.EndAcceleration();
	}

	public void StartDecelerating () {

		decelerating = true;
	}

	public void EndDecelerating () {

		decelerating = false;
		streetcar.EndAcceleration();
	}
}
*/