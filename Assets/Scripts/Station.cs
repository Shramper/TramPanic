using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class Station : MonoBehaviour {

	Streetcar streetcar;
	GameObject scorePanel;
	[SerializeField] int scoreToAdd = 10;


    private void Start()
    {
        foreach(GameObject thing in GameObject.FindGameObjectsWithTag("Streetcar"))
        { 
            streetcar = GameObject.FindGameObjectWithTag("Streetcar").GetComponent<Streetcar>();
            if (streetcar)
            {
                break;
            }
        }
        scorePanel = GameObject.FindGameObjectWithTag("ScorePanel");
    }

    void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Streetcar: " + streetcar);
        Debug.Log("ScorePanel: " + scorePanel);
		if(other.CompareTag("Fare")) {

			streetcar.AddToScore(scoreToAdd);
			Destroy(other.gameObject);
		}
		else if (other.CompareTag("Raver")) {

			streetcar.AddToScore(2 * scoreToAdd);
			Destroy(other.gameObject);
		}

		scorePanel.GetComponentInChildren<Text>().text = "Score:" + streetcar.GetScore().ToString("000");
	}
}
