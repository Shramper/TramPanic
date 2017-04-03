using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownTimer : MonoBehaviour {

	private GameController gameController;

	public AudioClip clip3;
	public AudioClip clip2;
	public AudioClip clip1;
	//public AudioClip clipGO;

	public AudioSource audioCountdown;
	public GameObject countdownImage;

	[SerializeField] MusicController musicController;

	public void Start() {

		audioCountdown = this.GetComponent<AudioSource>();
		countdownImage = GameObject.Find ("Countdown");
	}

	public void startGame()
	{	
		gameController = GameObject.Find ("Game Controller").GetComponent<GameController>();
	}

	public void countdown3() {

		Debug.Log ("3");
		audioCountdown.clip = clip3;
		audioCountdown.Play ();
	}

	public void countdown2() {

		Debug.Log ("2");
		audioCountdown.clip = clip2;
		audioCountdown.Play ();
	}

	public void countdown1() {

		Debug.Log ("1");
		audioCountdown.clip = clip1;
		audioCountdown.Play ();
	}

	public void countdownStart() {

		Debug.Log ("GO");
		musicController.PlayRegularMusic();
		gameController.StartGame();
	}
}
