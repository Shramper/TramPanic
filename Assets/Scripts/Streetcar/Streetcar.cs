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

    private enum systemType { Desktop, Mobile };
    private systemType system;

    [HideInInspector] public bool accelerating = false;      //Is car moving right.
    [HideInInspector] public bool decelerating = false;      //Is car moving left.
    bool thrusting = false;                                  //Is car moving period.
    public float frictionModifier = 0.9f;

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

    [Header("Passenger Panel")]
    public List<GameObject> PassengerObjects;
    public List<Sprite> PassengerSprites;
    public List<PedestrianData> PassengerInfo;
    [SerializeField] private int maxPassengers;
    private int currentPassengers;
    private int chunkyNum, inspectorNum, officerNum, stinkerNum, raverNum;



    //////////////////////////////////////
    //TO BE REMOVED

    public List<Role> abilityPassengers = new List<Role>(2);
    //public List<Sprite> streetCarPassengers;
    //public List<string> streetCarPassengersRole;
    public List<string> abilities = new List<string>(2);
    //private bool chunkyOnBoard = false;
    //private bool inspectorOnBoard = false;
    //public int inspectorCount;

    //////////////////////////////////////

    [Header("Parameters")]
    [SerializeField] float acceleration = 0.001f;
    [SerializeField] float maxSpeed = 0.1f;
    public float passengerLeaveRate;
    public Text speedBoostUI;

    [Header("Score")]
    public static int score;
    public GameObject scorePanel;

    [Header("Audio")]
    public AudioClip pickupSound;
    public AudioClip coinSound;
    public AudioClip fartSound;
    public AudioClip slowSound;
    public AudioClip stunSound;
    public AudioClip speedSound;
    public AudioClip immuneSound;
    public AudioClip raverSound;

    [Header("Passenger Info")]


    public GameObject pedestrian;

    [Header("References")]
    [SerializeField] Animator effectsAnimator;
    [SerializeField] Text hurryUpText;
    [SerializeField] SpriteRenderer windowsSpriteRenderer;
    [SerializeField] Sprite nightWindows;
    [SerializeField] Animator leftButtonAnimator;
    [SerializeField] Animator rightButtonAnimator;

    [Header("Minimap")]
    public GameObject minimapStreetCar;
    [SerializeField] Transform stationOneTransform;
    [SerializeField] Transform stationTwoTransform;
    [SerializeField] RectTransform miniStationOneTransform;
    [SerializeField] RectTransform miniStationTwoTransform;

    [Header("Ability Data")]
    [SerializeField] Sprite[] abilitiesSprites;
    public SpriteRenderer FirstAbilitySprite;
    public SpriteRenderer SecondAbilitySprite;
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

        FirstAbilitySprite = GameObject.Find("AbilitySprite1").GetComponent<SpriteRenderer>();
        SecondAbilitySprite = GameObject.Find("AbilitySprite2").GetComponent<SpriteRenderer>();

        //Set Internal References.
        rb2d = this.GetComponent<Rigidbody2D>();
        streetcarAnimator = this.GetComponent<Animator>();
        colorStrobe = this.GetComponentInChildren<ColorStrobe>();

        //Lists and other things.
        PassengerInfo = new List<PedestrianData>();
        currentPassengers = 0;

        //
        speedBoostUI.text = inspectorNum.ToString();
        scoreMultiplier = false;
        score = 0;

        //Other Functions.
        UpdateAbilitySpriteOrder();
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

    void FixedUpdate()
    {
        if (canMove && gameController.GameStarted())
        {
            // Give streetcar friction if not inputting acceleration
            if (!thrusting)
            {
                moveSpeed *= frictionModifier;
            }

            // Move the streetcar
            rb2d.MovePosition(this.transform.position + (Vector3.right * moveSpeed));

            // Move minimap streetcar
            float percentageBetweenStations = this.transform.position.x / (stationTwoTransform.position.x - stationOneTransform.position.x);
            float newMinimapStreetCarX = percentageBetweenStations * (miniStationTwoTransform.localPosition.x - miniStationOneTransform.localPosition.x) + miniStationOneTransform.localPosition.x;
            minimapStreetCar.GetComponent<RectTransform>().localPosition = new Vector3(newMinimapStreetCarX, minimapStreetCar.GetComponent<RectTransform>().localPosition.y, 0);
        }

        // Check if can dropoff people
        if (Mathf.Abs(moveSpeed) < 0.01f)
        {
            if (stationDown)
                DropOffPassengers(-2);

            else if (stationUp)
                DropOffPassengers(2);
        }
    }

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
                        PassengerInfo.Add(new PedestrianData(PassengerSprites[1], "Coin", currentPassengers));
                        currentPassengers++;
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
                    break;

                case Role.Stink:
                    //Trigger animations for streetcar and UI.
                    PassengerObjects[currentPassengers].GetComponent<Image>().sprite = PassengerSprites[5];
                    PassengerObjects[currentPassengers].GetComponent<Animator>().SetTrigger("Pulse");
                    streetcarAnimator.SetTrigger("Shrink");

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

                    //Add passenger data.
                    PassengerInfo.Add(new PedestrianData(PassengerSprites[5], "Stink", currentPassengers));
                    stinkerNum++;
                    currentPassengers++;
                    break;

                case Role.Inspector:
                    if (currentPassengers < maxPassengers)
                    {
                        //Add ability info.
                        abilities.Add("Speed Boost");
                        UpdateAbilitySpriteOrder();
                        speedBoostUI.text = inspectorNum.ToString();
                        abilityPassengers.Add(Role.Inspector);

                        GetComponent<AudioSource>().clip = pickupSound;
                        GetComponent<AudioSource>().Play();
                        PassengerObjects[currentPassengers].GetComponent<Image>().sprite = PassengerSprites[3];

                        //Add passenger data.
                        PassengerInfo.Add(new PedestrianData(PassengerSprites[3], "Inspector", currentPassengers));
                        inspectorNum++;
                        currentPassengers++;
                    }

                    break;

                case Role.Officer:
                    if (currentPassengers < maxPassengers)
                    {
                        //Add ability info.
                        abilities.Add("Officer");
                        UpdateAbilitySpriteOrder();
                        abilityPassengers.Add(Role.Officer);

                        GetComponent<AudioSource>().clip = pickupSound;
                        GetComponent<AudioSource>().Play();
                        PassengerObjects[currentPassengers].GetComponent<Image>().sprite = PassengerSprites[4];

                        //Add passenger data.
                        PassengerInfo.Add(new PedestrianData(PassengerSprites[4], "Officer", currentPassengers));
                        officerNum++;
                        currentPassengers++;
                    }

                    break;

                case Role.Chunky:
                    if (currentPassengers < maxPassengers)
                    {
                        if (inspectorNum <= 0)
                        {
                            maxSpeed = 0.06f;
                        }
                        else
                        {
                            maxSpeed = 0.1f;
                            acceleration = 0.001f;
                        }

                        GetComponent<AudioSource>().clip = slowSound;
                        GetComponent<AudioSource>().Play();
                        effectsAnimator.SetTrigger("Chunky");
                        PassengerObjects[currentPassengers].GetComponent<Image>().sprite = PassengerSprites[2];

                        //Add passenger data.
                        PassengerInfo.Add(new PedestrianData(PassengerSprites[2], "Chunky", currentPassengers));
                        chunkyNum++;
                        currentPassengers++;
                    }
                    break;
            }

            Destroy(other.gameObject);

            // Strobe capacity panel
            if (scoreMultiplier)
                for (int i = 0; i < currentPassengers; i++)
                    PassengerObjects[i].GetComponent<UIColorStrobe>().StartCoroutine("RecursiveColorChange");

            if (currentPassengers > 0)
                PassengerObjects[currentPassengers - 1].GetComponentInChildren<Animator>().SetTrigger("Pulse");

            streetcarAnimator.SetBool("Full", (currentPassengers == maxPassengers));

            /////////////////

            Debug.Log("Passenger Collision. Info:");
            for (int i = 0; i < PassengerInfo.Count; i++)
                Debug.Log(PassengerInfo[i].role);

            /////////////////

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
        if (chunkyNum > 0)
        {
            maxSpeed = 0.175f;
            acceleration = 0.005f;
            inspectorNum--;
            speedBoostUI.text = inspectorNum.ToString();
            Debug.Log(maxSpeed);
        }
        else
        {
            maxSpeed = 0.15f;
            inspectorNum--;
            speedBoostUI.text = inspectorNum.ToString();
        }

        yield return new WaitForSeconds(2);

        if (chunkyNum > 0)
        {
            maxSpeed = 0.1f;
            acceleration = 0.001f;
        }
        else
        {
            maxSpeed = 0.075f;
            acceleration = 0.001f;
        }
    }

    public float GetMoveSpeed()
    {

        return moveSpeed;
    }

    public void ActivateNextAbility()
    {
        if (abilities.IndexOf("Speed Boost") == 0)
        {
            abilities.Remove("Speed Boost");
            abilityPassengers.RemoveAt(abilityPassengers.Count - 1);

            GetComponent<AudioSource>().clip = speedSound;
            GetComponent<AudioSource>().Play();

            StartCoroutine(speedBoost());
        }
        else if (abilities.IndexOf("Officer") == 0)
        {
            abilities.Remove("Officer");
            abilityPassengers.RemoveAt(abilityPassengers.Count - 1);

            GetComponent<AudioSource>().clip = immuneSound;
            GetComponent<AudioSource>().Play();

            if (maxSpeed < 0.1f) { maxSpeed = 0.1f; }

            Camera.main.GetComponentInChildren<CameraOverlay>().ShowOverlay();
            GameObject[] allPedestrians = GameObject.FindGameObjectsWithTag("Pedestrian");

            foreach (GameObject pedestrianObject in allPedestrians)
            {

                Pedestrian pedestrian = pedestrianObject.GetComponent<Pedestrian>();

                if (pedestrian.GetRole() == Role.Stink || pedestrian.GetRole() == Role.Chunky || pedestrian.GetRole() == Role.Dazer)
                {

                    Destroy(pedestrian.gameObject);
                }
            }

            effectsAnimator.SetTrigger("Norm");
        }

        UpdateAbilitySpriteOrder();
    }

    public void UpdateAbilitySpriteOrder()
    {
        if (abilities.Count.Equals(0))
        {
            leftButtonAnimator.SetTrigger("Normal");
            rightButtonAnimator.SetTrigger("Normal");
            FirstAbilitySprite.sprite = null;
            leftAbilityButton.gameObject.SetActive(false);
            rightAbilityButton.gameObject.SetActive(false);
        }
        else if (abilities.IndexOf("Speed Boost") == 0)
        {
            leftButtonAnimator.SetTrigger("Speed");
            rightButtonAnimator.SetTrigger("Speed");
            FirstAbilitySprite.sprite = abilitiesSprites[0];

            leftAbilityButton.gameObject.SetActive(true);
            rightAbilityButton.gameObject.SetActive(true);
            leftAbilityButton.runtimeAnimatorController = inspectorButtonAnimator;
            rightAbilityButton.runtimeAnimatorController = inspectorButtonAnimator;
        }
        else if (abilities.IndexOf("Officer") == 0)
        {
            leftButtonAnimator.SetTrigger("Police");
            rightButtonAnimator.SetTrigger("Police");
            FirstAbilitySprite.sprite = abilitiesSprites[1];

            leftAbilityButton.gameObject.SetActive(true);
            rightAbilityButton.gameObject.SetActive(true);
            leftAbilityButton.runtimeAnimatorController = policeButtonAnimator;
            rightAbilityButton.runtimeAnimatorController = policeButtonAnimator;
        }

        if (abilities.Count <= 1)
        {
            SecondAbilitySprite.sprite = null;
        }
        else if (abilities.IndexOf("Speed Boost") == 1 || abilities.IndexOf("Speed Boost") == 0 && abilities.LastIndexOf("Speed Boost") == 1)
        {
            SecondAbilitySprite.sprite = abilitiesSprites[0];
            //Debug.Log ("SB1 Trigger");
        }
        else if (abilities.IndexOf("Officer") == 1 || abilities.IndexOf("Officer") == 0 && abilities.LastIndexOf("Officer") == 1)
        {
            SecondAbilitySprite.sprite = abilitiesSprites[1];
            //Debug.Log ("OFF1 Trigger");
        }
    }

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

            //Releases movement key.
            if (Input.GetKeyUp(KeyCode.D))
            {
                accelerating = false;
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                decelerating = false;
            }

            //Use an ability.
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ActivateNextAbility();
            }

            /*  NOT WORKING ATM.
            if (Input.GetKeyDown(KeyCode.Alpha1))       //Num 1, Debug.
            {
                abilities.Add("Speed Boost");
                UpdateAbilitySpriteOrder();
                inspectorCount++;
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))       //Num 2, Debug.
            {
                abilities.Add("Officer");
                UpdateAbilitySpriteOrder();
            }
            */
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

    //Called when dropping off passengers at station, or if a stinker collides with the streetcar.
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
            maxSpeed = 0.1f;
            acceleration = 0.001f;
            effectsAnimator.SetTrigger("Norm");
        }
    }

    public void RemoveInspector()
    {
        inspectorNum--;
        if (inspectorNum <= 0)
        {
            maxSpeed = 0.1f;
            acceleration = 0.001f;
            effectsAnimator.SetTrigger("Norm");
        }
    }

    public void RemoveOfficer()
    {
        officerNum--;
        if (abilities.Count > 0 && abilities[0] == "Officer")
        {
            abilities.Remove("Officer");
            abilityPassengers.RemoveAt(abilityPassengers.Count - 1);
            UpdateAbilitySpriteOrder();
        }
    }

    public void RemoveStinker()
    {
        stinkerNum--;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}