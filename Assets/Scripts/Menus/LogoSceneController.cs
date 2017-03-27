using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[DisallowMultipleComponent]
public class LogoSceneController : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] float fadeSpeed = 0.1f;

	[Header("References")]
	[SerializeField] SpriteRenderer cameraOverlay;

	void Start () {

		StartCoroutine (FadeScreen ());
	}

	IEnumerator FadeScreen () {

		cameraOverlay.gameObject.SetActive (true);
		cameraOverlay.color = Color.black;

		yield return new WaitForSeconds (1);

		// Fade in logo
		while (cameraOverlay.color.a > 0) {

			float newAlpha = cameraOverlay.color.a - fadeSpeed * Time.deltaTime;
			cameraOverlay.color = new Color (0, 0, 0, newAlpha);
			yield return null;
		}

		// Pause
		yield return new WaitForSeconds (1);

		// Fade out logo
		while (cameraOverlay.color.a < 1) {

			float newAlpha = cameraOverlay.color.a + fadeSpeed * Time.deltaTime;
			cameraOverlay.color = new Color (0, 0, 0, newAlpha);
			yield return null;
		}

		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex + 1);
	}
}