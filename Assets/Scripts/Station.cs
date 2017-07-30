using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class Station : MonoBehaviour
{
	Streetcar streetcar;
    GameControllerV2 gameController;
	Text scorePanel;
	[SerializeField] int scoreToAdd = 10;

    private void Start()
    {
        streetcar = GameObject.FindGameObjectWithTag("Streetcar").GetComponent<Streetcar>();
        scorePanel = GameObject.FindGameObjectWithTag("ScorePanel").GetComponentInChildren<Text>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerV2>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
		if(other.CompareTag("Fare"))
        {
			gameController.IncrementScore(scoreToAdd);
			Destroy(other.gameObject);
		}
		else if (other.CompareTag("Raver"))
        {
            gameController.IncrementScore(2 * scoreToAdd);
			Destroy(other.gameObject);
		}

		scorePanel.text = "Score:" + gameController.GetScore().ToString("000");
	}
}
