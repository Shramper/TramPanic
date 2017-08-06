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
    [SerializeField]
    private float xOffset;

    private int minStations = 1;
    private int maxStations = 2;

    private GameObject[] Level;

    private void Awake()
    {
        Level = new GameObject[Random.Range(minBlocks, maxBlocks + 1)];
        GC.GameLength *= Level.Length / (minBlocks + maxBlocks / 2.0f);
    }

    // Use this for initialization
    void Start ()
    {
        generate();
	}

    private void generate()
    {
        placeStations();
        placeLandmark();
        placeLevelCaps();

        // Fill the rest with blocks and spawn to level
        for (int i = 0; i < Level.Length; i++)
        {
            if (!Level[i])
            {
                Level[i] = Blocks[Random.Range(0, Blocks.Count)];
            }

            Vector3 pos = transform.position + (i * new Vector3(xOffset, 0, 0));
            Instantiate(Level[i], pos, Quaternion.identity);
        }

        // Move car controlling objects
        CarControllerObjects.transform.position = new Vector3((xOffset * Level.Length) - (xOffset / 2),
            CarControllerObjects.transform.position.y, 0);
    }

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

    private void placeLevelCaps()
    {
        // A good distance behind start
        constructBarrier(-5);
        // The mathematical end of the level
        constructBarrier((xOffset * (Level.Length - 1) ) + 5);

    }

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

    private void placeStations()
    {
        int u1 = Level.Length < 3 ?                     // Is the level less than 3 blocks long?
            Level.Length :                              // If it is, the station can be placed anywhere
            (Mathf.FloorToInt(Level.Length / 2) < 3 ?   // If not, is the midpoint less than 3 blocks in?
                Mathf.FloorToInt(Level.Length / 2) :    // If it is, place the first station to the left of the midpoint
                3);                                     // If not, place the station within the first 3 blocks

        int p1 = Random.Range(0, u1);
        int p2;
        
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
    }
}
