using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

	public Sprite[] seconds;
	public Sprite[] tens;
	public Sprite[] minutes;

	float timer;
	float delayTimer = 4;

	public float secTimer = 9;
	public float tensTimer = 0;
	public float minTimer = 0;

	public GameObject secs;
	public GameObject mins;
	public GameObject tenths;
    public GameObject colon;

	public int tenCount = 6;
	public int minCount = 4;

    bool isBlinking = false;

	public GameObject TimerUI;

	void Awake () {

		timer = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().GetGameLength();
		delayTimer = 5;
	}

	void Update () {

		if(timer > 0) {
			
			delayTimer -= Time.deltaTime;

			if (delayTimer <= 0) {
				delayTimer = 0;
			}
			
			int secInt = Mathf.FloorToInt (secTimer);
			int tenInt = Mathf.FloorToInt (tensTimer);
			int minInt = Mathf.FloorToInt (minTimer);

			if (delayTimer <= 0) {
				timer -= Time.deltaTime;
				minTimer -= Time.deltaTime;
				tensTimer -= Time.deltaTime;
				secTimer -= Time.deltaTime;
			}

			if (secTimer <= 0) {
				secTimer = 10;
			}

			if(secTimer >= 0 && secInt < 10){
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

	        if(timer <= 10 && !isBlinking) {

	            isBlinking = true;
	            StartCoroutine(BlinkTime());
	        }
		}
	}

    IEnumerator BlinkTime () {

        secs.SetActive(!secs.activeSelf);
        mins.SetActive(!mins.activeSelf);
        tenths.SetActive(!tenths.activeSelf);
        colon.SetActive(!colon.activeSelf);

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(BlinkTime());
    }
}
	