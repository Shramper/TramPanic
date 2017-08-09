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

	SpriteRenderer overlaySpriteRenderer;
	float gameTimer;
	float afternoonTime;
	float duskTime;
	float nightTime;

	void Awake () {

		overlaySpriteRenderer = this.GetComponent<SpriteRenderer>();
		gameTimer = 0;

		if(colorTransitions.Length > 0) {
			
			overlaySpriteRenderer.color = colorTransitions[0];
		}

		float gameLength = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerV2>().GetGameLength();
		afternoonTime = gameLength * afternoonPercent;
		duskTime = gameLength * duskPercent;
		nightTime = gameLength * nightPercent;
	}

	void Update () {

		gameTimer += Time.deltaTime;

		if(gameTimer > nightTime) {

			overlaySpriteRenderer.color = Color.Lerp(overlaySpriteRenderer.color, colorTransitions[3], 0.003f);
		}
		else if(gameTimer > duskTime) {

			overlaySpriteRenderer.color = Color.Lerp(overlaySpriteRenderer.color, colorTransitions[2], 0.002f);
		}
		else if(gameTimer > afternoonTime) {

			overlaySpriteRenderer.color = Color.Lerp(overlaySpriteRenderer.color, colorTransitions[1], 0.001f);
		}
	}
}
