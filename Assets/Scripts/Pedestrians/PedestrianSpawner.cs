using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Role
{
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
	[SerializeField, Range(0, 1)] float coinPercentage;
	[SerializeField, Range(0, 1)] float stinkPercentage;
	[SerializeField, Range(0, 1)] float chunkyPercentage;
	[SerializeField, Range(0, 1)] float inspectorPercentage;
	[SerializeField, Range(0, 1)] float officerPercentage;
    [SerializeField, Range(0, 1)] float dazerPercentage;
	[SerializeField, Range(0, 1)] float raverPercentage;

    [Header("Pedestrian Sprites")]
	public Sprite[] pedestrianSprites;
    public Sprite[] chunkySprites;
    public Sprite[] inspectorSprites;
    public Sprite[] officerSprites;
    public Sprite[] dazerSprites;

	[Header("Parameters")]
	[SerializeField] int startingPedestriansOnSidewalk;
	[SerializeField] int pedestrianSpawnRate;
    [SerializeField, Range(0, 1)] float busStopPercentage;

	[Header("Role Introduction Times")]
	[SerializeField] float coinStartPercentage;
	[SerializeField] float stinkStartPercentage;
	[SerializeField] float chunkyStartPercentage;
	[SerializeField] float inspectorStartPercentage;
    [SerializeField] float officerStartPercentage;
    [SerializeField] float dazerStartPercentage;
	[SerializeField] float raverStartPercentage;
    
	[Header("References")]
	[SerializeField] GameObject pedestrianPrefab;
	[SerializeField] Transform pedestrianContainer;
	[SerializeField] Transform opposingSpawnerTransform;

    [Header("Sorting Layer References")]
    [SerializeField] Transform[] heightReferences;

    List<GameObject> streetcarStops;
    GameControllerV2 gameController;
	BoxCollider2D boxCollider;
	Vector3 leftEnd;
	Vector3 rightEnd;
	float gameTimer;
	float gameLength;

    //Effective rates used to calculate which pedestrians spawn.
    float effectiveTotalRate;
    float effectiveCoinRate;
    float effectiveStinkRate;
    float effectiveChunkyRate;
    float effectiveInspectorRate;
    float effectiveOfficerRate;
    float effectiveDazerRate;
    float effectiveRaverRate;

    public string layerName;
	public int layerOrderShift = 0;
    public int pedestrianCount;

	void Awake ()
    {
        //Link GC and get game length.
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerV2>();
        gameLength = gameController.GetGameLength();
        gameTimer = 0;

        //Locate streetcar stops.
        GameObject[] stops = GameObject.FindGameObjectsWithTag("StreetcarStop");
        streetcarStops = new List<GameObject>();
        foreach(GameObject stop in stops)
        {
            streetcarStops.Add(stop);
        }
        Debug.Log("Streetcar Stop Count: " + streetcarStops.Count);

        //Set spawn area.
        boxCollider = this.GetComponent<BoxCollider2D>();
        leftEnd = new Vector3(boxCollider.bounds.min.x, this.transform.position.y, 0);
        rightEnd = new Vector3(boxCollider.bounds.max.x, this.transform.position.y, 0);
        
        //Fill the sidewalk with standard pedestrians.
        for (int i = 0; i < startingPedestriansOnSidewalk; i++)
            CreateNormalPedestrian();

        //Initialize spawn rates for pedestrians.
        InitSpawnRates();

        //Begin recursively spawning pedestrians.
        StartCoroutine(RecursiveSpawnNewPedestrian());

        //Set pedestrian height references.
        Pedestrian.heightReferences = heightReferences;
    }

    void InitSpawnRates()
    {
        effectiveTotalRate = (coinPercentage + stinkPercentage + chunkyPercentage + inspectorPercentage + officerPercentage + dazerPercentage + raverPercentage) * 100;
        effectiveCoinRate = (coinPercentage) * 100;
        effectiveStinkRate = (coinPercentage + stinkPercentage) * 100;
        effectiveChunkyRate = (coinPercentage + stinkPercentage + chunkyPercentage) * 100;
        effectiveInspectorRate = (coinPercentage + stinkPercentage + chunkyPercentage + inspectorPercentage) * 100;
        effectiveOfficerRate = (coinPercentage + stinkPercentage + chunkyPercentage + inspectorPercentage + officerPercentage) * 100;
        effectiveDazerRate = (coinPercentage + stinkPercentage + chunkyPercentage + inspectorPercentage + officerPercentage + dazerPercentage) * 100;
        effectiveRaverRate = (coinPercentage + stinkPercentage + chunkyPercentage + inspectorPercentage + officerPercentage + dazerPercentage + raverPercentage) * 100;
    }
    
	void Update ()
    {
		gameTimer += Time.deltaTime;
		CreateNormalPedestrian();

        //Debug commands to spawn Roles.
		if(Input.GetKeyDown(KeyCode.Q))
            CreateSpecificRole(Role.Raver);
		else if(Input.GetKeyDown(KeyCode.W))
			CreateSpecificRole(Role.Officer);
		else if(Input.GetKeyDown(KeyCode.E)) 
			CreateSpecificRole(Role.Inspector);
    }

	IEnumerator RecursiveSpawnNewPedestrian ()
    {
		while(true)
        {
			CreateNewPedestrian();
			yield return new WaitForSeconds(pedestrianSpawnRate);
		}
	}

    //Spawns a new pedestrian with a random role.
	void CreateNewPedestrian ()
    {
		Vector3 randomPosition = new Vector3(Random.Range(leftEnd.x, rightEnd.x), this.transform.position.y, 0);
		GameObject newPedestrian = Instantiate(pedestrianPrefab, randomPosition, Quaternion.identity) as GameObject;
		GetNewRole(newPedestrian);
		SetDestination(newPedestrian);
        newPedestrian.GetComponent<Pedestrian>().pedestrianID = pedestrianCount;
        pedestrianCount++;
    }

    //Spawns a new standard pedestrian if less than 50 exist in pedestrian container.
	void CreateNormalPedestrian ()
    {
		if(pedestrianContainer.childCount < 50)
        {
			Vector3 spawnPosition = new Vector3(Random.Range(leftEnd.x, rightEnd.x), transform.position.y, 0);
			GameObject newPedestrian = Instantiate(pedestrianPrefab, spawnPosition, Quaternion.identity) as GameObject;
			newPedestrian.GetComponent<SpriteRenderer>().sprite = pedestrianSprites[Random.Range(0, pedestrianSprites.Length)];
			newPedestrian.GetComponent<Pedestrian>().SetRole(Role.Norm);
			newPedestrian.GetComponent<Pedestrian>().SetDestination(new Vector3((Random.value < 0.5f ? leftEnd.x : rightEnd.x), transform.position.y, 0));
			newPedestrian.transform.SetParent(pedestrianContainer);
            newPedestrian.GetComponent<Pedestrian>().pedestrianID = pedestrianCount;
            pedestrianCount++;
        }
	}

    //Randomly assigns the role of a new pedestrian.
    void GetNewRole (GameObject pedestrian)
    {
		float randomValue = Random.Range(0.0f, effectiveTotalRate);
		Pedestrian pedestrianScript = pedestrian.GetComponent<Pedestrian>();

		if (randomValue < effectiveCoinRate)
        {
			pedestrianScript.SetRole (Role.Coin);
			pedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = pedestrianSprites [Random.Range (0, pedestrianSprites.Length)];
		}
		else if (randomValue < effectiveStinkRate)
        {
			pedestrianScript.SetRole (Role.Stink);
			pedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = pedestrianSprites [Random.Range (0, pedestrianSprites.Length)];
		}
		else if (randomValue < effectiveChunkyRate)
        {
			pedestrianScript.SetRole (Role.Chunky);
			pedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = chunkySprites [Random.Range (0, chunkySprites.Length)];
		}
		else if (randomValue < effectiveInspectorRate)
        {
			pedestrianScript.SetRole (Role.Inspector);
			pedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = inspectorSprites [Random.Range (0, inspectorSprites.Length)];
		}
		else if (randomValue < effectiveOfficerRate)
        {
            pedestrianScript.SetRole(Role.Officer);
            pedestrian.GetComponentInChildren<SpriteRenderer>().sprite = officerSprites[Random.Range(0, officerSprites.Length)];
		}
		else if (randomValue < effectiveDazerRate)
        {
            pedestrianScript.SetRole(Role.Dazer);
            pedestrian.GetComponentInChildren<SpriteRenderer>().sprite = dazerSprites[Random.Range(0, dazerSprites.Length)];
        }
		else if (randomValue < effectiveRaverRate)
        {
			pedestrianScript.SetRole (Role.Raver);
		}
		else
        {
			pedestrianScript.SetRole (Role.Norm);
			pedestrian.GetComponentInChildren<SpriteRenderer> ().sprite = pedestrianSprites [Random.Range (0, pedestrianSprites.Length)];
		}

		if(pedestrianScript.GetRole() != Role.Norm) {

			pedestrianScript.SetMoveSpeed(2);
		}
	}

    //
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
			if(Random.value <= busStopPercentage)
            {
                GameObject streetcarStop;

                //Find a stop that the streetcar is not currently stopped at.
                do
                {
                    streetcarStop = streetcarStops[Random.Range(0, streetcarStops.Count - 1)];
                    if (streetcarStops.Count <= 1)
                        break;
                }
                while (streetcarStop.GetComponent<StreetcarStop>().StreetcarStopped());

                pedestrian.transform.SetParent(streetcarStop.transform);
                Vector3 pedestrianPosition = streetcarStop.transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.3f, 0.3f), 0);
                pedestrian.transform.position = pedestrianPosition;
                pedestrian.GetComponent<Pedestrian>().MakeBusstopPedestrian();
            }
			else
            {
                Vector3 newDestination = (Random.value < 0.5f) ? leftEnd : rightEnd;
                pedestrianScript.SetDestination(newDestination);
                pedestrian.transform.SetParent(pedestrianContainer);
            }
		}

		else if(pedestrianScript.GetRole() == Role.Inspector || pedestrianScript.GetRole() == Role.Officer || pedestrianScript.GetRole() == Role.Raver)
        {
			GameObject streetcarStop;

			do {

				streetcarStop = streetcarStops[Random.Range(0, streetcarStops.Count)];

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
