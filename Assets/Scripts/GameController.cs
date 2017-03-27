using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class GameController : MonoBehaviour {

	[SerializeField] float gameLengthInSeconds = 120;
	[SerializeField] Streetcar streetcar;

	bool is_Game_Started = false;
	float gameTimer;

	void Start () {

		gameTimer = gameLengthInSeconds;
		StartCoroutine (StartGameCountdown());
		PlayerPrefs.SetInt("FinalScore", 0);
	}

	void Update () {

		if(gameTimer > 0) {

			gameTimer -= Time.deltaTime;

			if(gameTimer < 10) {

				streetcar.ShowStreetcarCanvas();
			}
		}
		else if(gameTimer < 0) {

			PlayerPrefs.SetInt("FinalScore", Streetcar.score);
			SceneManager.LoadScene ("Leaderboard");
		}
	}

	IEnumerator StartGameCountdown() {

		is_Game_Started = false;

		yield return new WaitForSeconds (2);

		is_Game_Started = true;

	}

	public float GetGameLength () {

		return gameLengthInSeconds;
	}

	public bool GameStarted () {

		return is_Game_Started;
	}

	public void StartGame () {

		is_Game_Started = true;
	}
}
