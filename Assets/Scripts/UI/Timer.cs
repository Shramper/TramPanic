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

    public int secCount = 10;
    public int tenCount = 6;
    public int minCount = 2;
    public float delayTime = 5;

    bool isBlinking = false;

    public GameObject TimerUI;

    void Awake()
    {
        gameLength = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().GetGameLength();
        StartCoroutine(Delay());
    }
    void Update()
    {
        gameLength -= Time.deltaTime;
        if (gameLength <= 10 && !isBlinking)
        {
            isBlinking = true;
        }
    }
    public float delay()
    {
        return delayTime;
    }
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(delayTime);
        StartCoroutine(Seconds());
        StartCoroutine(Tens());
        StartCoroutine(Minutes());
    }
    IEnumerator Seconds()
    {
        if (isBlinking == true)
        {
            StartCoroutine(BlinkTime());
        }
        if (secCount <= 0)
        {
            secCount = 10;
            StartCoroutine(Tens());
        }
        secCount -= 1;
        secs.transform.GetComponent<Image>().sprite = seconds[secCount];
        yield return new WaitForSeconds(1);
        StartCoroutine(Seconds());
    }
    IEnumerator Tens()
    {
        if (tenCount <= 0)
        {
            tenCount = 6;
            StartCoroutine(Minutes());
        }
        tenCount -= 1;
        tenths.transform.GetComponent<Image>().sprite = tens[tenCount];
        yield return new WaitForSeconds(0);
    }
    IEnumerator Minutes()
    {
        minCount -= 1;
        mins.transform.GetComponent<Image>().sprite = minutes[minCount];
        yield return new WaitForSeconds(0);

        if (minCount <= 0)
        {
            secCount = 10;
            tenCount = 6;
            minCount = 2;
            StopAllCoroutines();
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

