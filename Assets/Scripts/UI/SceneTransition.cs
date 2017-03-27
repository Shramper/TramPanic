using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour {

	public void TransitionToScene(string sceneName){
		Application.LoadLevel (sceneName);
	}
}
