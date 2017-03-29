using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class StreetcarStop : MonoBehaviour {

	GameObject streetcarTimerCanvas;
	Image timerFill;
	bool streetcarStopped = false;


	void Awake () {

		streetcarTimerCanvas = this.transform.GetChild(0).gameObject;
		streetcarTimerCanvas.SetActive(false);
		timerFill = streetcarTimerCanvas.GetComponentInChildren<Image>();
	}

	void Update () {

		CheckForStreetcar();
		CheckIfFull();
	}

	void CheckForStreetcar () {
		
		if(streetcarStopped) {

			for(int i = 1; i < this.transform.childCount; i++) {
				
				Vector3 newDestination = this.transform.GetChild(i).position + Mathf.Sign(this.transform.position.y) * 2 * Vector3.down;
				this.transform.GetChild(i).GetComponent<Pedestrian>().SetDestination(newDestination);
				this.transform.GetChild(i).GetComponent<Pedestrian>().SetMoveSpeed(1.5f);
			}
		}
	}

	void CheckIfFull () {

		if(this.transform.childCount > 5) {

			streetcarTimerCanvas.SetActive(true);

			if(streetcarTimerCanvas.activeSelf) {

				timerFill.fillAmount -= 0.1f * Time.deltaTime;

				if(timerFill.fillAmount <= 0) {

					for(int i = 1; i < this.transform.childCount; i++) {

						Destroy(this.transform.GetChild(i).gameObject, 0.5f);
						//this.transform.GetChild(i).GetComponent<Pedestrian>().SetDestination(this.transform.position + 2 * Vector3.up);
					}
				}
			}
		}
		else if(this.transform.childCount < 11) {

			timerFill.fillAmount = 1f;
			streetcarTimerCanvas.SetActive(false);
		}
	}

	void OnTriggerStay2D(Collider2D other) {

		if(other.transform.name == "Streetcar") {

			float moveSpeed = other.GetComponent<Streetcar>().GetMoveSpeed();

			if(Mathf.Abs(moveSpeed) < 0.01f && !streetcarStopped) {

				streetcarStopped = true;
			}
		}
	}

	void OnTriggerExit2D(Collider2D other) {

		if(other.transform.CompareTag("Streetcar")) {

			streetcarStopped = false;
		}
	}

	public bool StreetcarStopped () {

		return streetcarStopped;
	}
}
