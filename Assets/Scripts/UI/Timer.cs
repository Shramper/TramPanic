using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

	public Sprite[] seconds;
	public Sprite[] tens;
	public Sprite[] minutes;

    float gameLength;
	float delayTimer = 6;

	public float secTimer = 9;
	public float tensTimer = 0;
	public float minTimer = 0;

	public GameObject secs;
	public GameObject mins;
	public GameObject tenths;
    public GameObject colon;

    public int secCount = 10;
    public int tenCount = 6;
	public int minCount = 4;

    bool isBlinking = false;

	public GameObject TimerUI;

    void Awake()
    {
        StartCoroutine(Delay());
        gameLength = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().GetGameLength();
    }
	void Update ()
    {
        gameLength -= Time.deltaTime;
        if (gameLength <= 10 && !isBlinking)
        {
            isBlinking = true;
            StartCoroutine(BlinkTime());
        }
    }
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(5);
        StartCoroutine(Seconds());
        StartCoroutine(Tens());
        StartCoroutine(Minutes());
    }
    IEnumerator Seconds()
    {
        if (secCount <= 0)
        {
            secCount = 10;
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
        }
        tenCount -= 1;
        tenths.transform.GetComponent<Image>().sprite = tens[tenCount];
        yield return new WaitForSeconds(10);
        StartCoroutine(Tens());
    }
    IEnumerator Minutes()
    {
        if (minCount <= 0)
        {
            StopAllCoroutines();
        }
        minCount -= 1;
        mins.transform.GetComponent<Image>().sprite = minutes[minCount];
        yield return new WaitForSeconds(60);
        StartCoroutine(Minutes());
    }
    IEnumerator BlinkTime ()
    {
        secs.SetActive(!secs.activeSelf);
        mins.SetActive(!mins.activeSelf);
        tenths.SetActive(!tenths.activeSelf);
        colon.SetActive(!colon.activeSelf);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(BlinkTime());
    }
}
	