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

    [Header("References")]
    public Streetcar streetcar;
    public GameObject timerObj;

    //Hidden from Inspector
    float gameTimer;
    float gameLength;
    bool gameStart = false;

    void Awake()
    {
        switch (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
        {
            case "Main_Small":
                gameTimer = gameLengthShort;
                break;
            case "Main_Medium":
                gameTimer = gameLengthMedium;
                break;
            case "Main_Large":
                gameTimer = gameLengthLong;
                break;
        }

        gameLength = gameTimer;
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

    public bool GameStarted()
    {
        return gameStart;
    }

    public float TimeRemaining()
    {
        return gameTimer;
    }

    public float GetGameLength()
    {
        return gameLength;
    }
}
