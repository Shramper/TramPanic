using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Streetcar : MonoBehaviour
{
    #region Variables

    //Back end, hidden from inspector.

    private enum systemType { Desktop, Mobile };
    private systemType system;

    [HideInInspector] public bool accelerating = false;      //Is car moving right.
    [HideInInspector] public bool decelerating = false;      //Is car moving left.
    bool thrusting = false;                                  //Is car moving period.

    private int chunkyNum, inspectorNum, officerNum, stinkerNum, raverNum;

    public class PedestrianData
    {
        public Sprite sprite;
        public string role;
        public int position;

        public PedestrianData(Sprite sp, string ro, int po)
        {
            sprite = sp;
            role = ro;
            position = po;
        }
    }

    private float maxSpeed;
    private float acceleration;
    private bool boosting;
    private bool textEvent = false;

    [SerializeField]
    private float minDistToStation = 6f;

    //Front end, visible in inspector.

    [Header("Passenger Panel")]
    public List<GameObject> PassengerObjects;
    public List<Sprite> PassengerSprites;
    [SerializeField] public List<PedestrianData> PassengerInfo;
    [SerializeField] private int maxPassengers;
    [SerializeField] private int currentPassengers;

    [Header("Parameters")]
    public float baseMaxSpeed;
    public float baseAcceleration;
    public float boostMaxSpeed;
    public float boostAcceleration;
    public float slowedMaxSpeed;
    public float slowedAcceleration;
    public float frictionModifier;
    public float passengerLeaveRate;

    [Header("Audio")]
    public AudioClip pickupSound;
    public AudioClip coinSound;
    public AudioClip fartSound;
    public AudioClip slowSound;
    public AudioClip stunSound;
    public AudioClip speedSound;
    public AudioClip immuneSound;
    public AudioClip raverSound;

    [Header("References")]
    [SerializeField] Animator effectsAnimator;
    [SerializeField] Text hurryUpText;
    [SerializeField] Text eventText;
    [SerializeField] SpriteRenderer windowsSpriteRenderer;
    [SerializeField] Sprite nightWindows;
    [SerializeField] Animator leftButtonAnimator;
    [SerializeField] Animator rightButtonAnimator;
    public GameObject pedestrian;
    public GameObject scorePanel;
    //private GameObject[] stations;
    //public bool AwayFromStation
    //{
    //    get {
    //        if(stations[1] == null)
    //        {
    //            return xDiff(transform.position.x, stations[0].transform.position.x) > minDistToStation;
    //        }
    //        return xDiff(transform.position.x, stations[0].transform.position.x) > minDistToStation &&
    //          xDiff(transform.position.x, stations[1].transform.position.x) > minDistToStation;
    //    }
    //}

    [Header("Minimap")]
    public GameObject minimapStreetCar;
    public Transform stationOneTransform;
    public Transform stationTwoTransform;
    [SerializeField] RectTransform miniStationOneTransform;
    [SerializeField] RectTransform miniStationTwoTransform;
    [SerializeField] private MapGenerator LevelSpawner;
    private RectTransform miniStreetCarImg;

    [Header("Ability Data")]
    [SerializeField] private int speedBoosts;
    [SerializeField] private int shields;
    [SerializeField] private int abilities;
    public float shieldsActiveTime = 5.0f;
    private bool shieldsActive = false;
    public List<string> currentAbilities;
    public List<Sprite> abilitiesSprites;
    public List<SpriteRenderer> abilityCountHud;
    public Animator leftAbilityButton;
    public Animator rightAbilityButton;
    public RuntimeAnimatorController inspectorButtonAnimator;
    public RuntimeAnimatorController policeButtonAnimator;

    [Header("Raver")]
    [SerializeField] Image raverTimeBar;
    private ColorStrobe colorStrobe;
    private GameControllerV2 gameController;
    private MusicController musicController;
    private float raverBuffTimeStart = 30;
    private float raverBuffTimeCurrent = 30;
    private bool scoreMultiplier = false;

    // Misc
    private Rigidbody2D rb2d;
    private Animator streetcarAnimator;
    private float moveSpeed = 0;
    private float passengerRemovalCounter;
    private bool stationUp = false;
    private bool stationDown = false;
    private bool canMove = true;
    private float initY;

    //Minimap streetcar control.
    private bool randomLevel = false;
    public float staticWorldEndX;   //Only assigned in static levels.
    public float mapWidth;

    #endregion

    void Awake()
    {
        //Check if on Desktop or Mobile.
        CheckDeviceType();

        //Set External References.
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerV2>();
        musicController = GameObject.FindGameObjectWithTag("MusicController").GetComponent<MusicController>();

        //Set Internal References.
        rb2d = GetComponent<Rigidbody2D>();
        streetcarAnimator = GetComponent<Animator>();
        colorStrobe = GetComponentInChildren<ColorStrobe>();

        //Lists and other things.
        PassengerInfo = new List<PedestrianData>();
        currentPassengers = 0;
        currentAbilities = new List<string>();
        for (int i = 0; i < 5; i++)
            currentAbilities.Add("");

        //Initialize parameters.
        maxSpeed = baseMaxSpeed;
        acceleration = baseAcceleration;
        scoreMultiplier = false;
        shields = 0;
        speedBoosts = 0;
        abilities = 0;

        //Used for minimap streetcar tracking.
        if (SceneManager.GetActiveScene().name == "Main_Random")
            randomLevel = true;
    }

    private void Start()
    {
        miniStreetCarImg = minimapStreetCar.GetComponent<RectTransform>();
        raverTimeBar.gameObject.SetActive(true);
        raverTimeBar.color = Color.white;
        initY = transform.position.y;

        //stations = GameObject.FindGameObjectsWithTag("Station Entrance");
    }

    void Update()
    {
        CheckInput();
        Thrust();

        if (scoreMultiplier == true)
        {
            raverBuffTimeCurrent -= Time.deltaTime;
            raverTimeBar.fillAmount = (raverBuffTimeCurrent / raverBuffTimeStart);

            if (raverBuffTimeCurrent <= 0)
            {
                scoreMultiplier = false;
                raverBuffTimeCurrent = raverBuffTimeStart;
                //raverTimeBar.gameObject.SetActive(false);

                //Find raver in children, set destination, reactivate raver, and reset streetcar and passenger effects.
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).CompareTag("Pedestrian"))
                    {

                        GameObject raver = transform.GetChild(i).gameObject;
                        raver.transform.parent = null;
                        raver.transform.position = transform.position + new Vector3(0.0f, -1.5f, 0.0f);
                        raver.GetComponent<Pedestrian>().enabled = true;
                        raver.GetComponent<Pedestrian>().SetDestination(new Vector3(raver.transform.position.x, -3.0f, 0.0f));
                        raver.GetComponent<SpriteRenderer>().enabled = true;
                        raver.GetComponent<Collider2D>().enabled = true;
                        raver.GetComponent<Pedestrian>().ravingExpired = true;
                        break;
                    }
                }

                streetcarAnimator.SetBool("Raver", false);

                for (int i = 0; i < PassengerObjects.Count; i++)
                {
                    PassengerObjects[i].GetComponent<UIColorStrobe>().StopAllCoroutines();
                    PassengerObjects[i].GetComponent<Image>().color = Color.white;
                }

                musicController.PlayRegularMusic();
            }
        }

        // If the Y value has changed, don't
        if (transform.position.y != initY)
            transform.position = new Vector3(transform.position.x,
                initY,
                transform.position.z);
    }

    private float xDiff(float a, float b)
    {
        return Mathf.Abs(a - b);
    }

    //Handles physical movement of streetcar, minimap streetcar, and dropoff of passengers.
    [System.Obsolete("Please update to account for variably placed stations")]
    void FixedUpdate()
    {
        if (canMove && gameController.GetGameRunning())
        {
            //Give streetcar friction if not inputting acceleration.
            if (!thrusting)
                moveSpeed *= frictionModifier;

            //Move the streetcar. Also prevents streetcar from moving off center tracks.
            Vector3 nudgeCorrect = transform.position;
            float newY = 0.35f - nudgeCorrect.y;
            //Debug.Log("newY:" + newY);
            nudgeCorrect.y = -newY;
            rb2d.MovePosition(transform.position + nudgeCorrect);
            rb2d.MovePosition(transform.position + (Vector3.right * moveSpeed));
            //Debug.Log("pos: " + transform.position.y);

            //Move minimap streetcar.
            //float percentageBetweenStations = transform.position.x / (25.3f * 3);
            //float newMinimapStreetCarX = percentageBetweenStations * (miniStationTwoTransform.localPosition.x - miniStationOneTransform.localPosition.x) + miniStationOneTransform.localPosition.x;
            //minimapStreetCar.GetComponent<RectTransform>().localPosition = new Vector3(newMinimapStreetCarX, minimapStreetCar.GetComponent<RectTransform>().localPosition.y, 0);
            Vector2 position = miniStreetCarImg.anchoredPosition;

            //If statement removed since all levels are now random. Can be re-implemented if necessary for static levels if they are re-added to the game.
            //if (randomLevel)
                position.x = (transform.position.x / LevelSpawner.WorldEndXPos) * LevelSpawner.MapWidth - (LevelSpawner.MapWidth / 2);
            //else
                //position.x = (transform.position.x / staticWorldEndX) * mapWidth - (mapWidth / 2);

            
            miniStreetCarImg.anchoredPosition = position;
        }

        //Check if can dropoff passengers.
        if (Mathf.Abs(moveSpeed) < 0.01f)
        {
            if (stationDown)
                DropOffPassengers(-2);

            else if (stationUp)
                DropOffPassengers(2);
        }
    }

    //Handles all interaction with pedestrians, and barricades.
    void OnCollisionEnter2D(Collision2D other)
    {
        Pedestrian thisPedestrian = other.gameObject.GetComponent<Pedestrian>();
        //If colliding with a pedestrian.
        if (thisPedestrian)
        {
            //if (IsFull())
            //{
            //    //Destroy(other.collider); // Amazing that this doesn't break anything
            //    thisPedestrian.destination = new Vector3(thisPedestrian.destination.x,
            //        -thisPedestrian.destination.y,
            //        thisPedestrian.destination.z);
            //}
                switch (thisPedestrian.GetRole())
            {
                case Role.Coin:
                    if (currentPassengers < maxPassengers)
                    {
                        PassengerObjects[currentPassengers].GetComponent<Image>().sprite = PassengerSprites[1];
                        GetComponent<AudioSource>().clip = coinSound;
                        GetComponent<AudioSource>().Play();
                        streetcarAnimator.SetTrigger("Grow");

                        //Add passenger data.
                        PassengerInfo.Add(new PedestrianData(other.gameObject.GetComponent<SpriteRenderer>().sprite, "Coin", currentPassengers));
                        currentPassengers++;
                        Destroy(other.gameObject);
                    }
                    break;

                //No slot taken.
                case Role.Raver:
                    scoreMultiplier = true;
                    raverTimeBar.fillAmount = 1;
                    //raverTimeBar.gameObject.SetActive(true);
                    raverTimeBar.GetComponent<UIColorStrobe>().StartCoroutine(raverTimeBar.GetComponent<UIColorStrobe>().RecursiveColorChange(raverBuffTimeStart));

                    GetComponent<AudioSource>().clip = raverSound;
                    GetComponent<AudioSource>().Play();

                    streetcarAnimator.SetBool("Raver", true);
                    musicController.PlayRaverMusic();
                    StartCoroutine(EventTextAnim("Double Points!!"));

                    // Hide raver
                    other.transform.GetComponent<SpriteRenderer>().enabled = false;
                    other.transform.GetComponent<Collider2D>().enabled = false;
                    other.transform.GetComponent<Pedestrian>().enabled = false;
                    other.transform.position = transform.position;
                    other.transform.SetParent(transform);
                    break;

                //No slot taken.
                case Role.Dazer:
                    if (!shieldsActive)
                    {
                        //Stun the streetcar.
                        StartCoroutine(TempDisableMovement(other.gameObject));

                        //Create stun noise. Done so that the remove passenger audio doesn't cancel out this stun noise.
                        GameObject dazerAudioGameObject = new GameObject();
                        dazerAudioGameObject.AddComponent<AudioSource>();
                        dazerAudioGameObject.GetComponent<AudioSource>().clip = stunSound;
                        dazerAudioGameObject = Instantiate(dazerAudioGameObject, Vector3.zero, Quaternion.identity) as GameObject;
                        dazerAudioGameObject.GetComponent<AudioSource>().Play();
                        Destroy(dazerAudioGameObject, 5);
                        StartCoroutine(EventTextAnim("Stunned!"));

                        //Force out half of passenger count.
                        int halfOfPassengers = (int)(0.5f * currentPassengers);
                        for (int i = 0; i < halfOfPassengers; i++)
                        {
                            int direction = (Random.value < 0.5f) ? -1 : 1;
                            RemovePassenger(direction);
                        }
                    }

                    Destroy(other.gameObject);
                    break;

                case Role.Stink:
                    if (!shieldsActive)
                    {
                        //Create fart noise. Done so that the remove passenger audio doesn't cancel out this fart noise.
                        GameObject fartSoundGameobject = new GameObject();
                        fartSoundGameobject.AddComponent<AudioSource>();
                        fartSoundGameobject.GetComponent<AudioSource>().clip = fartSound;
                        fartSoundGameobject = Instantiate(fartSoundGameobject, Vector3.zero, Quaternion.identity) as GameObject;
                        fartSoundGameobject.GetComponent<AudioSource>().Play();
                        Destroy(fartSoundGameobject, 5);

                        //Force out random number of passengers.
                        int passengersToRemove = Random.value < 0.5f ? 3 : 4;
                        for (int i = 0; i < passengersToRemove; i++)
                        {
                            int direction = (Random.value < 0.5f) ? -1 : 1;
                            RemovePassenger(direction);
                        }

                        raverTimeBar.fillAmount = 1;
                        raverTimeBar.GetComponent<UIColorStrobe>().StartCoroutine(raverTimeBar.GetComponent<UIColorStrobe>().StinkerColorChange(2));
                        StartCoroutine(EventTextAnim("Gross..."));

                        //Adjust capacity panel after passengers removed.
                        PassengerObjects[currentPassengers].GetComponent<Image>().sprite = PassengerSprites[5];

                        //Trigger animations for streetcar and UI.
                        PassengerObjects[currentPassengers].GetComponent<Animator>().SetTrigger("Pulse");
                        streetcarAnimator.SetTrigger("Shrink");

                        //Add passenger data.
                        PassengerInfo.Add(new PedestrianData(other.gameObject.GetComponent<SpriteRenderer>().sprite, "Stink", currentPassengers));
                        stinkerNum++;
                        currentPassengers++;
                    }

                    Destroy(other.gameObject);
                    break;

                case Role.Inspector:
                    if (currentPassengers < maxPassengers)
                    {
                        if (abilities < 4)
                        {
                            speedBoosts++;
                            currentAbilities[abilities] = "speedboost";
                            abilities++;
                            inspectorNum++;
                            UpdateAbilities();
                        }

                        GetComponent<AudioSource>().clip = pickupSound;
                        GetComponent<AudioSource>().Play();

                        PassengerObjects[currentPassengers].GetComponent<Image>().sprite = PassengerSprites[3];

                        //Add passenger data.
                        PassengerInfo.Add(new PedestrianData(other.gameObject.GetComponent<SpriteRenderer>().sprite, "Inspector", currentPassengers));
                        currentPassengers++;
                        Destroy(other.gameObject);
                    }

                    break;

                case Role.Officer:
                    if (currentPassengers < maxPassengers)
                    {
                        if (abilities < 4)
                        {
                            shields++;
                            currentAbilities[abilities] = "shield";
                            abilities++;
                            officerNum++;
                            UpdateAbilities();
                        }

                        GetComponent<AudioSource>().clip = pickupSound;
                        GetComponent<AudioSource>().Play();

                        PassengerObjects[currentPassengers].GetComponent<Image>().sprite = PassengerSprites[4];

                        //Add passenger data.
                        PassengerInfo.Add(new PedestrianData(other.gameObject.GetComponent<SpriteRenderer>().sprite, "Officer", currentPassengers));
                        currentPassengers++;
                        Destroy(other.gameObject);
                    }

                    break;

                case Role.Chunky:
                    if (currentPassengers < maxPassengers)
                    {
                        if (!shieldsActive)
                        { 
                            if (!boosting)
                            {
                                maxSpeed = slowedMaxSpeed;
                                acceleration = slowedAcceleration;
                            }

                            GetComponent<AudioSource>().clip = slowSound;
                            GetComponent<AudioSource>().Play();
                   
                            effectsAnimator.SetTrigger("Chunky");
                            StartCoroutine(EventTextAnim("Slowed Down..."));

                            PassengerObjects[currentPassengers].GetComponent<Image>().sprite = PassengerSprites[2];

                            //Add passenger data.
                            PassengerInfo.Add(new PedestrianData(other.gameObject.GetComponent<SpriteRenderer>().sprite, "Chunky", currentPassengers));
                            chunkyNum++;
                            currentPassengers++;
                        }
                        Destroy(other.gameObject);
                    }
                    break;
            }

            //Strobe capacity panel.
            if (scoreMultiplier)
                for (int i = 0; i < currentPassengers; i++)
                    PassengerObjects[i].GetComponent<UIColorStrobe>().StartCoroutine(PassengerObjects[i].GetComponent<UIColorStrobe>().RecursiveColorChange(raverBuffTimeStart));

            if (currentPassengers > 0)
                PassengerObjects[currentPassengers - 1].GetComponentInChildren<Animator>().SetTrigger("Pulse");

            streetcarAnimator.SetBool("Full", (currentPassengers == maxPassengers));
        }

        //If colliding with a barricade.
        else if (other.transform.CompareTag("Barricade"))
        {
            Camera.main.GetComponent<CameraEffects>().ShakeCamera(0.05f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "StationUp")
        {
            stationUp = true;
            stationDown = false;
        }

        if (other.gameObject.tag == "StationDown")
        {
            stationUp = false;
            stationDown = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "StationUp")
        {
            stationUp = false;
            stationDown = false;
        }

        if (other.gameObject.tag == "StationDown")
        {
            stationUp = false;
            stationDown = false;
        }
    }

    public float GetMoveSpeed()
    {

        return moveSpeed;
    }

    ////////////////////////////////////

    IEnumerator TempDisableMovement(GameObject dazer)
    {

        canMove = false;
        rb2d.bodyType = RigidbodyType2D.Static;
        effectsAnimator.SetTrigger("Dazer");
        
        GetComponent<SpriteRenderer>().color = Color.grey;
        colorStrobe.gameObject.GetComponent<SpriteRenderer>().color = Color.grey;
        Camera.main.GetComponent<CameraEffects>().ShakeCamera();

        GameObject newDazer = Instantiate(dazer, transform.position + Vector3.down, Quaternion.identity) as GameObject;
        newDazer.SetActive(false);

        yield return new WaitForSeconds(3);

        canMove = true;
        rb2d.bodyType = RigidbodyType2D.Dynamic;

        effectsAnimator.SetTrigger("Norm");
        if (chunkyNum > 0)
            effectsAnimator.SetTrigger("Chunky");
            

        GetComponent<SpriteRenderer>().color = Color.white;
        colorStrobe.gameObject.GetComponent<SpriteRenderer>().color = Color.white;

        newDazer.GetComponent<Pedestrian>().SetDestination(transform.position + 2 * Vector3.down);
        newDazer.SetActive(true);
    }

    IEnumerator speedBoost()
    {
        boosting = true;
        maxSpeed = boostMaxSpeed;
        acceleration = boostAcceleration;

        yield return new WaitForSeconds(2);

        boosting = false;
        maxSpeed = baseMaxSpeed;
        acceleration = baseAcceleration;
    }

    ////////////////////////////////////

    public void ShowHurryUpText(bool flag)
    {
        hurryUpText.gameObject.SetActive(flag);
    }

    public bool IsFull()
    {
        return (currentPassengers == maxPassengers);
    }

    void CheckDeviceType()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop)
            system = systemType.Desktop;
        else if (SystemInfo.deviceType == DeviceType.Handheld)
            system = systemType.Mobile;
    }

    void CheckInput()
    {
        //If user is on desktop.
        if (system == systemType.Desktop)
        {
            //Presses movement key.
            if (Input.GetKeyDown(KeyCode.D))
            {
                accelerating = true;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                decelerating = true;
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                ActivateAbility();
            }

            //Releases movement key.
            if (Input.GetKeyUp(KeyCode.D))
            {
                accelerating = false;
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                decelerating = false;
            }
        }
    }

    public void MobileAcceleration(bool accel)
    {
        accelerating = accel;
    }

    public void MobileDecceleration(bool accel)
    {
        decelerating = accel;
    }

    public void Thrust()
    {
        thrusting = true;

        //If holding button to move right.
        if (accelerating)
        {
            if (moveSpeed < 0) { moveSpeed *= frictionModifier; }
            moveSpeed += acceleration;
            rightButtonAnimator.SetBool("ButtonDown", true);
        }
        //If holding button to move left.
        if (decelerating)
        {
            if (moveSpeed > 0) { moveSpeed *= frictionModifier; }
            moveSpeed -= acceleration;
            leftButtonAnimator.SetBool("ButtonDown", true);
        }
        //If not holding a movement button.
        if (!accelerating && !decelerating)
        {
            thrusting = false;
            leftButtonAnimator.SetBool("ButtonDown", false);
            rightButtonAnimator.SetBool("ButtonDown", false);
        }

        moveSpeed = Mathf.Clamp(moveSpeed, -maxSpeed, maxSpeed);
    }

    /*  Called when abilities are used, or when ability passengers board/leave the streetcar.
        Adjusts the UI to properly display abilities, doesn't use abilities themselves.   */
    public void UpdateAbilities()
    {
        if (currentAbilities[0] == "speedboost")
        {
            leftButtonAnimator.SetTrigger("Speed");
            leftAbilityButton.gameObject.SetActive(true);
            leftAbilityButton.runtimeAnimatorController = inspectorButtonAnimator;

            rightButtonAnimator.SetTrigger("Speed");
            rightAbilityButton.gameObject.SetActive(true);
            rightAbilityButton.runtimeAnimatorController = inspectorButtonAnimator;
        }
        else if (currentAbilities[0] == "shield")
        {
            leftButtonAnimator.SetTrigger("Police");
            leftAbilityButton.gameObject.SetActive(true);
            leftAbilityButton.runtimeAnimatorController = policeButtonAnimator;

            rightButtonAnimator.SetTrigger("Police");
            rightAbilityButton.gameObject.SetActive(true);
            rightAbilityButton.runtimeAnimatorController = policeButtonAnimator;
        }
        else
        {
            leftButtonAnimator.SetTrigger("Normal");
            leftAbilityButton.gameObject.SetActive(false);
            rightButtonAnimator.SetTrigger("Normal");
            rightAbilityButton.gameObject.SetActive(false);
        }

        for (int i = 0; i < 4; i++)
        {
            if (currentAbilities[i] == "speedboost")
            {
                abilityCountHud[i].sprite = abilitiesSprites[0];
                abilityCountHud[i].gameObject.SetActive(true);
            }
            else if (currentAbilities[i] == "shield")
            {
                abilityCountHud[i].sprite = abilitiesSprites[1];
                abilityCountHud[i].gameObject.SetActive(true);
            }
            else
                abilityCountHud[i].gameObject.SetActive(false);
        }
    }

    //Activate correct ability, shift the ability queue.
    public void ActivateAbility()
    {
        if (currentAbilities[0] == "speedboost")
            ActivateSpeedBoost();
        else if (currentAbilities[0] == "shield")
            ActivateShield();

        for (int i = 0; i < 4; i++)
            currentAbilities[i] = currentAbilities[i + 1];

        UpdateAbilities();
    }

    public void ActivateShield()
    {
        if (shields > 0)
        {
            shields--;
            abilities--;

            GetComponent<AudioSource>().clip = immuneSound;
            GetComponent<AudioSource>().Play();

            if (maxSpeed < baseMaxSpeed) { maxSpeed = baseMaxSpeed; }

            /*
            //Camera.main.GetComponentInChildren<CameraOverlay>().ShowOverlay();
            //Check all pedestrians for negative ones, and destroy them.
            GameObject[] allPedestrians = GameObject.FindGameObjectsWithTag("Pedestrian");
            foreach (GameObject pedestrianObject in allPedestrians)
            {
                Pedestrian pedestrian = pedestrianObject.GetComponent<Pedestrian>();
                if (pedestrian.GetRole() == Role.Stink || pedestrian.GetRole() == Role.Chunky || pedestrian.GetRole() == Role.Dazer)
                    Destroy(pedestrian.gameObject);
            }
            */

            StartCoroutine("ShieldsUp");
            if (scoreMultiplier)
                StartCoroutine(EventTextAnim("CorrEct aNd SwerVe..."));
            else
                StartCoroutine(EventTextAnim("Protect and Serve!"));

            raverTimeBar.GetComponent<UIColorStrobe>().StartCoroutine(raverTimeBar.GetComponent<UIColorStrobe>().ShieldColorChange(shieldsActiveTime));
        }
    }

    //Called from Right ability button when speed boosts are available.
    public void ActivateSpeedBoost()
    {
        if (speedBoosts > 0)
        {
            speedBoosts--;
            abilities--;

            GetComponent<AudioSource>().clip = speedSound;
            GetComponent<AudioSource>().Play();

            StartCoroutine(EventTextAnim("Speed Up!"));
            StartCoroutine(speedBoost());
        }
    }

    //Called from FixedUpdate when not moving and in trigger of station.
    public void DropOffPassengers(int pedestrianDirection)
    {
        if (currentPassengers > 0)
        {
            passengerRemovalCounter += Time.deltaTime;

            if (passengerRemovalCounter > passengerLeaveRate)
            {
                RemovePassenger(pedestrianDirection);
            }
        }
    }

    //Called from CollisionEnter and DropOffPassengers when dropping off passengers at station, or if a stinker collides with the streetcar.
    public void RemovePassenger(int pedestrianDirection)
    {
        if (currentPassengers > 0)
        {
            //Instantiate the pedestrian back on the street.
            int passengerIndex = currentPassengers - 1;
            Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-1f, 1f), 0.3f * pedestrianDirection, 0);
            GameObject pedestrianPrefab = Instantiate(pedestrian, spawnPosition, Quaternion.identity) as GameObject;
            pedestrianPrefab.tag = (scoreMultiplier) ? "Raver" : "Fare";

            //Adjust the new pedestrian to look like the one removed.
            pedestrianPrefab.GetComponent<SpriteRenderer>().sprite = PassengerInfo[passengerIndex].sprite;
            pedestrianPrefab.GetComponent<SpriteRenderer>().sortingOrder = -pedestrianDirection * GetComponent<SpriteRenderer>().sortingOrder;
            pedestrianPrefab.GetComponent<Pedestrian>().SetDestination(transform.position + new Vector3(0, 2 * pedestrianDirection, 0));
            pedestrianPrefab.GetComponent<Pedestrian>().SetMoveSpeed(1.5f);
            pedestrianPrefab.GetComponent<Collider2D>().isTrigger = true;

            string role = PassengerInfo[currentPassengers - 1].role;
            switch (role)
            {
                case "Coin":
                    RemoveCoin();
                    break;

                case "Chunky":
                    RemoveChunky();
                    break;

                case "Inspector":
                    RemoveInspector();
                    break;

                case "Officer":
                    RemoveOfficer();
                    break;

                case "Stinker":
                    RemoveStinker();
                    break;
            }

            PassengerInfo.RemoveAt(currentPassengers - 1);
            currentPassengers--;

            passengerRemovalCounter = 0;
            GetComponent<AudioSource>().clip = pickupSound;
            GetComponent<AudioSource>().Play();

            PassengerObjects[currentPassengers].GetComponent<Image>().sprite = PassengerSprites[0];
            streetcarAnimator.SetBool("Full", (currentPassengers == maxPassengers));
        }
    }

    public void RemoveCoin()
    {
        
    }

    public void RemoveChunky()
    {
        chunkyNum--;
        if (chunkyNum <= 0)
        {
            maxSpeed = baseMaxSpeed;
            acceleration = baseAcceleration;
            effectsAnimator.ResetTrigger("Chunky");
            effectsAnimator.SetTrigger("Norm");
        }
    }

    public void RemoveInspector()
    {
        if (inspectorNum > 0)
            inspectorNum--;

        if (speedBoosts > inspectorNum)
        {
            abilities--;
            speedBoosts--;

            for(int i = 0; i < 4; i++)
            {
                if(currentAbilities[i] == "speedboost")
                {
                    for (int j = i; j < 4; j++)
                        currentAbilities[j] = currentAbilities[j + 1];

                    break;
                }
            }

            UpdateAbilities();
        }

        if (inspectorNum <= 0)
        {
            maxSpeed = baseMaxSpeed;
            acceleration = baseAcceleration;
            effectsAnimator.SetTrigger("Norm");
        }
    }

    public void RemoveOfficer()
    {
        if (officerNum > 0)
            officerNum--;

        if (shields > officerNum)
        {
            abilities--;
            shields--;

            for (int i = 0; i < 4; i++)
            {
                if (currentAbilities[i] == "shield")
                {
                    for (int j = i; j < 4; j++)
                        currentAbilities[j] = currentAbilities[j + 1];

                    break;
                }
            }

            UpdateAbilities();
        }
    }

    public void RemoveStinker()
    {
        stinkerNum--;
    }

    public void ResetContents()
    {
        PassengerInfo = new List<PedestrianData>();
        currentPassengers = 0;
        currentAbilities = new List<string>();
        for (int i = 0; i < 5; i++)
            currentAbilities.Add("");
        UpdateAbilities();

        effectsAnimator.SetTrigger("Norm");

        foreach (GameObject marker in PassengerObjects)
        {
            marker.GetComponent<Image>().sprite = PassengerSprites[0];
        }
    }


    IEnumerator ShieldsUp()
    {
        if (!scoreMultiplier)
            raverTimeBar.fillAmount = 1;
        shieldsActive = true;

        yield return new WaitForSeconds(shieldsActiveTime);

        if (!scoreMultiplier)
            raverTimeBar.fillAmount = 0;
        shieldsActive = false;
    }

    IEnumerator EventTextAnim(string txt)
    {
        if (!textEvent) //Only one text event at a time.
        {
            float totalAnimTime = 1.0f;
            float animTime = 0.0f;
            float step = 0.01f;
            float initialY = 1.4f;
            float finalY = initialY + 0.6f;
            float dy = finalY - initialY;

            textEvent = true;
            RectTransform rect = eventText.gameObject.GetComponent<RectTransform>();
            Vector3 newPos = Vector3.zero;
            eventText.gameObject.SetActive(true);
            eventText.text = txt;

            while (animTime < totalAnimTime)
            {
                newPos = new Vector3(transform.position.x, initialY + (dy * (animTime / totalAnimTime)), 0.0f);
                rect.SetPositionAndRotation(newPos, Quaternion.identity);
                yield return new WaitForSeconds(step);
                animTime += step;
            }

            //reset text location.
            eventText.gameObject.SetActive(false);
            textEvent = false;
        }
    }
}