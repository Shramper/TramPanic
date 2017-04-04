using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameStartAnim : MonoBehaviour {
	
	public GameObject Doors;
	public Animator doors;
	public bool doorsOpen; //If Doors are Open

	[SerializeField] GameObject countdownTimer;

	void Awake(){
		
		startGame ();
	}

	public void startGame(){

		if (doorsOpen) {

			Debug.Log ("DOORS ARE OPEN");
			//doors.SetTrigger ("Open");
			Doors.SetActive (true);
			doors.Play("transitionClose");

		} else {

			Debug.Log ("DOORS ARE CLOSED");
			//doors.SetTrigger ("Close");
			Doors.SetActive (true);
			doors.Play ("transitionOpen");

		}

		StartCoroutine (timeDelay(1));
	}

	IEnumerator timeDelay(float waitTime){
		
		yield return new WaitForSeconds (waitTime);
		Doors.SetActive (false);
		countdownTimer.SetActive(true);
	}
}
