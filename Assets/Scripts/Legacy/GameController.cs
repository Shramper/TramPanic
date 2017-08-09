using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class GameController : MonoBehaviour {

	[SerializeField] float gameLengthInSeconds;
    public float GameLength
    {
        get { return gameLengthInSeconds; }
        set { gameLengthInSeconds = value; }
    }

	[SerializeField] GameObject timerObject;
	[SerializeField] Streetcar streetcar;
	[SerializeField] Image leaderboardBackground;
	[SerializeField] LeaderboardController leaderboardController;
	bool is_Game_Started = false;
	float gameTimer;
    float delayTime = 5;

	void Start () {
        gameTimer = gameLengthInSeconds;
        leaderboardBackground.color = new Color(leaderboardBackground.color.r, leaderboardBackground.color.g, leaderboardBackground.color.b, 0);
	}

	void Update () {

		if(is_Game_Started && gameTimer > 0) {

			gameTimer -= Time.deltaTime;
			if(gameTimer < 10) {

				streetcar.ShowHurryUpText();
			}
		}
		else if(gameTimer < 0 && leaderboardBackground.color.a < 0.7f) {

			float newAlpha = leaderboardBackground.color.a + Time.deltaTime;
			leaderboardBackground.color = new Color(leaderboardBackground.color.r, leaderboardBackground.color.g, leaderboardBackground.color.b, newAlpha);

			if(leaderboardBackground.color.a >= 0.7f && is_Game_Started) {

				is_Game_Started = false;
				EndGame();
			}
		}
	}

	void EndGame () {

		leaderboardController.OpenLeaderboard();
	}
    public float GetGameLength()
    {
        return gameLengthInSeconds;
    }
    public float GetTimeRemaining () {

		return gameTimer;
	}
	public bool GameStarted () {

		return is_Game_Started;
	}
	public void StartGame () {

		is_Game_Started = true;
	}
}
