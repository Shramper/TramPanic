using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownTimer : MonoBehaviour {

	private GameData gameData;



	public void startGame()
	{	
		gameData = GameObject.Find ("GameManager").GetComponent<GameData>();
		gameData.is_Game_Started = true;
	}
}
