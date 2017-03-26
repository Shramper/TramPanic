using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PedestrianSpawner : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] int startingPedestriansOnSidewalk = 150;
	[SerializeField] int pedestrianSpawnRate = 1;

	[Header("References")]
	[SerializeField] StreetcarStopController streetcarStopController;
	[SerializeField] GameObject pedestrianPrefab;
	[SerializeField] Sprite[] pedestrianSprites;
	[SerializeField] Transform pedestrianContainer;

	BoxCollider2D boxCollider;
	Vector3 leftEnd;
	Vector3 rightEnd;


	void Awake () {

		InitializeVariables();
		InitializeSidewalkWithPedestrians();
		StartCoroutine(RecursiveSpawnNewPedestrian());
	}

	void InitializeVariables () {

		boxCollider = this.GetComponent<BoxCollider2D>();
		leftEnd = new Vector3(boxCollider.bounds.min.x, this.transform.position.y, 0);
		rightEnd = new Vector3(boxCollider.bounds.max.x, this.transform.position.y, 0);
	}

	void InitializeSidewalkWithPedestrians () {

		for(int i = 0; i < startingPedestriansOnSidewalk; i++) {

			CreateNewPedestrian();
		}	
	}

	IEnumerator RecursiveSpawnNewPedestrian () {

		while(true) {

			CreateNewPedestrian();
			yield return new WaitForSeconds(pedestrianSpawnRate);
		}
	}

	void CreateNewPedestrian () {

		Vector3 randomPosition = new Vector3(Random.Range(leftEnd.x, rightEnd.x), Random.Range(boxCollider.bounds.min.y, boxCollider.bounds.max.y), 0);
		GameObject newPedestrian = Instantiate(pedestrianPrefab, randomPosition, Quaternion.identity) as GameObject;
		Pedestrian pedestrianScript = newPedestrian.GetComponent<Pedestrian>();
		newPedestrian.transform.SetParent(pedestrianContainer);

		float roleValue = Random.value;

		if(roleValue < 0.02f && streetcarStopController.GetRolePercentage(Role.Dazer) > 0) {

			streetcarStopController.CreateSpecificRole(Role.Dazer);
		}
		else if(roleValue < 0.04f && streetcarStopController.GetRolePercentage(Role.Raver) > 0) {

			streetcarStopController.CreateSpecificRole(Role.Raver);
		}
		else {
			
			pedestrianScript.SetRole (Role.Norm);
			newPedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = pedestrianSprites [Random.Range (0, pedestrianSprites.Length)];

			Vector3 newDestination = (Random.value < 0.5f) ? leftEnd : rightEnd;
			newPedestrian.GetComponent<Pedestrian>().SetDestination(newDestination);
		}
	}
}
