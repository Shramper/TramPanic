using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalScore : MonoBehaviour {

	public GUIText FScore;

	void Start(){
		FScore.text = "SCORE:" + Streetcar.score.ToString();
	}
}
