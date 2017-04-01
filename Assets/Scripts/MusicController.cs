using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour {

	[Header("Music Tracks")]
	[SerializeField] AudioClip regularMusic;
	[SerializeField] AudioClip raverMusic;

	AudioSource audioSource;


	void Awake () {

		audioSource = this.GetComponent<AudioSource>();
	}

	public void PlayRegularMusic () {

		if(audioSource.clip != regularMusic) {
			
			audioSource.clip = regularMusic;
			audioSource.Play();
		}
	}

	public void PlayRaverMusic () {

		if(audioSource.clip != raverMusic) {

			audioSource.clip = raverMusic;
			audioSource.Play();
		}
	}
}
