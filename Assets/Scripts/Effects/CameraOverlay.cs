using UnityEngine;
using System.Collections;

public class CameraOverlay : MonoBehaviour {

	[SerializeField] float fadeSpeed;

	SpriteRenderer spriteRenderer;

	void Awake () {

		spriteRenderer = this.GetComponent<SpriteRenderer>();
	}

	void Update () {

		float alpha = spriteRenderer.color.a;

		if(alpha > 0) {
			
			alpha -= fadeSpeed * Time.deltaTime;
			spriteRenderer.color = new Color(1, 1, 1, alpha);
		}
	}

	public void ShowOverlay () {

		spriteRenderer.color = Color.white;
	}
}
