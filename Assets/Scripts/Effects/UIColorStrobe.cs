using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class UIColorStrobe : MonoBehaviour {

	Image image;

	void Awake () {

		image = this.GetComponent<Image> ();
	}

	public IEnumerator RecursiveColorChange () {

		image.color = new Color (Random.Range (0, 1f), Random.Range (0, 1f), Random.Range (0, 1f));

		yield return new WaitForSeconds (0.1f);

		StartCoroutine (RecursiveColorChange ());
	}
}
