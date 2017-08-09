using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalScore : MonoBehaviour
{
    GameControllerV2 gameController;
	public GUIText FScore;

	void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerV2>();
        FScore.text = "SCORE:" + gameController.GetScore().ToString();
	}
}
