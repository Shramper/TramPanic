using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    [Header("References")]
    [SerializeField]
    private List<GameObject> Stations = new List<GameObject>();
    [SerializeField]
    private List<GameObject> Landmarks = new List<GameObject>();
    [SerializeField]
    private List<GameObject> Blocks = new List<GameObject>();
    [SerializeField]
    private List<Sprite> Barricades = new List<Sprite>();
    [SerializeField]
    private GameController GC;
    [SerializeField]
    private GameObject CarControllerObjects;

    [Header("Parameters")]
    [SerializeField]
    private int minBlocks = 3;
    [SerializeField]
    private int maxBlocks = 5;
    [SerializeField]
    [Range(0.0f, 100.0f)]
    private float landMarkChance = 2.0f;
    [Range(0.0f, 100.0f)]
    private float centralStopChance = 60.0f;
    [SerializeField]
    private float xOffset;

    private int minStations = 1;
    private int maxStations = 2;
    private List<int> stationIndexes = new List<int>();
    private List<int> stopIndexes = new List<int>();

    private GameObject[] Level;

    /// <summary>
    /// Hook stuff up to the generator before Start() is called anywhere.
    /// </summary>
    private void Awake()
    {
        Level = new GameObject[Random.Range(minBlocks, maxBlocks + 1)];
        GC.GameLength *= Level.Length / (minBlocks + maxBlocks / 2.0f);
    }

    /// <summary>
    /// Perform setup steps based on other things, finding refs etc.
    /// Currently not much here.
    /// </summary>
    void Start ()
    {
        generate();
	}

    /// <summary>
    /// Take necessary steps to put the level together.
    /// </summary>
    private void generate()
    {
        placeStations();
        placeLandmark();
        placeLevelCaps();

        // Fill the rest with blocks and spawn to level
        // Handle last spot first for bus stop reasons
        if (!Level[Level.Length - 1])
        {
            Debug.Log("Working on block " + (Level.Length - 1) + "/" + (Level.Length - 1));
            Level[Level.Length - 1] = Blocks[Random.Range(0, Blocks.Count)];

            handleStops(Level.Length - 1);
        }

        Vector3 pos = transform.position + ((Level.Length - 1) * new Vector3(xOffset, 0, 0));
        Instantiate(Level[Level.Length - 1], pos, Quaternion.identity);

        // Handle all except last block
        for (int i = 0; i < Level.Length - 1; i++)
        {
            Debug.Log("Working on block " + i + "/" + (Level.Length - 1));
            if (!Level[i])
            {
                Level[i] = Blocks[Random.Range(0, Blocks.Count)];
                handleStops(i);
            }

            pos = transform.position + (i * new Vector3(xOffset, 0, 0));
            Instantiate(Level[i], pos, Quaternion.identity);
        }

        // Move car controlling objects
        CarControllerObjects.transform.position = new Vector3((xOffset * Level.Length) - (xOffset / 2),
            CarControllerObjects.transform.position.y, 0);
    }

    private void handleStops(int i)
    {
        Debug.Log("<<handleStops");
        // Try to put stops at the end of levels if there is nothing there already
        if (i == 0 || i == Level.Length - 1)
        {
            turnOffBusStations(i, false);
            Debug.Log(">>handleStops, at end of level");
            return;
        }

        bool both = false;

        // Prevent stops on both sides of a station
        for(int j = 0; j < stationIndexes.Count; j++)
        {
            // Am I next to a station?
            //if (Mathf.Abs(j - i) == 1)
            //{
            //    // Is there another bus stop on the other side?
            //    Debug.Log("turnOffBusStations("+ i +", " + (stopIndexes.Contains(j + (j - i))) + "));");
            //    Debug.Log("That's seeing if stopIndexes contains: " + (j + (j - i)));
            //    turnOffBusStations(i, stopIndexes.Contains(j + (j - i)));
            //    Debug.Log(">>handleStops, next to stop");
            //    return;
            //}
            both = Mathf.Abs(j - 1) == 1 ? stopIndexes.Contains(j + (j - 1)) : both;
        }

        int denom = stationIndexes.Count > 1 ? 
            Mathf.Abs(stationIndexes[0] - stationIndexes[1]) :
            (Level.Length - 1) - stationIndexes[0];

        // Make stops more likely to turn off near stations
        // TODO : Add percentage in editor?
        Debug.Log(">>handleStops");
    }

    /// <summary>
    /// Handle spawning and placement of landmarks
    /// </summary>
    private void placeLandmark()
    {
        // We should spawn a landmark this time
        if (Random.Range(0, 100.0f) < landMarkChance && Level.Length > maxStations)
        {
            // keep vars in scope
            int x;
            bool searching;
            do
            {
                // Pick a spot
                x = Random.Range(0, Level.Length);
                searching = Level[x] != null; // see if there is anything there

                Level[x] = searching ? Level[x] : Landmarks[Random.Range(0, Landmarks.Count)]; // If there is, keep it there 
                                                                                               // If not put a landmark
            } while (searching); // keep doing this until we find an empty space
        }
    }

    /// <summary>
    /// Put invisible walls and visible excuses in place.
    /// This way the player can't travel off map.
    /// TODO : Hook this up to the minimap somehow
    /// </summary>
    private void placeLevelCaps()
    {
        // A good distance behind start
        constructBarrier(-5);
        // The mathematical end of the level
        constructBarrier((xOffset * (Level.Length - 1) ) + 5);

    }

    /// <summary>
    /// Constructs graphic for the barriers at the end of the level.
    /// People don't like invisible walls, so here is the visual.
    /// </summary>
    /// <param name="x">Position along the x axis to spawn.</param>
    private void constructBarrier(float x)
    {
        // Make new object
        GameObject barrierMarker = new GameObject();
        // Position and scale it
        barrierMarker.transform.position = new Vector3(x, -0.6f, 0);
        barrierMarker.transform.localScale = new Vector3(4, 4, 1);
        // Add a renderer to it
        SpriteRenderer barrierRenderer = barrierMarker.AddComponent<SpriteRenderer>();
        // Set up the renderer
        barrierRenderer.sprite = Barricades[Random.Range(0, Barricades.Count)];
        barrierRenderer.sortingLayerName = "Road";
        barrierRenderer.sortingOrder = 4;
        // Add a collider to it
        barrierMarker.AddComponent<BoxCollider2D>();
    }

    private void turnOffBusStations(int i, bool both)
    {
        Debug.Log("<<turnOffBusStations");
        // Find bus stations in the block at index i
        List<GameObject> busStops = new List<GameObject>();
        foreach(Transform child in Level[i].transform) // Might have to be Transform
        {
            Debug.Log(child.name);
            if (child.name.Contains("BusStop") && child.gameObject.activeSelf) // Is this child an active bus stop?
            {
                Debug.Log("Got one!");
                busStops.Add(child.gameObject); 
            }
        }

        // Should I turn them both off?
        if (both)
        {
            Debug.Log("Turning both stops off");
            foreach(GameObject stop in busStops)
            {
                stop.SetActive(false);
            }

            return;
        }

        if (busStops.Count > 1) // Make sure there isn't more than one stop per block
        {
            Debug.Log("Turning off a random stop if there are 2");
            busStops[Random.Range(0, busStops.Count)].SetActive(false);
        }
        stopIndexes.Add(i); // Remember where the remaining bus stop is
        Debug.Log("Bus stop is at index: " + i);
        Debug.Log(">>turnOffBusStations");
    }

    /// <summary>
    /// Spawn stations into the level.
    /// </summary>
    private void placeStations()
    {
        int u1 = Level.Length < 3 ?                     // Is the level less than 3 blocks long?
            Level.Length :                              // If it is, the station can be placed anywhere
            (Mathf.FloorToInt(Level.Length / 2) < 3 ?   // If not, is the midpoint less than 3 blocks in?
                Mathf.FloorToInt(Level.Length / 2) :    // If it is, place the first station to the left of the midpoint
                3);                                     // If not, place the station within the first 3 blocks

        int p1 = Random.Range(0, u1);
        int p2 = p1;
        
        // Determine if a 2nd station should be placed
        if (Level.Length >= 3)
        {
            // Place second station
            int u2 = (Level.Length - 3) <= p1 ? p1 + 1 : Level.Length - 3;
            p2 = Random.Range(u2, Level.Length);

            // If stations are next to one another
            if (p2 - p1 == 1)
            {
                // Find which is closer to centre and move it out toward, but not past, that side
                if (p1 > Level.Length - p2)
                {
                    p1 = --p1 < 0 ? 0 : p1;
                }
                else
                {
                    p2 = ++p2 >= Level.Length ? Level.Length - 1 : p2;
                }
            }

            // Place second station
            Level[p2] = Stations[Random.Range(0, Stations.Count)];
        }

        //Place first station
        Level[p1] = Stations[Random.Range(0, Stations.Count)];

        // Keep track of where stations were placed
        stationIndexes.Add(p1);
        if (p1 != p2)
        {
            stationIndexes.Add(p2);
        }
        foreach(int stationIndex in stationIndexes)
        {
            Debug.Log("Station at: " + stationIndex);
        }
    }
}
