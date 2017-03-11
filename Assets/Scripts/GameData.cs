using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameData : MonoBehaviour {

	[SerializeField] float gameLengthInSeconds = 120;

	public bool is_Game_Started = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public float GetGameLength () {

		return gameLengthInSeconds;
	}
}
