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
    bool gameStart = false;

    private void Awake()
    {
        InitGame();   
    }

    //Called from SetGameLength().
    void InitGame()
    {
        gameLength = GameLength.Long;
        
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
        if (gameStart && gameTimer > 0)
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
        gameStart = true;
    }

    #region Getters & Setters

    public bool GetGameStarted()
    {
        return gameStart;
    }

    public float GetTimeRemaining()
    {
        return gameTimer;
    }

    public float GetGameLength()
    {
        return gameTime;
    }

    //Called from Main Menu Buttons.
    public void SetGameLength(GameLength length)
    {
        gameLength = length;
        InitGame();
    }

    #endregion
}
