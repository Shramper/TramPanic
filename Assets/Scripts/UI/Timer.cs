using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Sprite[] seconds;
    public Sprite[] tens;
    public Sprite[] minutes;

    float gameLength;

    public GameObject secs;
    public GameObject mins;
    public GameObject tenths;
    public GameObject colon;

    private Image secImage;
    private Image tenthImage;
    private Image minImage;

    //public int secCount = 10;
    //public int tenCount = 6;
    //public int minCount = 2;
    [SerializeField]
    private int gameTimeInSec = 120;
    public float delayTime = 5;

    bool isBlinking = false;

    public GameObject TimerUI;

    private IEnumerator timer;

    void Awake()
    {
        gameLength = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerV2>().GetGameLength();
        StartCoroutine(Delay());
        secImage = secs.GetComponent<Image>();
        tenthImage = tenths.GetComponent<Image>();
        minImage = mins.GetComponent<Image>();
        timer = secondCounter();
    }

    public float delay()
    {
        return delayTime;
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(delayTime);
        StartCoroutine(timer);
    }

    private IEnumerator secondCounter()
    {
        while (gameLength > 0)
        {
            gameLength -= 1;

            secImage.sprite = seconds[Mathf.Clamp(Mathf.FloorToInt(gameLength) % 10, 0, seconds.Length)];
            tenthImage.sprite = seconds[Mathf.Clamp(Mathf.FloorToInt(gameLength/10) % 6, 0, seconds.Length)];
            minImage.sprite = seconds[Mathf.Clamp(Mathf.FloorToInt(gameLength/60) % 10, 0, seconds.Length)];

            if (gameLength <= 10 && !isBlinking)
            {
                StartCoroutine(BlinkTime());
            }

            yield return new WaitForSeconds(1);
        }
    }
    
    IEnumerator BlinkTime()
    {
        secs.SetActive(!secs.activeSelf);
        mins.SetActive(!mins.activeSelf);
        tenths.SetActive(!tenths.activeSelf);
        colon.SetActive(!colon.activeSelf);
        yield return new WaitForSeconds(0.5f);
        secs.SetActive(!secs.activeSelf);
        mins.SetActive(!mins.activeSelf);
        tenths.SetActive(!tenths.activeSelf);
        colon.SetActive(!colon.activeSelf);
    }
}

