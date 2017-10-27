using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public enum GameLength { Short, Medium, Long, Random };

    [Header ("Scene Names")]
    [SerializeField] string shortScene;
    [SerializeField] string mediumScene;
    [SerializeField] string longScene;
    [SerializeField] string randomScene;
    [SerializeField] string menuScene;
    [SerializeField]
    private Scene jacobTestScene;

    [Header("References")]
    public GameObject gameControllerObj;
    public GameObject doorsUI;
	public Animator doorsAnim;

    [HideInInspector] public GameControllerV2 gameController;
    string sceneName;
    bool regen = false;

    public void CreateGameManager(string gameLength)
    {
        GameObject gm = Instantiate(gameControllerObj) as GameObject;
        gameController = gm.GetComponent<GameControllerV2>();
        gameController.transitionController = this;

        Debug.Log("GameManager Created.");

        gameController.LaunchGame(gameLength);
    }

	public void TransitionToGame(string gamelength)
    {
        switch (gamelength)
        {
            case "short":
                sceneName = shortScene;
                break;

            case "medium":
                sceneName = mediumScene;
                break;

            case "long":
                sceneName = longScene;
                break;

            case "random":
                sceneName = randomScene;
                break;
        }

        doorsUI.SetActive(true);
        doorsAnim.Play("transitionClose");

        Debug.Log("scene selected, passing to delay.");

        StartCoroutine("Delay");
	}

    public void TransitionToMenu()
    {
        sceneName = menuScene;
        doorsUI.SetActive(true);
        doorsAnim.Play("transitionClose");
        StartCoroutine("Delay");
        Destroy(gameController.gameObject);
    }

    public void ReplayLevel()
    {
        //if (SceneManager.GetActiveScene().name == randomScene)
            gameController.ReplayLevel();
        //else
        //    RegenLevel();
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.J))
    //    {
    //        Debug.Log("You hit J!");
    //        SceneManager.LoadScene("Jacob_Working");
    //    }
    //}

    public void RegenLevel()
    {
        regen = true;
        sceneName = SceneManager.GetActiveScene().name;
        doorsUI.SetActive(true);
        doorsAnim.Play("transitionClose");
        StartCoroutine("Delay");
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(2.0f);
        if (regen)
        {
            gameController.RegenLevel();
            gameController.inGameScene = false;
        }
        SceneManager.LoadScene(sceneName);
    }

    //Called from objective panel start button.
    public void BeginGame()
    {
        gameController.StartCoroutine("Countdown");
    }
}
