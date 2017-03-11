using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour {

	public GUIText timerGT;

	//GUIText scoreP1GT;
	//GUIText scoreP2GT;

	StreetcarInput streetcarP1_script;
	StreetcarInput streetcarP2_script;

	public float scoreP1;
	public float scoreP2;

	public float stageTime = 5f;

	public CanvasGroup cgTimeUp;
	private bool timeUp = false;

	public GameObject[] minTimer;

	// Use this for initialization
	void Start () {
	
		GameObject timerGO = GameObject.Find ("Timer"); //Find Timer GameObject, call it timerGO
		timerGT = timerGO.GetComponent<GUIText> (); // Find the GUI Text in Game Object

		GameObject streetcarP1 = GameObject.Find ("Streetcar");
		streetcarP1_script = streetcarP1.GetComponent<StreetcarInput> ();
	}
	
	// Update is called once per frame
	void Update () {

		if(stageTime < 120f && stageTime > 60f){
			minTimer[2].active = false;
			minTimer[1].active = true;
		}
			
		if(stageTime < 59f){
			minTimer[1].active = false;
			minTimer[0].active = true;
		}
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

			timeUp = true;

			//cgTimeUp.alpha = 0; //Fill screen with white

			SceneManager.LoadScene ("Score");



		}

		/*if (timeUp) {

			cgTimeUp.alpha = cgTimeUp.alpha + Time.deltaTime; //Fade over time
			if (cgTimeUp.alpha <= 0) {
				cgTimeUp.alpha = 0;
				timeUp = false;
			}

		}*/
	}
}
