using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Streetcar : MonoBehaviour
{
    #region Variables

    ////////// MY VARIABLES //////////////
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
    public static int score;

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
    [SerializeField] SpriteRenderer windowsSpriteRenderer;
    [SerializeField] Sprite nightWindows;
    [SerializeField] Animator leftButtonAnimator;
    [SerializeField] Animator rightButtonAnimator;
    public Text speedBoostUI;
    public GameObject pedestrian;
    public GameObject scorePanel;

    [Header("Minimap")]
    public GameObject minimapStreetCar;
    [SerializeField] Transform stationOneTransform;
    [SerializeField] Transform stationTwoTransform;
    [SerializeField] RectTransform miniStationOneTransform;
    [SerializeField] RectTransform miniStationTwoTransform;

    [Header("Ability Data")]
    [SerializeField] private int speedBoosts;
    [SerializeField] private int shields;
    [SerializeField] private int abilities;
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
    private GameController gameController;
    private MusicController musicController;
    private float raverBuffTime = 30;
    private bool scoreMultiplier = false;

    // Misc
    private Rigidbody2D rb2d;
    private Animator streetcarAnimator;
    private float moveSpeed = 0;
    private float passengerRemovalCounter;
    private bool stationUp = false;
    private bool stationDown = false;
    private bool canMove = true;

    #endregion

    void Awake()
    {
        //Check if on Desktop or Mobile.
        CheckDeviceType();

        //Set External References.
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        musicController = GameObject.FindGameObjectWithTag("MusicController").GetComponent<MusicController>();

        //Set Internal References.
        rb2d = this.GetComponent<Rigidbody2D>();
        streetcarAnimator = this.GetComponent<Animator>();
        colorStrobe = this.GetComponentInChildren<ColorStrobe>();

        //Lists and other things.
        PassengerInfo = new List<PedestrianData>();
        currentPassengers = 0;
        currentAbilities = new List<string>();
        for (int i = 0; i < 5; i++)
            currentAbilities.Add("");

        //Initialize parameters.
        maxSpeed = baseMaxSpeed;
        acceleration = baseAcceleration;
        speedBoostUI.text = inspectorNum.ToString();
        scoreMultiplier = false;
        score = 0;
        shields = 0;
        speedBoosts = 0;
        abilities = 0;
    }

    void Update()
    {
        CheckInput();
        Thrust();

        if (scoreMultiplier == true)
        {
            raverBuffTime -= Time.deltaTime;
            raverTimeBar.fillAmount = (raverBuffTime / 30);

            if (raverBuffTime <= 0)
            {
                scoreMultiplier = false;
                raverTimeBar.gameObject.SetActive(false);

                //Find raver in children, set destination, reactivate raver, and reset streetcar and passenger effects.
                for (int i = 0; i < this.transform.childCount; i++)
                {

                    if (this.transform.GetChild(i).CompareTag("Pedestrian"))
                    {

                        GameObject raver = this.transform.GetChild(i).gameObject;
                        raver.transform.parent = null;
                        raver.transform.position = this.transform.position + 1.5f * Vector3.down;
                        raver.GetComponent<Pedestrian>().enabled = true;
                        raver.GetComponent<Pedestrian>().SetDestination(this.transform.position + 3 * Vector3.down);
                        raver.GetComponent<SpriteRenderer>().enabled = true;
                        raver.GetComponent<Collider2D>().enabled = true;
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

        //With low time remaining, turn on streetcar's night windows.
        if (gameController.GetTimeRemaining() < 40 && windowsSpriteRenderer.sprite != nightWindows)
        {
            windowsSpriteRenderer.sprite = nightWindows;
        }
    }

    //Handles physical movement of streetcar, minimap streetcar, and dropoff of passengers.
    [System.Obsolete("Please update to account for variably placed stations")]
    void FixedUpdate()
    {
        if (canMove && gameController.GameStarted())
        {
            //Give streetcar friction if not inputting acceleration.
            if (!thrusting)
                moveSpeed *= frictionModifier;

            //Move the streetcar.
            rb2d.MovePosition(this.transform.position + (Vector3.right * moveSpeed));

            //Move minimap streetcar.
            //float percentageBetweenStations = transform.position.x / (stationTwoTransform.position.x - stationOneTransform.position.x);
            //float newMinimapStreetCarX = percentageBetweenStations * (miniStationTwoTransform.localPosition.x - miniStationOneTransform.localPosition.x) + miniStationOneTransform.localPosition.x;
            //minimapStreetCar.GetComponent<RectTransform>().localPosition = new Vector3(newMinimapStreetCarX, minimapStreetCar.GetComponent<RectTransform>().localPosition.y, 0);
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
        //If colliding with a pedestrian.
        if (other.gameObject.GetComponent<Pedestrian>())
        {
            Pedestrian collidedWith = other.gameObject.GetComponent<Pedestrian>();

            switch (collidedWith.GetRole())
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
                    raverBuffTime = 30;
                    raverTimeBar.fillAmount = 1;
                    raverTimeBar.gameObject.SetActive(true);
                    raverTimeBar.GetComponent<UIColorStrobe>().StartCoroutine("RecursiveColorChange");

                    GetComponent<AudioSource>().clip = raverSound;
                    GetComponent<AudioSource>().Play();

                    streetcarAnimator.SetBool("Raver", true);
                    musicController.PlayRaverMusic();

                    // Hide raver
                    other.transform.GetComponent<SpriteRenderer>().enabled = false;
                    other.transform.GetComponent<Collider2D>().enabled = false;
                    other.transform.GetComponent<Pedestrian>().enabled = false;
                    other.transform.position = this.transform.position;
                    other.transform.SetParent(this.transform);
                    break;

                //No slot taken.
                case Role.Dazer:
                    //Stun the streetcar.
                    StartCoroutine(TempDisableMovement(other.gameObject));

                    //Create stun noise. Done so that the remove passenger audio doesn't cancel out this stun noise.
                    GameObject dazerAudioGameObject = new GameObject();
                    dazerAudioGameObject.AddComponent<AudioSource>();
                    dazerAudioGameObject.GetComponent<AudioSource>().clip = stunSound;
                    dazerAudioGameObject = Instantiate(dazerAudioGameObject, Vector3.zero, Quaternion.identity) as GameObject;
                    dazerAudioGameObject.GetComponent<AudioSource>().Play();
                    Destroy(dazerAudioGameObject, 5);

                    //Force out half of passenger count.
                    int halfOfPassengers = (int)(0.5f * currentPassengers);
                    for (int i = 0; i < halfOfPassengers; i++)
                    {
                        int direction = (Random.value < 0.5f) ? -1 : 1;
                        RemovePassenger(direction);
                    }
                    Destroy(other.gameObject);
                    break;

                case Role.Stink:
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

                    //Adjust capacity panel after passengers removed.
                    PassengerObjects[currentPassengers].GetComponent<Image>().sprite = PassengerSprites[5];

                    //Trigger animations for streetcar and UI.
                    PassengerObjects[currentPassengers].GetComponent<Animator>().SetTrigger("Pulse");
                    streetcarAnimator.SetTrigger("Shrink");

                    //Add passenger data.
                    PassengerInfo.Add(new PedestrianData(other.gameObject.GetComponent<SpriteRenderer>().sprite, "Stink", currentPassengers));
                    stinkerNum++;
                    currentPassengers++;
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
                        if (!boosting)
                        {
                            maxSpeed = slowedMaxSpeed;
                            acceleration = slowedAcceleration;
                        }

                        GetComponent<AudioSource>().clip = slowSound;
                        GetComponent<AudioSource>().Play();
                        effectsAnimator.SetTrigger("Chunky");
                        PassengerObjects[currentPassengers].GetComponent<Image>().sprite = PassengerSprites[2];

                        //Add passenger data.
                        PassengerInfo.Add(new PedestrianData(other.gameObject.GetComponent<SpriteRenderer>().sprite, "Chunky", currentPassengers));
                        chunkyNum++;
                        currentPassengers++;
                        Destroy(other.gameObject);
                    }
                    break;
            }

            //Strobe capacity panel.
            if (scoreMultiplier)
                for (int i = 0; i < currentPassengers; i++)
                    PassengerObjects[i].GetComponent<UIColorStrobe>().StartCoroutine("RecursiveColorChange");

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
        this.GetComponent<SpriteRenderer>().color = Color.grey;
        colorStrobe.gameObject.GetComponent<SpriteRenderer>().color = Color.grey;
        Camera.main.GetComponent<CameraEffects>().ShakeCamera();

        GameObject newDazer = Instantiate(dazer, this.transform.position + Vector3.down, Quaternion.identity) as GameObject;
        newDazer.SetActive(false);

        yield return new WaitForSeconds(3);

        canMove = true;
        rb2d.bodyType = RigidbodyType2D.Dynamic;
        effectsAnimator.SetTrigger("Norm");
        this.GetComponent<SpriteRenderer>().color = Color.white;
        colorStrobe.gameObject.GetComponent<SpriteRenderer>().color = Color.white;

        newDazer.GetComponent<Pedestrian>().SetDestination(this.transform.position + 2 * Vector3.down);
        newDazer.SetActive(true);
    }

    IEnumerator speedBoost()
    {
        boosting = true;
        maxSpeed = boostMaxSpeed;
        acceleration = boostAcceleration;
        speedBoostUI.text = inspectorNum.ToString();

        yield return new WaitForSeconds(2);

        boosting = false;
        maxSpeed = baseMaxSpeed;
        acceleration = baseAcceleration;
    }

    ////////////////////////////////////

    public void ShowHurryUpText()
    {
        hurryUpText.gameObject.SetActive(true);
    }

    public void AddToScore(int scoreAddition)
    {
        score += scoreAddition;
    }

    public int GetScore()
    {
        return score;
    }

    public bool IsFull()
    {
        return (currentPassengers == maxPassengers);
    }




    //////////////////////////////////////    MY FUNCTIONS SAFE ZONE    /////////////////////////////////////////////////////////////

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
            Camera.main.GetComponentInChildren<CameraOverlay>().ShowOverlay();

            //Check all pedestrians for negative ones, and destroy them.
            GameObject[] allPedestrians = GameObject.FindGameObjectsWithTag("Pedestrian");
            foreach (GameObject pedestrianObject in allPedestrians)
            {
                Pedestrian pedestrian = pedestrianObject.GetComponent<Pedestrian>();
                if (pedestrian.GetRole() == Role.Stink || pedestrian.GetRole() == Role.Chunky || pedestrian.GetRole() == Role.Dazer)
                    Destroy(pedestrian.gameObject);
            }

            effectsAnimator.SetTrigger("Norm");
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
            Vector3 spawnPosition = this.transform.position + new Vector3(Random.Range(-1f, 1f), 0.3f * pedestrianDirection, 0);
            GameObject pedestrianPrefab = Instantiate(pedestrian, spawnPosition, Quaternion.identity) as GameObject;
            pedestrianPrefab.tag = (scoreMultiplier) ? "Raver" : "Fare";

            //Adjust the new pedestrian to look like the one removed.
            pedestrianPrefab.GetComponent<SpriteRenderer>().sprite = PassengerInfo[passengerIndex].sprite;
            pedestrianPrefab.GetComponent<SpriteRenderer>().sortingOrder = -pedestrianDirection * this.GetComponent<SpriteRenderer>().sortingOrder;
            pedestrianPrefab.GetComponent<Pedestrian>().SetDestination(this.transform.position + new Vector3(0, 2 * pedestrianDirection, 0));
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
            effectsAnimator.SetTrigger("Norm");
        }
    }

    public void RemoveInspector()
    {
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

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}