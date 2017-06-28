using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour {
	public GameObject Doors;
	public Animator doors;
	public string sceneName;
	public bool doorsOpen; 

	IEnumerator SceneChange()
    {
		yield return new WaitForSeconds(2);
		SceneManager.LoadScene (sceneName);
	}

	public void TransitionToScene()
    {
		if (doorsOpen)
        {
			Doors.SetActive (true);
			doors.Play("transitionClose");
		}
        else
        {
			Doors.SetActive (true);
			doors.Play ("transitionOpen");
		}
		StartCoroutine (SceneChange());
	}
}
