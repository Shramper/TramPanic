using UnityEngine;
using System.Collections;

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

	[Header("References")]
	[SerializeField] GameObject pedestrianPrefab;
	[SerializeField] Transform pedestrianContainer;
	[SerializeField] Sprite[] pedestrianSpriteArray;
	[SerializeField] Transform opposingSpawnerTransform;

	// Private variables
	BoxCollider2D boxCollider;
	Vector3 leftEnd;
	Vector3 rightEnd;


	void Awake () {
		
		InitializeComponents();
		InitializeVariables();
		InitializeSidewalkWithPedestrians();
		StartCoroutine(RecursiveSpawnNewPedestrian());
	}

	void InitializeComponents () {

		boxCollider = this.GetComponentInChildren<BoxCollider2D>();
		boxCollider.isTrigger = true;
	}

	void InitializeVariables () {

		leftEnd = new Vector3(boxCollider.bounds.min.x, this.transform.position.y, 0);
		rightEnd = new Vector3(boxCollider.bounds.max.x, this.transform.position.y, 0);
	}

	void InitializeSidewalkWithPedestrians () {
		
		for(int i = 0; i < startingPedestriansOnSidewalk; i++) {

			CreateNewPedestrian();
		}	
	}

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

		if(randomValue < normPercentage) {

			pedestrianScript.SetRole(Role.Norm);
			pedestrian.GetComponentInChildren<SpriteRenderer>().sprite = pedestrianSpriteArray[Random.Range(0, pedestrianSpriteArray.Length)];
		}
		else if(randomValue < coinPercentage + normPercentage) {

			pedestrianScript.SetRole(Role.Coin);
			pedestrian.GetComponentInChildren<SpriteRenderer>().sprite = pedestrianSpriteArray[Random.Range(0, pedestrianSpriteArray.Length)];
		}
		else if(randomValue < stinkPercentage + coinPercentage + normPercentage) {

			pedestrianScript.SetRole(Role.Stink);
		}
		else if(randomValue < chunkyPercentage + stinkPercentage + coinPercentage + normPercentage) {

			pedestrianScript.SetRole(Role.Chunky);
		}
		else if(randomValue < inspectorPercentage + chunkyPercentage + stinkPercentage + coinPercentage + normPercentage) {
			
			pedestrianScript.SetRole(Role.Inspector);
		}
		else if(randomValue < dazerPercentage + inspectorPercentage + chunkyPercentage + stinkPercentage + coinPercentage + normPercentage) {
			
			pedestrianScript.SetRole(Role.Dazer);
		}
		else if(randomValue < officerPercentage + dazerPercentage + inspectorPercentage + chunkyPercentage + stinkPercentage + coinPercentage + normPercentage) {
			
			pedestrianScript.SetRole(Role.Officer);
		}
		else if(randomValue < raverPercentage + officerPercentage + dazerPercentage + inspectorPercentage + chunkyPercentage + stinkPercentage + coinPercentage + normPercentage) {
			
			pedestrianScript.SetRole(Role.Raver);
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
}
