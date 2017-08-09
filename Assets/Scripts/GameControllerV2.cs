using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class GameControllerV2 : MonoBehaviour
{
	[Header("Parameters")]
    [SerializeField] float gameLengthShort;
    [SerializeField] float gameLengthMedium;
    [SerializeField] float gameLengthLong;
    [SerializeField] float hurryUpTime;

    [Header("Countdown Audio")]
    public AudioClip clip1;
    public AudioClip clip2;
    public AudioClip clip3;

    [Header("Scoring")]
    [SerializeField] int winningIncrement;
    [SerializeField] int losingDecrement;
    [SerializeField] int baseShortTarget;
    [SerializeField] int baseMediumTarget;
    [SerializeField] int baseLongTarget;
    [SerializeField] string winnerText;
    [SerializeField] string loserText;

    [Header("Debug")]
    [SerializeField] bool debugActive;
    [SerializeField] bool resetTargetScores;
    [SerializeField] bool extraShortGame;

    //Dynamic References set in InitGame().
    [HideInInspector] public SceneTransition transitionController;
    Streetcar streetcar;
    GameObject timerObj;
    MusicController musicController;

    //Opening and closing animation references.
    GameObject doorsUI;
    Animator doorAnimator;
    GameObject countdownUI;
    AudioSource countdownAudio;
    Image endgameBackground;
    GameObject endgamePanel;
    Text targetScoreText;
    Text playerScoreText;
    Text gameResultText;

    //Game flow variables.
    public enum GameLength { Short, Medium, Long, Random };
    GameLength gameLength;
    float gameTimer;    //Decrementing game timer, ticks.
    float gameTime;     //Length of game, static.
    [HideInInspector] public bool inGameScene = false;  //1
    [HideInInspector] public bool gameRunning = false;  //2
    [HideInInspector] public bool gameOver = false;     //3
    [HideInInspector] public bool gameWon = false;      //4

    //Scoring variables.
    int score = 0;
    int targetScore;
    string targetScoreString;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);

        if (!PlayerPrefs.HasKey("ShortTargetScore"))
        {
            Debug.Log("Creating base target scores in player prefs.");
            ResetTargetScores();
        }
    }

    void Update()
    {
        //What runs while in the game scene.
        if (inGameScene)
        {
            if (gameRunning && gameTimer > 0)
            {
                gameTimer = timerObj.GetComponent<Timer>().GetTime();
                if (gameTimer < hurryUpTime)
                    streetcar.ShowHurryUpText();
            }

            if (gameTimer <= 0 && !gameOver)
            {
                gameRunning = false;
                gameOver = true;
                gameTimer = 0.0f;
                //StartCoroutine("GameOver");
                GameOver();
            }
        }
        //Otherwise monitor when the game scene becomes the active scene.
        else
        {
            string sceneName = SceneManager.GetActiveScene().name;
            switch (sceneName)
            {
                case "Main_Small":
                case "Main_Medium":
                case "Main_Long":
                case "Jacob_Working":
                    Debug.Log("Game Scene Detected.");
                    inGameScene = true;
                    InitGame();
                    break;
                default:
                    break;
            }
        }
    }

    //Called from Update().
    public void InitGame()
    {

        Debug.Log("Running init game in gamecontroller.");

        //Toggle debug in Inspector.
        if (debugActive)
        {
            //Allows tester to drop GM in scene and run.
            gameLength = GameLength.Short;

            if (resetTargetScores)
            {
                Debug.Log("Resetting target scores via debug.");
                ResetTargetScores();
            }
        }
        
        //Set total game time.
        switch (gameLength)
        {
            case GameLength.Short:
                targetScore = PlayerPrefs.GetInt("ShortTargetScore");
                targetScoreString = "ShortTargetScore";
                gameTime = gameLengthShort;
                break;
            case GameLength.Medium:
                targetScore = PlayerPrefs.GetInt("MediumTargetScore");
                targetScoreString = "MediumTargetScore";
                gameTime = gameLengthMedium;
                break;
            case GameLength.Long:
                targetScore = PlayerPrefs.GetInt("LongTargetScore");
                targetScoreString = "LongTargetScore";
                gameTime = gameLengthLong;
                break;
        }

        if (debugActive && extraShortGame)
            gameTime = 10.0f;

        //Set timer.
        gameTimer = gameTime;

        //Set Dynameic References.
        streetcar = GameObject.FindGameObjectWithTag("Streetcar").GetComponent<Streetcar>();
        transitionController = GameObject.FindGameObjectWithTag("SceneController").GetComponent<SceneTransition>();
        transitionController.gameController = this;
        timerObj = GameObject.FindGameObjectWithTag("Timer");
        musicController = GameObject.FindGameObjectWithTag("MusicController").GetComponent<MusicController>();
        doorsUI = GameObject.Find("TransitionDoors");
        doorAnimator = doorsUI.GetComponent<Animator>();
        doorsUI.SetActive(false);
        countdownUI = GameObject.Find("Countdown");
        countdownAudio = countdownUI.GetComponent<AudioSource>();
        countdownUI.SetActive(false);
        endgameBackground = GameObject.Find("EndgameBackground").GetComponent<Image>();
        endgameBackground.gameObject.SetActive(false);
        endgamePanel = GameObject.Find("EndgamePanel");
        gameResultText = endgamePanel.transform.FindChild("WinLossText").GetComponent<Text>();
        targetScoreText = endgamePanel.transform.FindChild("TargetScoreText").GetChild(0).GetComponent<Text>();
        playerScoreText = endgamePanel.transform.FindChild("PlayerScoreText").GetChild(0).GetComponent<Text>();
        endgamePanel.SetActive(false);

        inGameScene = true;
        timerObj.GetComponent<Timer>().InitTimer();
        StartCoroutine("Countdown");
    }

    //Begins the game.
    IEnumerator Countdown()
    {
        //Open doors animation.
        doorsUI.SetActive(true);
        doorAnimator.Play("transitionOpen");
        yield return new WaitForSeconds(1.0f);
        doorsUI.SetActive(false);

        //Play audio and animations of countdown.
        countdownUI.SetActive(true);
        countdownAudio.clip = clip3;
        countdownAudio.Play();
        yield return new WaitForSeconds(1.0f);
        countdownAudio.clip = clip2;
        countdownAudio.Play();
        yield return new WaitForSeconds(1.0f);
        countdownAudio.clip = clip1;
        countdownAudio.Play();
        yield return new WaitForSeconds(1.0f);
        musicController.PlayRegularMusic();

        Debug.Log("Game Started in GameControllerV2");
        gameRunning = true;
        timerObj.GetComponent<Timer>().StartCoroutine("Tick");
    }

    //Determine win condition, display results, and save player prefs.
    void GameOver()
    {
        if (score >= targetScore)
            gameWon = true;
        else
            gameWon = false;

        //Display endgame panel.
        endgameBackground.gameObject.SetActive(true);
        endgamePanel.SetActive(true);
        targetScoreText.text = targetScore.ToString();
        playerScoreText.text = score.ToString();

        //Save new score target.
        if (gameWon)
        {
            gameResultText.text = winnerText;
            playerScoreText.color = Color.green;
            PlayerPrefs.SetInt(targetScoreString, targetScore + winningIncrement);
        }
        else
        {
            gameResultText.text = loserText;
            playerScoreText.color = Color.red;
            PlayerPrefs.SetInt(targetScoreString, targetScore - losingDecrement);
        }

        PlayerPrefs.Save();
    }

    //To replay the same level again, reset all the pieces and run InitGame().
    public void ReplayLevel()
    {
        Debug.Log("Replay Level.");
    }

    //To regen the same size level differently, just reset the GM (since its persistent) and reload the scene.
    public void RegenLevel()
    {
        //I really thought there would be more to be done here. I planned well.
        gameOver = false;
        score = 0;
        Debug.Log("Regen Level.");
    }

    #region Getters & Setters

    public bool GetGameRunning()
    {
        return gameRunning;
    }

    public float GetTimeRemaining()
    {
        return gameTimer;
    }

    public float GetGameLength()
    {
        return gameTime;
    }

    public int GetScore()
    {
        return score;
    }

    public void IncrementScore(int scoreAddition)
    {
        score += scoreAddition;
    }

    //Called from SceneTransition.cs
    public void LaunchGame(string length)
    {
        switch (length)
        {
            case "short":
                gameLength = GameLength.Short;
                break;

            case "medium":
                gameLength = GameLength.Medium;
                break;

            case "long":
                gameLength = GameLength.Long;
                break;
        }

        Debug.Log("Game Time set, passing to scenetransition.");

        transitionController.TransitionToGame(length);
    }

    private void ResetTargetScores()
    {
        PlayerPrefs.SetInt("ShortTargetScore", baseShortTarget);
        PlayerPrefs.SetInt("MediumTargetScore", baseMediumTarget);
        PlayerPrefs.SetInt("LongTargetScore", baseLongTarget);
        PlayerPrefs.Save();
    }
    #endregion
}
