using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Role {

	Norm,
	Coin,
	Stink,
	Chunky,
	Inspector,
	Dazer,
	Officer,
	Raver,
	RoleCount
}

[DisallowMultipleComponent]
public class PedestrianSpawner : MonoBehaviour
{
	[Header("Role Percentages")]
	[SerializeField, Range(0, 1)] float coinPercentage = 0.20f;
	[SerializeField, Range(0, 1)] float stinkPercentage = 0.01f;
	[SerializeField, Range(0, 1)] float chunkyPercentage = 0.01f;
	[SerializeField, Range(0, 1)] float inspectorPercentage = 0.01f;
	[SerializeField, Range(0, 1)] float dazerPercentage = 0.01f;
	[SerializeField, Range(0, 1)] float officerPercentage = 0.01f;
	[SerializeField, Range(0, 1)] float raverPercentage = 0.01f;

	[Header("Pedestrian Sprites")]
	[SerializeField] Sprite[] pedestrianSprites;
	[SerializeField] Sprite[] chunkySprites;
	[SerializeField] Sprite[] inspectorSprites;
	[SerializeField] Sprite[] dazerSprites;
	[SerializeField] Sprite[] officerSprites;

	[Header("Parameters")]
	[SerializeField] int startingPedestriansOnSidewalk = 150;
	[SerializeField] int pedestrianSpawnRate = 1;

	[Header("Role Introduction Times")]
	[SerializeField] float coinStartPercentage;
	[SerializeField] float stinkStartPercentage;
	[SerializeField] float chunkyStartPercentage;
	[SerializeField] float inspectorStartPercentage;
	[SerializeField] float dazerStartPercentage;
	[SerializeField] float officerStartPercentage;
	[SerializeField] float raverStartPercentage;

    [Header("Role Introduction Texts")]
	[SerializeField, TextArea(1,2)] string coinIntroductionString;
	[SerializeField, TextArea(1,2)] string stinkIntroductionString;
	[SerializeField, TextArea(1,2)] string chunkyIntroductionString;
	[SerializeField, TextArea(1,2)] string inspectorIntroductionString;
	[SerializeField, TextArea(1,2)] string dazerIntroductionString;
	[SerializeField, TextArea(1,2)] string officerIntroductionString;
	[SerializeField, TextArea(1,2)] string raverIntroductionString;

	[Header("References")]
	[SerializeField] GameObject pedestrianPrefab;
	[SerializeField] Transform pedestrianContainer;
	[SerializeField] Transform opposingSpawnerTransform;
	[SerializeField] GameObject popupPanel;
	[SerializeField] GameObject[] streetcarStops;

    [Header("Sorting Layer References")]
    [SerializeField] Transform[] heightReferences;

	GameControllerV2 gameController;
	BoxCollider2D boxCollider;
	Vector3 leftEnd;
	Vector3 rightEnd;
	float gameTimer;
	float gameLength;
	float tempCoinPercentage;
	float tempStinkPercentage;
	float tempChunkyPercentage;
	float tempInspectorPercentage;
	float tempDazerPercentage;
	float tempOfficerPercentage;
	float tempRaverPercentage;
    bool tutorialShown = false;

	public string layerName;
	public int layerOrderShift = 0;

    private int pedestrianCount = 1;

	#region Initialization

	void Awake ()
    {
		InitializeVariables();
		InitializeSidewalkWithPedestrians();
		StartCoroutine(RecursiveSpawnNewPedestrian());
		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerV2>();
        gameLength = gameController.GetGameLength();

        // set pedestrian height references
        Pedestrian.heightReferences = heightReferences;
    }

	void InitializeVariables ()
    {
		boxCollider = this.GetComponent<BoxCollider2D>();
		leftEnd = new Vector3(boxCollider.bounds.min.x, this.transform.position.y, 0);
		rightEnd = new Vector3(boxCollider.bounds.max.x, this.transform.position.y, 0);

		tempCoinPercentage = coinPercentage;
		tempStinkPercentage = stinkPercentage;
		tempChunkyPercentage = chunkyPercentage;
		tempInspectorPercentage = inspectorPercentage;
		tempDazerPercentage = dazerPercentage;
		tempOfficerPercentage = officerPercentage;
		tempRaverPercentage = raverPercentage;

		gameTimer = 0;
		coinPercentage = 0;
		stinkPercentage = 0;
		chunkyPercentage = 0;
		inspectorPercentage = 0;
		dazerPercentage = 0;
		officerPercentage = 0;
		raverPercentage = 0;
	}

	void InitializeSidewalkWithPedestrians ()
    {
		for(int i = 0; i < startingPedestriansOnSidewalk; i++)
			CreateNewPedestrian();
	}

    #endregion
        
	#region Updates

	void Update ()
    {
		gameTimer += Time.deltaTime;
		CheckRoleIntroduction ();
		CreateNormalPedestrian();

		if(Input.GetKeyDown(KeyCode.Q)) {

			CreateSpecificRole(Role.Raver);
		}
		else if(Input.GetKeyDown(KeyCode.W)) {

			CreateSpecificRole(Role.Officer);
		}
		else if(Input.GetKeyDown(KeyCode.E)) {

			CreateSpecificRole(Role.Inspector);
		}
    }

	void CheckRoleIntroduction () {

		float percentageIntoGame = gameTimer / gameLength * 100;

		if(Mathf.Floor(percentageIntoGame) == 2 && !tutorialShown) {
            Debug.Log("show");
            popupPanel.GetComponent<Animator>().SetTrigger("Show");
            tutorialShown = true;
        }
		else if (percentageIntoGame > raverStartPercentage && raverPercentage == 0) {

			raverPercentage = tempRaverPercentage;
			popupPanel.GetComponent<Animator> ().SetTrigger ("Show");
			popupPanel.transform.Find("Person Image").GetComponent<Image> ().sprite = pedestrianSprites [Random.Range(0, pedestrianSprites.Length)];
			popupPanel.transform.Find("Person Image").GetComponent<UIColorStrobe>().StartCoroutine("RecursiveColorChange");
			popupPanel.transform.Find ("Icon Image").gameObject.SetActive(false);
			popupPanel.GetComponentInChildren<Text> ().text = raverIntroductionString.ToUpper();
			CreateSpecificRole (Role.Raver);
		}
		else if (percentageIntoGame > officerStartPercentage && officerPercentage == 0) {

			officerPercentage = tempOfficerPercentage;
			popupPanel.GetComponent<Animator> ().SetTrigger ("Show");
			popupPanel.transform.Find("Person Image").GetComponent<Image> ().sprite = officerSprites [Random.Range(0, officerSprites.Length)];
			popupPanel.transform.Find ("Icon Image").GetComponent<Animator> ().SetTrigger (Role.Officer.ToString());
			popupPanel.GetComponentInChildren<Text> ().text = officerIntroductionString.ToUpper();
			CreateSpecificRole (Role.Officer);
		}
		else if (percentageIntoGame > dazerStartPercentage && dazerPercentage == 0) {

			dazerPercentage = tempDazerPercentage;
			popupPanel.GetComponent<Animator> ().SetTrigger ("Show");
			popupPanel.transform.Find("Person Image").GetComponent<Image> ().sprite = dazerSprites [Random.Range(0, dazerSprites.Length)];
			popupPanel.transform.Find ("Icon Image").GetComponent<Animator> ().SetTrigger (Role.Dazer.ToString());
			popupPanel.GetComponentInChildren<Text> ().text = dazerIntroductionString.ToUpper();
			CreateSpecificRole (Role.Dazer);
		}
		else if (percentageIntoGame > inspectorStartPercentage && inspectorPercentage == 0) {

			inspectorPercentage = tempInspectorPercentage;
			popupPanel.GetComponent<Animator> ().SetTrigger ("Show");
			popupPanel.transform.Find("Person Image").GetComponent<Image> ().sprite = inspectorSprites [Random.Range(0, inspectorSprites.Length)];
			popupPanel.transform.Find ("Icon Image").GetComponent<Animator> ().SetTrigger (Role.Inspector.ToString());
			popupPanel.GetComponentInChildren<Text> ().text = inspectorIntroductionString.ToUpper();
			CreateSpecificRole (Role.Inspector);
		}
		else if (percentageIntoGame > chunkyStartPercentage && chunkyPercentage == 0) {

			chunkyPercentage = tempChunkyPercentage;
			popupPanel.GetComponent<Animator> ().SetTrigger ("Show");
			popupPanel.transform.Find("Person Image").GetComponent<Image> ().sprite = chunkySprites [Random.Range(0, chunkySprites.Length)];
			popupPanel.transform.Find ("Icon Image").GetComponent<Animator> ().SetTrigger (Role.Chunky.ToString());
			popupPanel.GetComponentInChildren<Text> ().text = chunkyIntroductionString.ToUpper();
			CreateSpecificRole (Role.Chunky);
		}
		else if (percentageIntoGame > stinkStartPercentage && stinkPercentage == 0) {

			stinkPercentage = tempStinkPercentage;
			popupPanel.GetComponent<Animator> ().SetTrigger ("Show");
			popupPanel.transform.Find("Person Image").GetComponent<Image> ().sprite = pedestrianSprites [0];
			popupPanel.transform.Find ("Icon Image").GetComponent<Animator> ().SetTrigger (Role.Stink.ToString());
			popupPanel.GetComponentInChildren<Text> ().text = stinkIntroductionString.ToUpper();
			CreateSpecificRole (Role.Stink);
		}
		else if (percentageIntoGame > coinStartPercentage && coinPercentage == 0) {

			coinPercentage = tempCoinPercentage;
			popupPanel.GetComponent<Animator> ().SetTrigger ("Show");
			popupPanel.transform.Find("Person Image").GetComponent<Image> ().sprite = pedestrianSprites [0];
			popupPanel.transform.Find ("Icon Image").GetComponent<Animator> ().SetTrigger (Role.Coin.ToString());
            popupPanel.transform.Find("Icon Image").GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
            popupPanel.GetComponentInChildren<Text> ().text = coinIntroductionString.ToUpper();
			CreateSpecificRole (Role.Coin);
		}
	}
	#endregion

	IEnumerator RecursiveSpawnNewPedestrian () {

		while(true) {
			CreateNewPedestrian();
			yield return new WaitForSeconds(pedestrianSpawnRate);
		}
	}

	void CreateNewPedestrian () {

		Vector3 randomPosition = new Vector3(Random.Range(leftEnd.x, rightEnd.x), this.transform.position.y, 0);
		GameObject newPedestrian = Instantiate(pedestrianPrefab, randomPosition, Quaternion.identity) as GameObject;
        newPedestrian.GetComponent<Pedestrian>().pedestrianID = pedestrianCount;
        pedestrianCount++;
		GetNewRole(newPedestrian);
		SetDestination(newPedestrian);
	}

	void CreateNormalPedestrian () {

		if(pedestrianContainer.childCount < 50) {

            // Randomize between spawning on the bottom or top sidewalk
            //Transform sidewalkTransform = Random.value < 0.5f ? transform : opposingSpawnerTransform;
            Transform sidewalkTransform = transform;
			float leftSide = sidewalkTransform.GetComponent<Collider2D>().bounds.min.x;
			float rightSide = sidewalkTransform.GetComponent<Collider2D>().bounds.max.x;
			Vector3 spawnPosition = new Vector3(Random.Range(leftSide, rightSide), sidewalkTransform.position.y, 0);

			// Initialize person
			GameObject newPedestrian = Instantiate(pedestrianPrefab, spawnPosition, Quaternion.identity) as GameObject;
			newPedestrian.GetComponent<SpriteRenderer>().sprite = pedestrianSprites[Random.Range(0, pedestrianSprites.Length)];
			newPedestrian.GetComponent<Pedestrian>().SetRole(Role.Norm);
			newPedestrian.GetComponent<Pedestrian>().SetDestination(new Vector3((Random.value < 0.5f ? leftSide : rightSide), sidewalkTransform.position.y, 0));
			newPedestrian.transform.SetParent(pedestrianContainer);
		}
	}

    void GetNewRole (GameObject pedestrian) {

		float randomValue = Random.Range(0, raverPercentage + officerPercentage + dazerPercentage + inspectorPercentage + chunkyPercentage + stinkPercentage + coinPercentage);
		Pedestrian pedestrianScript = pedestrian.GetComponent<Pedestrian>();

		if (randomValue < coinPercentage) {

			pedestrianScript.SetRole (Role.Coin);
			pedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = pedestrianSprites [Random.Range (0, pedestrianSprites.Length)];
		}
		else if (randomValue < stinkPercentage + coinPercentage) {

			pedestrianScript.SetRole (Role.Stink);
			pedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = pedestrianSprites [Random.Range (0, pedestrianSprites.Length)];
		}
		else if (randomValue < chunkyPercentage + stinkPercentage + coinPercentage) {

			pedestrianScript.SetRole (Role.Chunky);
			pedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = chunkySprites [Random.Range (0, chunkySprites.Length)];
		}
		else if (randomValue < inspectorPercentage + chunkyPercentage + stinkPercentage + coinPercentage) {

			pedestrianScript.SetRole (Role.Inspector);
			pedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = inspectorSprites [Random.Range (0, inspectorSprites.Length)];
		}
		else if (randomValue < dazerPercentage + inspectorPercentage + chunkyPercentage + stinkPercentage + coinPercentage) {

			pedestrianScript.SetRole (Role.Dazer);
			pedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = dazerSprites [Random.Range (0, dazerSprites.Length)];
		}
		else if (randomValue < officerPercentage + dazerPercentage + inspectorPercentage + chunkyPercentage + stinkPercentage + coinPercentage) {

			pedestrianScript.SetRole (Role.Officer);
			pedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = officerSprites [Random.Range (0, officerSprites.Length)];
		}
		else if (randomValue < raverPercentage + officerPercentage + dazerPercentage + inspectorPercentage + chunkyPercentage + stinkPercentage + coinPercentage) {

			pedestrianScript.SetRole (Role.Raver);
		}
		else {

			pedestrianScript.SetRole (Role.Norm);
			pedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = pedestrianSprites [Random.Range (0, pedestrianSprites.Length)];
		}

		if(pedestrianScript.GetRole() != Role.Norm) {

			pedestrianScript.SetMoveSpeed(2);
		}
	}

	void SetDestination (GameObject pedestrian) {

		Pedestrian pedestrianScript = pedestrian.GetComponent<Pedestrian>();
		pedestrianScript.SetMoveDelayTime(1f);

		if(pedestrianScript.GetRole() == Role.Norm)
        {
			Vector3 newDestination = (Random.value < 0.5f) ? leftEnd : rightEnd;
			pedestrianScript.SetDestination(newDestination);
			pedestrian.transform.SetParent(pedestrianContainer);
		}

		else if (pedestrianScript.GetRole() == Role.Coin)
        {
			// Either spawn to walk sidewalk or spawn in stop
			if(Random.value < 0.75)
            {
				Vector3 newDestination = (Random.value < 0.5f) ? leftEnd : rightEnd;
				pedestrianScript.SetDestination(newDestination);
				pedestrian.transform.SetParent(pedestrianContainer);
			}
			else
            {
				GameObject streetcarStop;

                //Find a stop that the streetcar is not currently stopped at.
				do
                {
					streetcarStop = streetcarStops[Random.Range(0, streetcarStops.Length)];
				} while(streetcarStop.GetComponent<StreetcarStop>().StreetcarStopped());

				pedestrian.transform.SetParent(streetcarStop.transform);
				Vector3 pedestrianPosition = streetcarStop.transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.3f, 0.3f), 0);
				pedestrian.transform.position = pedestrianPosition;
                chunkyPercentage *= (streetcarStop.transform.childCount - 1);
				streetcarStop.GetComponent<StreetcarStop>().UpdateMinimap();
                pedestrian.GetComponent<Pedestrian>().CheckIfBusStopPedestrian();
            }
		}

		else if(pedestrianScript.GetRole() == Role.Inspector || pedestrianScript.GetRole() == Role.Officer || pedestrianScript.GetRole() == Role.Raver)
        {
			GameObject streetcarStop;

			do {

				streetcarStop = streetcarStops[Random.Range(0, streetcarStops.Length)];

			} while(streetcarStop.GetComponent<StreetcarStop>().StreetcarStopped());

			// If stop already has role, change new person to a coin
			if(streetcarStop.GetComponent<StreetcarStop>().HasRole(pedestrianScript.GetRole())) {

				pedestrianScript.SetRole (Role.Coin);
				pedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = pedestrianSprites [Random.Range (0, pedestrianSprites.Length)];
			}

			pedestrian.transform.SetParent(streetcarStop.transform);
			Vector3 pedestrianPosition = streetcarStop.transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
			pedestrian.transform.position = pedestrianPosition;
			streetcarStop.GetComponent<StreetcarStop>().UpdateMinimap();
		}

		else
        {
			Vector3 newDestination = new Vector3(pedestrian.transform.position.x, opposingSpawnerTransform.position.y, 0);
			pedestrianScript.SetDestination(newDestination);
			pedestrian.transform.SetParent(pedestrianContainer);
		}
	}

	public void CreateSpecificRole (Role newRole) {

		if(transform.position.y > 0) {

			// Determine start and end positions for pedestrian
			float startY = 2.2f;
			float endY = -3f;

			// Create pedestrian
			Vector3 streetcarPosition = GameObject.FindGameObjectWithTag ("Streetcar").transform.position;
			Vector3 spawnPosition = new Vector3(streetcarPosition.x + 3, startY, 0);

			GameObject newPedestrian = Instantiate(pedestrianPrefab, spawnPosition, Quaternion.identity) as GameObject;
			newPedestrian.transform.SetParent(pedestrianContainer);

			// Set Role
			Pedestrian pedestrianScript = newPedestrian.GetComponent<Pedestrian> ();
			pedestrianScript.SetRole (newRole);
			pedestrianScript.SetMoveSpeed(1f);
			pedestrianScript.SetMoveDelayTime(1f);

			// Set Sprite
			switch (newRole) {
			case Role.Coin:
				newPedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = pedestrianSprites [0];
				break;
			case Role.Stink:
				newPedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = pedestrianSprites [Random.Range (0, pedestrianSprites.Length)];
				break;
			case Role.Chunky:
				newPedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = chunkySprites [Random.Range (0, chunkySprites.Length)];
				break;
			case Role.Inspector:
				newPedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = inspectorSprites [Random.Range (0, inspectorSprites.Length)];
				break;
			case Role.Dazer:
				newPedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = dazerSprites [Random.Range (0, dazerSprites.Length)];
				break;
			case Role.Officer:
				newPedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = officerSprites [Random.Range (0, officerSprites.Length)];
				break;
			case Role.Raver:
				newPedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = pedestrianSprites [Random.Range (0, pedestrianSprites.Length)];
				break;
			}

			// Set destination
			Vector3 newDestination = new Vector3(streetcarPosition.x + 3, endY, 0);
			pedestrianScript.SetDestination (newDestination);
		}
	}
}
