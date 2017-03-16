using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameData : MonoBehaviour {

	[SerializeField] float gameLengthInSeconds = 120;

	public bool is_Game_Started = false;
	//private Streetcar abilitiesList;
	//[SerializeField] Sprite[] abilitiesSprites;

	//public SpriteRenderer FirstAbilitySprite;

	// Use this for initialization
	void Start () 
	{
		//is_Game_Started = false;
	
		StartCoroutine (StartGameCountdown());
	}
	
	// Update is called once per frame
	void Update () 
	{	
		
	}

	public float GetGameLength () {

		return gameLengthInSeconds;
	}

	IEnumerator StartGameCountdown() {

		is_Game_Started = false;

		yield return new WaitForSeconds (2);

		is_Game_Started = true;

	}


}
