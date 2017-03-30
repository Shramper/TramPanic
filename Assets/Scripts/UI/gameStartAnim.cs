using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameStartAnim : MonoBehaviour {
	public GameObject Doors;
	public Animator doors;
	private IEnumerator timer;

	void Awake(){
		startGame ();
	}

	IEnumerator timeDelay(float waitTime){
		yield return new WaitForSeconds (waitTime);
		Doors.SetActive (false);
	}



	public void startGame(){
		Doors.SetActive (true);
		doors.SetTrigger ("Close");

		timer = timeDelay(3);
		StartCoroutine (timer);

	}
}
