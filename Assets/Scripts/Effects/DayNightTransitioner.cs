using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class DayNightTransitioner : MonoBehaviour {

	[SerializeField] Color[] colorTransitions;
	[SerializeField] float afternoonPercent = 0.2f;
	[SerializeField] float duskPercent = 0.8f;
	[SerializeField] float nightPercent = 0.9f;

	Image overlayImage;
	float gameTimer;
	float afternoonTime;
	float duskTime;
	float nightTime;

	void Awake () {

		overlayImage = this.GetComponent<Image>();
		gameTimer = 0;

		if(colorTransitions.Length > 0) {
			
			overlayImage.color = colorTransitions[0];
		}

		float gameLength = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().GetGameLength();
		afternoonTime = gameLength * afternoonPercent;
		duskTime = gameLength * duskPercent;
		nightTime = gameLength * nightPercent;
	}

	void Update () {

		gameTimer += Time.deltaTime;

		if(gameTimer > nightTime) {

			overlayImage.color = Color.Lerp(overlayImage.color, colorTransitions[3], 0.003f);
		}
		else if(gameTimer > duskTime) {

			overlayImage.color = Color.Lerp(overlayImage.color, colorTransitions[2], 0.002f);
		}
		else if(gameTimer > afternoonTime) {

			overlayImage.color = Color.Lerp(overlayImage.color, colorTransitions[1], 0.001f);
		}
	}
}
