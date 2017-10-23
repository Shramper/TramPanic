using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[DisallowMultipleComponent]
public class StreetcarStopController : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] float pedestrianSpawnRate = 1;

	[Header("References")]
	[SerializeField] GameObject pedestrianPrefab;
	[SerializeField] Transform pedestrianContainer;
	[SerializeField] Sprite[] pedestrianSprites;

	// Private variables
	GameObject[] streetcarStops;


	#region Initialization
	void Awake () {
		
		InitializeVariables();
		StartCoroutine(RecursiveSpawnNewPedestrian());
	}

	void InitializeVariables () {

		streetcarStops = new GameObject[this.transform.childCount];
		for(int i = 0; i < streetcarStops.Length; i++) {

			streetcarStops[i] = this.transform.GetChild(i).gameObject;
		}
	}
	#endregion

	#region CreatingPedestrians
	void CreateNewPedestrian () {

		GameObject streetcarStop;

		// Only spawn a pedestrian at a streetcar stop if the streetcar isn't there
		do {

			streetcarStop = streetcarStops[Random.Range(0, streetcarStops.Length)];

		} while(streetcarStop.GetComponent<StreetcarStop>().StreetcarStopped());

        Vector3 spawnPosition = streetcarStop.transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
        //Vector3 spawnPosition = streetcarStop.transform.position + new Vector3(0, 0, 0);
        GameObject newPedestrian = Instantiate(pedestrianPrefab, spawnPosition, Quaternion.identity) as GameObject;
		newPedestrian.transform.SetParent(streetcarStop.transform);
		newPedestrian.GetComponent<Pedestrian>().SetRole (Role.Coin);
		newPedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = pedestrianSprites [Random.Range (0, pedestrianSprites.Length)];
	}

	IEnumerator RecursiveSpawnNewPedestrian () {

		while(true) {
			
			CreateNewPedestrian();
			yield return new WaitForSeconds(pedestrianSpawnRate);
		}
	}
	#endregion
}
