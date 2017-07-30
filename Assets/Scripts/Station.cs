using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class Station : MonoBehaviour {

	Streetcar streetcar;
	Text scorePanel;
	[SerializeField] int scoreToAdd = 10;


    private void Start()
    {
        streetcar = GameObject.FindGameObjectWithTag("Streetcar").GetComponent<Streetcar>();
        scorePanel = GameObject.FindGameObjectWithTag("ScorePanel").GetComponentInChildren<Text>();

        //initVars();
    }

    /*
    private void initVars()
    {
        foreach (GameObject thing in GameObject.FindGameObjectsWithTag("Streetcar"))
        {
            streetcar = GameObject.FindGameObjectWithTag("Streetcar").GetComponent<Streetcar>();
            if (streetcar)
            {
                break;
            }
        }

        Debug.Log(gameObject.name + " found " + GameObject.FindGameObjectsWithTag("ScorePanel").Length + " scorePanels");
        scorePanel = GameObject.FindGameObjectWithTag("ScorePanel").GetComponentInChildren<Text>();
        Debug.Log(scorePanel);
    }
    */

    void OnTriggerEnter2D(Collider2D other)
    {
        /*
        if (!streetcar || !scorePanel)
        {
            initVars();
        }
        */

		if(other.CompareTag("Fare"))
        {
			streetcar.AddToScore(scoreToAdd);
			Destroy(other.gameObject);
		}
		else if (other.CompareTag("Raver"))
        {
			streetcar.AddToScore(2 * scoreToAdd);
			Destroy(other.gameObject);
		}

        Debug.Log(gameObject.name + " references " + scorePanel.text);
        Debug.Log("Trying to add to: " + streetcar.GetScore().ToString("000"));
		scorePanel.text = "Score:" + streetcar.GetScore().ToString("000");
	}
}
