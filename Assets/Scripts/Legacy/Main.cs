using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour {

	public static float stageTime = 240f;

	public CanvasGroup cgTimeUp;
	//private bool timeUp = false;

	public GameObject[] minTimer;

	// Use this for initialization
	void Start () {
	
//		GameObject timerGO = GameObject.Find ("Timer"); //Find Timer GameObject, call it timerGO
//		timerGT = timerGO.GetComponent<GUIText> (); // Find the GUI Text in Game Object
//
//		GameObject streetcarP1 = GameObject.Find ("Streetcar");
//		streetcarP1_script = streetcarP1.GetComponent<StreetcarInput> ();
//
//		PlayerPrefs.SetInt("FinalScore", 0);
	}
	
	// Update is called once per frame
	void Update () {

		if (stageTime > 0) {

			stageTime -= Time.deltaTime;
			//timerGT.text = "" + Mathf.Round(stageTime);

		} 
		else if (stageTime < 0) {

			//NEED GAME OVER SCREEN
			stageTime = 0;

			//Debug.Log("P1 Score: " + streetcarP1_script.score);
			//Debug.Log("P2 Score: " + streetcarP2_script.score);
			//Debug.Log ("TIMES UP");

			//timeUp = true;

			//cgTimeUp.alpha = 0; //Fill screen with white

		}
	}
}
