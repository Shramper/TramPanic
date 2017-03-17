using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// TODO:
// - Spawn rate checks need to be flexible since roles are introduced at specific times now

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
[RequireComponent(typeof(BoxCollider2D))]
public class PedestrianSpawner : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] int startingPedestriansOnSidewalk = 10;
	[SerializeField] float pedestrianSpawnRate = 1;

	[Header("Role Percentages")]
	[SerializeField, Range(0, 1)] float normPercentage = 0.70f;
	[SerializeField, Range(0, 1)] float coinPercentage = 0.20f;
	[SerializeField, Range(0, 1)] float stinkPercentage = 0.01f;
	[SerializeField, Range(0, 1)] float chunkyPercentage = 0.01f;
	[SerializeField, Range(0, 1)] float inspectorPercentage = 0.01f;
	[SerializeField, Range(0, 1)] float dazerPercentage = 0.01f;
	[SerializeField, Range(0, 1)] float officerPercentage = 0.01f;
	[SerializeField, Range(0, 1)] float raverPercentage = 0.01f;

	[Header("Role Sprites")]
	[SerializeField] Sprite[] pedestrianSprites;
	[SerializeField] Sprite[] chunkySprites;
	[SerializeField] Sprite[] inspectorSprites;
	[SerializeField] Sprite[] dazerSprites;
	[SerializeField] Sprite[] officerSprites;

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

	// Private variables
	private GameData gameData;
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

	#region Initialization
	void Awake () {
		
		InitializeComponents();
		InitializeVariables();
		InitializeSidewalkWithPedestrians();
		StartCoroutine(RecursiveSpawnNewPedestrian());
		gameData = GameObject.Find ("GameManager").GetComponent<GameData>();
		gameLength = gameData.GetGameLength ();
	}

	void InitializeComponents () {

		boxCollider = this.GetComponentInChildren<BoxCollider2D>();
		boxCollider.isTrigger = true;
	}

	void InitializeVariables () {

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

	void InitializeSidewalkWithPedestrians () {
		
		for(int i = 0; i < startingPedestriansOnSidewalk; i++) {

			CreateNewPedestrian();
		}	
	}
	#endregion

	#region Updates
	void Update () {

		gameTimer += Time.deltaTime;

		CheckRoleIntroduction ();
	}

	void CheckRoleIntroduction () {

		float percentageIntoGame = gameTimer / gameLength * 100;

		if (percentageIntoGame > raverStartPercentage && raverPercentage == 0) {

			raverPercentage = tempRaverPercentage;
			popupPanel.GetComponent<Animator> ().SetTrigger ("Show");
			popupPanel.transform.FindChild("Person Image").GetComponent<Image> ().sprite = pedestrianSprites [0];
			popupPanel.transform.FindChild ("Icon Image").GetComponent<Animator> ().SetTrigger (Role.Raver.ToString());
			popupPanel.GetComponentInChildren<Text> ().text = raverIntroductionString.ToUpper();
			CreateSpecificRole (Role.Raver);
		}
		else if (percentageIntoGame > officerStartPercentage && officerPercentage == 0) {

			officerPercentage = tempOfficerPercentage;
			popupPanel.GetComponent<Animator> ().SetTrigger ("Show");
			popupPanel.transform.FindChild("Person Image").GetComponent<Image> ().sprite = officerSprites [Random.Range(0, officerSprites.Length)];
			popupPanel.transform.FindChild ("Icon Image").GetComponent<Animator> ().SetTrigger (Role.Officer.ToString());
			popupPanel.GetComponentInChildren<Text> ().text = officerIntroductionString.ToUpper();
			CreateSpecificRole (Role.Officer);
		}
		else if (percentageIntoGame > dazerStartPercentage && dazerPercentage == 0) {

			dazerPercentage = tempDazerPercentage;
			popupPanel.GetComponent<Animator> ().SetTrigger ("Show");
			popupPanel.transform.FindChild("Person Image").GetComponent<Image> ().sprite = dazerSprites [Random.Range(0, dazerSprites.Length)];
			popupPanel.transform.FindChild ("Icon Image").GetComponent<Animator> ().SetTrigger (Role.Dazer.ToString());
			popupPanel.GetComponentInChildren<Text> ().text = dazerIntroductionString.ToUpper();
			CreateSpecificRole (Role.Dazer);
		}
		else if (percentageIntoGame > inspectorStartPercentage && inspectorPercentage == 0) {

			inspectorPercentage = tempInspectorPercentage;
			popupPanel.GetComponent<Animator> ().SetTrigger ("Show");
			popupPanel.transform.FindChild("Person Image").GetComponent<Image> ().sprite = inspectorSprites [Random.Range(0, inspectorSprites.Length)];
			popupPanel.transform.FindChild ("Icon Image").GetComponent<Animator> ().SetTrigger (Role.Inspector.ToString());
			popupPanel.GetComponentInChildren<Text> ().text = inspectorIntroductionString.ToUpper();
			CreateSpecificRole (Role.Inspector);
		}
		else if (percentageIntoGame > chunkyStartPercentage && chunkyPercentage == 0) {

			chunkyPercentage = tempChunkyPercentage;
			popupPanel.GetComponent<Animator> ().SetTrigger ("Show");
			popupPanel.transform.FindChild("Person Image").GetComponent<Image> ().sprite = chunkySprites [Random.Range(0, chunkySprites.Length)];
			popupPanel.transform.FindChild ("Icon Image").GetComponent<Animator> ().SetTrigger (Role.Chunky.ToString());
			popupPanel.GetComponentInChildren<Text> ().text = chunkyIntroductionString.ToUpper();
			CreateSpecificRole (Role.Chunky);
		}
		else if (percentageIntoGame > stinkStartPercentage && stinkPercentage == 0) {

			stinkPercentage = tempStinkPercentage;
			popupPanel.GetComponent<Animator> ().SetTrigger ("Show");
			popupPanel.transform.FindChild("Person Image").GetComponent<Image> ().sprite = pedestrianSprites [0];
			popupPanel.transform.FindChild ("Icon Image").GetComponent<Animator> ().SetTrigger (Role.Stink.ToString());
			popupPanel.GetComponentInChildren<Text> ().text = stinkIntroductionString.ToUpper();
			CreateSpecificRole (Role.Stink);
		}
		else if (percentageIntoGame > coinStartPercentage && coinPercentage == 0) {

			coinPercentage = tempCoinPercentage;
			popupPanel.GetComponent<Animator> ().SetTrigger ("Show");
			popupPanel.transform.FindChild("Person Image").GetComponent<Image> ().sprite = pedestrianSprites [0];
			popupPanel.transform.FindChild ("Icon Image").GetComponent<Animator> ().SetTrigger (Role.Coin.ToString());
			popupPanel.GetComponentInChildren<Text> ().text = coinIntroductionString.ToUpper();
			CreateSpecificRole (Role.Coin);
		}
	}

	#endregion

	#region CreatingPedestrians
	void CreateNewPedestrian () {

		Vector3 randomPosition = new Vector3(Random.Range(boxCollider.bounds.min.x, boxCollider.bounds.max.x), Random.Range(boxCollider.bounds.min.y, boxCollider.bounds.max.y), 0);
		GameObject newPedestrian = Instantiate(pedestrianPrefab, randomPosition, Quaternion.identity) as GameObject;
		newPedestrian.transform.SetParent(pedestrianContainer);
		GetNewRole(newPedestrian);
		SetDestination(newPedestrian);
	}

	void GetNewRole (GameObject pedestrian) {

		float randomValue = Random.value;
		Pedestrian pedestrianScript = pedestrian.GetComponent<Pedestrian>();

		if (randomValue < normPercentage) {

			pedestrianScript.SetRole (Role.Norm);
			pedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = pedestrianSprites [Random.Range (0, pedestrianSprites.Length)];
		}
		else if (randomValue < coinPercentage + normPercentage) {

			pedestrianScript.SetRole (Role.Coin);
			pedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = pedestrianSprites [Random.Range (0, pedestrianSprites.Length)];
		}
		else if (randomValue < stinkPercentage + coinPercentage + normPercentage) {

			pedestrianScript.SetRole (Role.Stink);
			pedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = pedestrianSprites [Random.Range (0, pedestrianSprites.Length)];
		}
		else if (randomValue < chunkyPercentage + stinkPercentage + coinPercentage + normPercentage) {

			pedestrianScript.SetRole (Role.Chunky);
			pedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = chunkySprites [Random.Range (0, chunkySprites.Length)];
		}
		else if (randomValue < inspectorPercentage + chunkyPercentage + stinkPercentage + coinPercentage + normPercentage) {
			
			pedestrianScript.SetRole (Role.Inspector);
			pedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = inspectorSprites [Random.Range (0, inspectorSprites.Length)];
		}
		else if (randomValue < dazerPercentage + inspectorPercentage + chunkyPercentage + stinkPercentage + coinPercentage + normPercentage) {
			
			pedestrianScript.SetRole (Role.Dazer);
			pedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = dazerSprites [Random.Range (0, dazerSprites.Length)];
		}
		else if (randomValue < officerPercentage + dazerPercentage + inspectorPercentage + chunkyPercentage + stinkPercentage + coinPercentage + normPercentage) {
			
			pedestrianScript.SetRole (Role.Officer);
			pedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = officerSprites [Random.Range (0, officerSprites.Length)];
		}
		else if (randomValue < raverPercentage + officerPercentage + dazerPercentage + inspectorPercentage + chunkyPercentage + stinkPercentage + coinPercentage + normPercentage) {
			
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

		if(pedestrianScript.GetRole() == Role.Norm || pedestrianScript.GetRole() == Role.Coin) {

			Vector3 newDestination = (Random.value < 0.5f) ? leftEnd : rightEnd;
			pedestrianScript.SetDestination(newDestination);
		}
		else {

			Vector3 newDestination = new Vector3(pedestrian.transform.position.x, opposingSpawnerTransform.position.y, 0);
			pedestrianScript.SetDestination(newDestination);
		}
	}

	IEnumerator RecursiveSpawnNewPedestrian () {

		CreateNewPedestrian();

		yield return new WaitForSeconds(pedestrianSpawnRate);

		StartCoroutine(RecursiveSpawnNewPedestrian());
	}

	public void CreateSpecificRole (Role newRole) {

		// Create pedestrian
		Vector3 streetcarPosition = GameObject.FindGameObjectWithTag ("Streetcar").transform.position;
		Vector3 spawnPosition = new Vector3(streetcarPosition.x + 3, this.transform.position.y, 0);

		GameObject newPedestrian = Instantiate(pedestrianPrefab, spawnPosition, Quaternion.identity) as GameObject;
		newPedestrian.transform.SetParent(pedestrianContainer);

		// Set Role
		Pedestrian pedestrianScript = newPedestrian.GetComponent<Pedestrian> ();
		pedestrianScript.SetRole (newRole);
		pedestrianScript.SetMoveSpeed(1f);

		// Set destination
		Vector3 newDestination = new Vector3(streetcarPosition.x + 3, opposingSpawnerTransform.position.y, 0);
		pedestrianScript.SetDestination (newDestination);
	}
	#endregion
}
