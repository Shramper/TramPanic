using UnityEngine;
using System.Collections;

public class CarSpawner : MonoBehaviour
{

    // Array for placing car prefabs;
    public GameObject[] carArray;
    public Color[] carColorOptions;

    //  Spawn location
    public Transform carSpawnPoint = null;

    public float minSpawnTime;
    public float maxSpawnTime;
    public string layerName;
    public int layerOrderShift = 0;
    public bool timerActive = false;

    [SerializeField]
    Transform carContainerTransform;

    private float timer;
    private int randomSpawn;

    private void Start()
    {
        StartCoroutine(spawnCar());
    }

    /// <summary>
    /// Coroutine replaces all the old if statements
    /// and timer stuff.
    /// Put multiples of cars in the carArray to influence 
    /// spawn chance.
    /// </summary>
    private IEnumerator spawnCar()
    {
        while (timerActive)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));

            randomSpawn = Random.Range(0, carArray.Length);

            GameObject car = null;

            car = Instantiate(carArray[randomSpawn], carSpawnPoint.position, carSpawnPoint.rotation) as GameObject;
            // Color car randomly
            if (carArray[randomSpawn].name != "Taxi" && carArray[randomSpawn].name != "Police")
            {
                SpriteRenderer spriteRenderer = car.transform.GetChild(2).GetComponent<SpriteRenderer>();
                spriteRenderer.color = carColorOptions[Random.Range(0, carColorOptions.Length)];
            }

            car.transform.SetParent(carContainerTransform);

            // Shift sprite layer order for cars that go behind or in front of the streetcar
            if (layerOrderShift != 0)
            {

                SpriteRenderer[] spriteRenderers = car.GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer sortRenderer in spriteRenderers)
                {
                    sortRenderer.sortingLayerName = layerName;
                    int newSortingOrder = sortRenderer.sortingOrder + layerOrderShift;
                    sortRenderer.sortingOrder = newSortingOrder;
                }
            }

        }
    }
}