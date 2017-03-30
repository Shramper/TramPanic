﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour {
	public GameObject Doors;
	public Animator doors;
	public string sceneName;
	private IEnumerator timer;



	IEnumerator timeDelay(float waitTime){
		yield return new WaitForSeconds (waitTime);
		SceneManager.LoadScene (sceneName);
	}



	public void TransitionToScene(){
		Doors.SetActive (true);
		doors.SetTrigger ("Open");

		timer = timeDelay(2);
		StartCoroutine (timer);
	}
}
