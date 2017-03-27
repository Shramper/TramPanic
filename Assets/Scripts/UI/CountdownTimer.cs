using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownTimer : MonoBehaviour {

	private GameController gameData;


	public void startGame()
	{	
		gameData = GameObject.Find ("GameManager").GetComponent<GameController>();
		gameData.StartGame();
	}
}
