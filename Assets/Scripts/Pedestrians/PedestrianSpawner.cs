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
	[Header("Role Spawn Percentages")]
	[SerializeField, Range(0, 100)] float coinPercentage;
	[SerializeField, Range(0, 100)] float stinkPercentage;
	[SerializeField, Range(0, 100)] float chunkyPercentage;
	[SerializeField, Range(0, 100)] float inspectorPercentage;
	[SerializeField, Range(0, 100)] float officerPercentage;
    [SerializeField, Range(0, 100)] float dazerPercentage;
	[SerializeField, Range(0, 100)] float raverPercentage;

    [Header("Role Introduction Times (Percent)")]
	[SerializeField, Range(0, 100)] float coinStart;
	[SerializeField, Range(0, 100)] float stinkStart;
	[SerializeField, Range(0, 100)] float chunkyStart;
	[SerializeField, Range(0, 100)] float inspectorStart;
    [SerializeField, Range(0, 100)] float officerStart;
    [SerializeField, Range(0, 100)] float dazerStart;
	[SerializeField, Range(0, 100)] float raverStart;

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
    
	[Header("References")]
	[SerializeField] GameObject pedestrianPrefab;
    [SerializeField] Transform sidewalkContainer;
	[SerializeField] Transform opposingSpawnerTransform;

    [Header("Sorting Layer References")]
    [SerializeField] Transform[] heightReferences;

    List<GameObject> streetcarStops;
    GameControllerV2 gameController;
	BoxCollider2D boxCollider;
	// Vector3 leftEnd;
	private Vector3 rightEnd
    {
        get { return new Vector3(boxCollider.bounds.max.x, transform.position.y, 0); }
    }

    private Vector3 leftEnd
    {
        get { return new Vector3(boxCollider.bounds.min.x, transform.position.y, 0); }
    }

    //leftEnd = new Vector3(boxCollider.bounds.min.x, this.transform.position.y, 0);
    //rightEnd = new Vector3(boxCollider.bounds.max.x, this.transform.position.y, 0);
    float gameTimer;
	float gameLength;
    bool startSpawning = false;

    //Effective rates used to calculate which pedestrians spawn, assigned in InitSpawnRates().
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
        boxCollider = GetComponent<BoxCollider2D>();
        //leftEnd = new Vector3(boxCollider.bounds.min.x, this.transform.position.y, 0);
        //rightEnd = new Vector3(boxCollider.bounds.max.x, this.transform.position.y, 0);

        //Set pedestrian height references.
        Pedestrian.heightReferences = heightReferences;
    }

    private void Start()
    {
        //Fill the sidewalk with standard pedestrians.
        for (int i = 0; i < startingPedestriansOnSidewalk; i++)
            CreateNormalPedestrian();

        //Initialize spawn rates for pedestrians.
        InitSpawnRates();
    }

    void InitSpawnRates()
    {
        effectiveTotalRate = (coinPercentage + stinkPercentage + chunkyPercentage + inspectorPercentage + officerPercentage + dazerPercentage + raverPercentage);
        effectiveCoinRate = (coinPercentage);
        effectiveStinkRate = (coinPercentage + stinkPercentage);
        effectiveChunkyRate = (coinPercentage + stinkPercentage + chunkyPercentage);
        effectiveInspectorRate = (coinPercentage + stinkPercentage + chunkyPercentage + inspectorPercentage);
        effectiveOfficerRate = (coinPercentage + stinkPercentage + chunkyPercentage + inspectorPercentage + officerPercentage);
        effectiveDazerRate = (coinPercentage + stinkPercentage + chunkyPercentage + inspectorPercentage + officerPercentage + dazerPercentage);
        effectiveRaverRate = (coinPercentage + stinkPercentage + chunkyPercentage + inspectorPercentage + officerPercentage + dazerPercentage + raverPercentage);
    }
    
	void Update ()
    {
        //Begin recursively spawning pedestrians.
        if (!startSpawning && gameController.gameRunning)
        {
            startSpawning = true;
            StartCoroutine(RecursiveSpawnNewPedestrian());
        }

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
		while(gameController.gameRunning)
        {
			CreateNewPedestrian();
			yield return new WaitForSeconds(pedestrianSpawnRate);
		}
	}

    //Spawns a new pedestrian with a random role.
	void CreateNewPedestrian ()
    {
		Vector3 randomPosition = new Vector3(Random.Range(leftEnd.x, rightEnd.x), transform.position.y, 0);
		GameObject newPedestrian = Instantiate(pedestrianPrefab, randomPosition, Quaternion.identity) as GameObject;
		GetNewRole(newPedestrian);
		SetDestination(newPedestrian);
        newPedestrian.GetComponent<Pedestrian>().pedestrianID = pedestrianCount;
        pedestrianCount++;
    }

    //Spawns a new standard pedestrian if less than 50 exist in pedestrian container.
	void CreateNormalPedestrian ()
    {
		if(sidewalkContainer.childCount < 50)
        {
			Vector3 spawnPosition = new Vector3(Random.Range(leftEnd.x, rightEnd.x), transform.position.y, 0);
			GameObject newPedestrian = Instantiate(pedestrianPrefab, spawnPosition, Quaternion.identity) as GameObject;
			newPedestrian.GetComponent<SpriteRenderer>().sprite = pedestrianSprites[Random.Range(0, pedestrianSprites.Length)];
			newPedestrian.GetComponent<Pedestrian>().SetRole(Role.Norm);
			newPedestrian.GetComponent<Pedestrian>().SetDestination(new Vector3((Random.value < 0.5f ? leftEnd.x : rightEnd.x), transform.position.y, 0));
			newPedestrian.transform.SetParent(sidewalkContainer);
            newPedestrian.GetComponent<Pedestrian>().pedestrianID = pedestrianCount;
            pedestrianCount++;
        }
	}

    //Randomly assigns the role of a new pedestrian.
    void GetNewRole (GameObject pedestrian)
    {
		float randomValue = Random.Range(0.0f, 100.0f);
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

		if(pedestrianScript.GetRole() != Role.Norm)
			pedestrianScript.SetMoveSpeed(2);
	}

    //
	void SetDestination (GameObject pedestrian)
    {
		Pedestrian pedestrianScript = pedestrian.GetComponent<Pedestrian>();
        Role pedRole = pedestrianScript.GetRole();
        pedestrianScript.SetMoveDelayTime(1f);

		if(pedRole == Role.Norm)
        {
			Vector3 newDestination = (Random.value < 0.5f) ? leftEnd : rightEnd;
			pedestrianScript.SetDestination(newDestination);
			pedestrian.transform.SetParent(sidewalkContainer);
		}

		else
        {
            bool canSpawnAtStop = false;
            bool spawnSideWalk = false;
            
            //Only positive roles can spawn at streetcar stops.
            if (pedRole == Role.Coin || pedRole == Role.Inspector || pedRole == Role.Officer || pedRole == Role.Raver)
                canSpawnAtStop = true;
            
            //Random chance to spawn at streetcar stop.
			if(Random.value <= busStopPercentage && streetcarStops.Count > 0 && canSpawnAtStop)
            {
                GameObject chosenStop = null;
                
                if (streetcarStops.Count == 1)
                {
                    //If the streetcar is not stoppped at the stop, and the stop is not full.
                    if (!streetcarStops[0].GetComponent<StreetcarStop>().StreetcarStopped() &&
                            !streetcarStops[0].GetComponent<StreetcarStop>().StopIsFull())
                        chosenStop = streetcarStops[0];
                    else
                        spawnSideWalk = true;
                }

                else if (streetcarStops.Count > 1)
                {
                    int potentialStop = Random.Range(0, streetcarStops.Count);

                    //Run through all stops, starting with a random one. If it is suitable, break.
                    for (int i = streetcarStops.Count; i > 0; i--)
                    {
                        if (!streetcarStops[potentialStop].GetComponent<StreetcarStop>().StreetcarStopped() &&
                            !streetcarStops[potentialStop].GetComponent<StreetcarStop>().StopIsFull())
                        {
                            chosenStop = streetcarStops[potentialStop];
                            break;
                        }
                        else
                        {
                            potentialStop--;
                            if (potentialStop < 0)
                                potentialStop = streetcarStops.Count - 1;
                        }
                    }

                    if (chosenStop == null)
                        spawnSideWalk = true;
                }

                //If a suitable stop was found, spawn at it.
                if (!spawnSideWalk)
                {
                    Transform streetcarStopContainer = chosenStop.GetComponent<StreetcarStop>().GetContainer();
                    pedestrian.transform.SetParent(streetcarStopContainer);
                    Vector3 pedestrianPosition = chosenStop.transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.15f, 0.15f), 0);
                    pedestrian.transform.position = pedestrianPosition;
                    pedestrian.GetComponent<Pedestrian>().MakeBusstopPedestrian();
                }
            }

            //If no suitable stop is found, fall back to sidewalk spawn.
            else
            {
                spawnSideWalk = true;
            }
            
			if (spawnSideWalk)
            {
                Vector3 newDestination;
                Vector3 pedPos = pedestrian.transform.position;

                if (pedRole == Role.Coin)
                    newDestination = (Random.value < 0.5f) ? leftEnd : rightEnd;
                else
                    newDestination = new Vector3(pedPos.x, opposingSpawnerTransform.position.y, pedPos.z);

                pedestrianScript.SetDestination(newDestination);
                pedestrian.transform.SetParent(sidewalkContainer);
            }
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
			newPedestrian.transform.SetParent(sidewalkContainer);

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
