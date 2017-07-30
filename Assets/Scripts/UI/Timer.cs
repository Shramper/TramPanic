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
    
    public float delayTime;
    public float blinkTime;
    float gameLength;

    void Start()
    {
        gameLength = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerV2>().GetGameLength();

        Debug.Log("Game Length from Timer: " + gameLength.ToString());

        secsImage.sprite = clockNumbers[Mathf.Clamp(Mathf.FloorToInt(gameLength) % 10, 0, clockNumbers.Length)];
        tensImage.sprite = clockNumbers[Mathf.Clamp(Mathf.FloorToInt(gameLength / 10) % 6, 0, clockNumbers.Length)];
        minsImage.sprite = clockNumbers[Mathf.Clamp(Mathf.FloorToInt(gameLength / 60) % 10, 0, clockNumbers.Length)];

        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(delayTime);
        StartCoroutine("Tick");
    }

    private IEnumerator Tick()
    {
        while (gameLength > 0)
        {
            secsImage.sprite = clockNumbers[Mathf.Clamp(Mathf.FloorToInt(gameLength) % 10, 0, clockNumbers.Length)];
            tensImage.sprite = clockNumbers[Mathf.Clamp(Mathf.FloorToInt(gameLength/10) % 6, 0, clockNumbers.Length)];
            minsImage.sprite = clockNumbers[Mathf.Clamp(Mathf.FloorToInt(gameLength/60) % 10, 0, clockNumbers.Length)];

            if (gameLength <= blinkTime)
                StartCoroutine("BlinkTimerFace");

            yield return new WaitForSeconds(1);
            gameLength -= 1;
        }
    }
    
    IEnumerator BlinkTimerFace()
    {
        timerFace.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        timerFace.SetActive(true);
    }

    public float delay()
    {
        return delayTime;
    }
}

