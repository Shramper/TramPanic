﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class LeaderboardController : MonoBehaviour {

	const int leaderboardEntryCount = 10;

	[SerializeField] Text finalScoreText;
	[SerializeField] GameObject nameEntryPanel;
	[SerializeField] Text[] scoreTextArray;
	[SerializeField] Text[] nameTextArray;
	[SerializeField] Text[] timeTextArray;

	int[] scoreArray = new int[leaderboardEntryCount];
	string[] nameArray = new string[leaderboardEntryCount];

	int finalScore = 0;
	int indexOfNewHighScore;
	string newName;

	void Start () {

		FillLeaderboardTest();
		//SaveToPlayerPrefs();
		LoadPlayerPrefs();
		UpdateLeaderboardText();

		finalScore = PlayerPrefs.GetInt("FinalScore");
		finalScoreText.text = "Final Score\n" + finalScore;

		nameEntryPanel.SetActive(false);
		CheckIfNewHighScore();
	}

	void Update () {

		if(nameEntryPanel.activeSelf == false) {

			if((SystemInfo.deviceType == DeviceType.Desktop && Input.anyKeyDown) ||
				(SystemInfo.deviceType == DeviceType.Handheld && Input.touchCount > 0)) {

				SceneManager.LoadScene(0);
			}
		}
	}

	void CheckIfNewHighScore () {

		int lowestLeaderboardScore = scoreArray[leaderboardEntryCount - 1];
		Debug.Log("lowestLeaderboardScore: " + lowestLeaderboardScore);

		if(finalScore > lowestLeaderboardScore) {
			Debug.Log("Addine new high score");
			nameEntryPanel.SetActive(true);
			AddNewHighScore();
		}
	}

	void AddNewHighScore () {

		indexOfNewHighScore = -1;

		// Cycle through scores to see where the new highscore should go
		if(finalScore > scoreArray[0]) {

			indexOfNewHighScore = 0;
		}
		else {
			
			for(int i = leaderboardEntryCount - 1; i > 1; i--) {
				Debug.Log("Checking " + i);
				if(finalScore > scoreArray[i] && finalScore <= scoreArray[i - 1]) {

					indexOfNewHighScore = i;
					break;
				}
			}
		}

		// Move all entries down one to make space for new entry
		for(int i = leaderboardEntryCount - 1; i > indexOfNewHighScore; i--) {

			scoreArray[i] = scoreArray[i - 1];
			nameArray[i] = nameArray[i - 1];
		}

		// Enter new highscore data
		scoreArray[indexOfNewHighScore] = finalScore;
		nameArray[indexOfNewHighScore] = newName;

		SaveToPlayerPrefs();
		UpdateLeaderboardText();
	}

	void FillLeaderboardTest () {

		for(int i = 0; i < leaderboardEntryCount; i++) {

			scoreArray[i] = 20 - (2 * i);
			nameArray[i] = i.ToString();
		}
	}

	void LoadPlayerPrefs () {

		for(int i = 0; i < leaderboardEntryCount; i++) {

			scoreArray[i] = PlayerPrefs.GetInt("LeaderboardScore" + i);
			nameArray[i] = PlayerPrefs.GetString("LeaderboardName" + i);
		}
	}

	void SaveToPlayerPrefs () {

		for(int i = 0; i < leaderboardEntryCount; i++) {

			PlayerPrefs.SetInt("LeaderboardScore" + i, scoreArray[i]);
			PlayerPrefs.SetString("LeaderboardName" + i, nameArray[i]);
		}
	}

	void UpdateLeaderboardText () {

		for(int i = 0; i < leaderboardEntryCount; i++) {

			scoreTextArray[i].text = scoreArray[i].ToString();
			nameTextArray[i].text = nameArray[i];
		}
	}

	public void SaveNewName () {

		newName = nameEntryPanel.GetComponentInChildren<InputField>().text;

		float randomValue = Random.value;
		if(randomValue < 0.25f) { newName += " North"; }
		else if(randomValue < 0.5f) { newName += " South"; }
		else if(randomValue < 0.75f) { newName += " East"; }
		else { newName += " West"; }

		if(indexOfNewHighScore != -1) { nameArray[indexOfNewHighScore] = newName; }
		nameEntryPanel.SetActive(false);
		UpdateLeaderboardText();
		SaveToPlayerPrefs();
	}

	public void ResetLeaderboard () {

		scoreArray = new int[leaderboardEntryCount];
		nameArray = new string[leaderboardEntryCount];

		for(int i = 0; i < leaderboardEntryCount; i++) {

			scoreArray[i] = 0;
			nameArray[i] = "";
		}

		SaveToPlayerPrefs();
	}
}