using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class Station : MonoBehaviour {

	[SerializeField] GameObject scorePanel;
	[SerializeField] int scoreToAdd = 10;

	void OnTriggerEnter2D(Collider2D other) {

//		if(other.CompareTag("Fare")) {
//
//
//		}
	}
}
