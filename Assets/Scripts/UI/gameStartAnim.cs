using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameStartAnim : MonoBehaviour {
	
	public GameObject Doors;
	public Animator doors;

	[SerializeField] GameObject countdownTimer;

	void Awake(){
		
		startGame ();
	}

	public void startGame(){
		
		Doors.SetActive (true);
		doors.SetTrigger ("Close");

		StartCoroutine (timeDelay(1));
	}

	IEnumerator timeDelay(float waitTime){
		
		yield return new WaitForSeconds (waitTime);
		Doors.SetActive (false);
		countdownTimer.SetActive(true);
	}
}
