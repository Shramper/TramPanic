using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour {
	public GameObject Doors;
	public Animator doors;
	public string sceneName;
	private IEnumerator timer;

	public bool doorsOpen; //If Doors are Open

	IEnumerator timeDelay(float waitTime){
		yield return new WaitForSeconds (waitTime);
		SceneManager.LoadScene (sceneName);
	}



	public void TransitionToScene(){

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

		timer = timeDelay(2);
		StartCoroutine (timer);
	}
}
