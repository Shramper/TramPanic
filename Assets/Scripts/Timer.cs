using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

	public Sprite[] seconds;
	public Sprite[] tens;
	public Sprite[] minutes;

	float timer = Main.stageTime;

	public float secTimer = 10;
	public float tensTimer = 0;
	public float minTimer = 60;

	public GameObject secs;
	public GameObject mins;
	public GameObject tenths;

	public int tenCount = 6;
	public int minCount = 4;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		int secInt = Mathf.FloorToInt (secTimer);
		int tenInt = Mathf.FloorToInt (tensTimer);
		int minInt = Mathf.FloorToInt (minTimer);

		timer -= Time.deltaTime;
		minTimer -= Time.deltaTime;
		tensTimer -= Time.deltaTime;
		secTimer -= Time.deltaTime;


		if (secTimer <= 0) {
			secTimer = 10;
		}
		if(secTimer >= 0){
			secs.transform.GetComponent<Image> ().sprite = seconds [secInt];
		}


		if (tensTimer <= 0) {
			tenCount -= 1;
			tenths.transform.GetComponent<Image> ().sprite = tens [tenCount];
			tensTimer = 10;
			if (tenCount <= 0) {
				tenCount = 6;
			}
		}

		if (minTimer <= 0) {
			minCount -= 1;
			mins.transform.GetComponent<Image> ().sprite = minutes [minCount];
			minTimer = 60;
		}
			
	}
}
	