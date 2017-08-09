using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    //External References.
    public GameObject timerFace;
    public Sprite[] clockNumbers;
    public Image minsImage;
    public Image tensImage;
    public Image secsImage;
    
    public float blinkTime;
    float gameLength;

    public void InitTimer()
    {
        gameLength = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerV2>().GetGameLength();

        secsImage.sprite = clockNumbers[Mathf.Clamp(Mathf.FloorToInt(gameLength) % 10, 0, clockNumbers.Length)];
        tensImage.sprite = clockNumbers[Mathf.Clamp(Mathf.FloorToInt(gameLength / 10) % 6, 0, clockNumbers.Length)];
        minsImage.sprite = clockNumbers[Mathf.Clamp(Mathf.FloorToInt(gameLength / 60) % 10, 0, clockNumbers.Length)];
    }

    public IEnumerator Tick()
    {
        gameLength -= 1;

        secsImage.sprite = clockNumbers[Mathf.Clamp(Mathf.FloorToInt(gameLength) % 10, 0, clockNumbers.Length)];
        tensImage.sprite = clockNumbers[Mathf.Clamp(Mathf.FloorToInt(gameLength/10) % 6, 0, clockNumbers.Length)];
        minsImage.sprite = clockNumbers[Mathf.Clamp(Mathf.FloorToInt(gameLength/60) % 10, 0, clockNumbers.Length)];

        if (gameLength <= blinkTime && gameLength > 0)
            StartCoroutine("BlinkTimerFace");

        yield return new WaitForSeconds(1);

        if (gameLength >= 0)
            StartCoroutine("Tick");
    }

    IEnumerator BlinkTimerFace()
    {
        for (int i = 0; i < timerFace.transform.childCount; i++)
            timerFace.transform.GetChild(i).gameObject.SetActive(false);

        yield return new WaitForSeconds(0.25f);

        for (int i = 0; i < timerFace.transform.childCount; i++)
            timerFace.transform.GetChild(i).gameObject.SetActive(true);
    }

    public float GetTime()
    {
        return gameLength;
    }
}

