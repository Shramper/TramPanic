using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class GameControllerV2 : MonoBehaviour
{
	[Header("Parameters")]
    [SerializeField] float gameLengthShort;
    [SerializeField] float gameLengthMedium;
    [SerializeField] float gameLengthLong;
    [SerializeField] float delayTime;
    [SerializeField] float hurryUpTime;

    //Dynamic References set in InitGame().
    Streetcar streetcar;
    GameObject timerObj;



    public enum GameLength { Short, Medium, Long };
    private GameLength gameLength;

    float gameTimer;
    float gameTime;
    int score = 0;
    bool gameRunning = false;

    private void Awake()
    {
        InitGame();   
    }

    //Called from SetGameLength().
    void InitGame()
    {
        gameLength = GameLength.Short;
        
        //Set total game time.
        switch (gameLength)
        {
            case GameLength.Short:
                gameTime = gameLengthShort;
                break;
            case GameLength.Medium:
                gameTime = gameLengthMedium;
                break;
            case GameLength.Long:
                gameTime = gameLengthLong;
                break;
        }

        //Set timer.
        gameTimer = gameTime;

        //Set Dynameic References.
        streetcar = GameObject.FindGameObjectWithTag("Streetcar").GetComponent<Streetcar>();
        timerObj = GameObject.FindGameObjectWithTag("Timer");

    }
	
	void Update ()
    {
        if (gameRunning && gameTimer > 0)
        {
            gameTimer -= Time.deltaTime;
            if (gameTimer < hurryUpTime)
                streetcar.ShowHurryUpText();
        }

        if (gameTimer <= 0)
            GameOver();
    }

    void GameOver()
    {

    }

    //Called from CountdownTimer.cs
    public void StartGame()
    {
        Debug.Log("Game Started in GameControllerV2");
        gameRunning = true;
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

    //Called from Main Menu Buttons.
    public void SetGameLength(GameLength length)
    {
        gameLength = length;
        InitGame();
    }

    #endregion
}
