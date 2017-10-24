using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class ColorStrobe : MonoBehaviour {

	SpriteRenderer spriteRenderer;

	void Awake () {

		spriteRenderer = this.GetComponent<SpriteRenderer> ();
	}

	//public IEnumerator RecursiveColorChange () {
    //
	//	spriteRenderer.color = new Color (Random.Range (0, 1f), Random.Range (0, 1f), Random.Range (0, 1f));
    //
	//	yield return new WaitForSeconds (0.1f);
    //
	//	StartCoroutine (RecursiveColorChange ());
	//}
}
