using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    [Header("References")]
    [SerializeField]
    private List<GameObject> Stations = new List<GameObject>();
    [SerializeField]
    private List<GameObject> Blocks = new List<GameObject>();

    [Header("Parameters")]
    [SerializeField]
    private int minBlocks = 3;
    [SerializeField]
    private int maxBlocks = 5;
    [SerializeField]
    private float xOffset;

    private int minStations = 1;
    private int maxStations = 2;

    private GameObject[] Level;

	// Use this for initialization
	void Start ()
    {
        Level = new GameObject[Random.Range(minBlocks, maxBlocks)];
        generate();
	}

    private void generate()
    {
        // Place first station
        int u1 = Level.Length < 3 ? Level.Length : 3;
        int p1 = Random.Range(0, u1);
        Level[p1] = Stations[Random.Range(0, Stations.Count)];
        // Determine if a 2nd stations should be placed
        if (Level.Length - p1 > 2)
        {
            // Place second station
            int u2 = (Level.Length - 2) < p1 ? p1 : Level.Length - 2;
            Level[Random.Range(u2, Level.Length - 1)] = Stations[Random.Range(0, Stations.Count)];
        }

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
    }
}
