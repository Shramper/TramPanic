using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class UIColorStrobe : MonoBehaviour {

    float timeElapsed = 0.0f;
    float shieldTimeElapsed = 0.0f;
    float waitTime = 0.1f;
    int counter = 0;
	Image image;
    Color[] colors = {Color.red, Color.white, Color.blue};


	void Awake () {

		image = GetComponent<Image> ();
	}

	public IEnumerator RecursiveColorChange (float time)
    {
        while (timeElapsed < time)
        {
            image.color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
            yield return new WaitForSeconds(waitTime);
            timeElapsed += waitTime;
        }

        timeElapsed = 0.0f;
	}
    public IEnumerator ShieldColorChange(float time)
    {
        while (shieldTimeElapsed < time)
        {
            image.color = colors[counter++ % colors.Length];
            yield return new WaitForSeconds(waitTime);
            shieldTimeElapsed += waitTime;
        }

        shieldTimeElapsed = 0.0f;
    }

    public IEnumerator StinkerColorChange(float time)
    {
        Color thisColor = new Color(0.23f, 0.75f, 0.1f, 1.0f);
        float timeElapsed = 0;
        while (timeElapsed < time)
        {
            thisColor.a = (time - timeElapsed) / time;
            image.color = thisColor;
            yield return new WaitForSeconds(waitTime);
            timeElapsed += waitTime;
        }

        image.color = Color.white;
    }

}
