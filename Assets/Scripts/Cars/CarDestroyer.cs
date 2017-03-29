using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDestroyer : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other) {

		if (other.CompareTag ("Car") || other.CompareTag ("Taxi") || other.CompareTag ("Police")) {


			Destroy (other.gameObject);
		}
	}
}
