using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class LeaderboardController : MonoBehaviour {

    const int leaderboardEntryCount = 10;

    [SerializeField] GameObject leaderboardPanel;
    [SerializeField] Text finalScoreText;
    [SerializeField] GameObject nameEntryPanel;
    [SerializeField] GameObject endButtonsObject;

    [SerializeField] Text[] scoreTextArray;
    [SerializeField] Text[] nameTextArray;
    [SerializeField] Text[] timeTextArray;
    
    [SerializeField] GameObject leftArrowObject;
    [SerializeField] GameObject rightArrowObject;
    [SerializeField] Animator transitionDoorsAnimator;
    GameControllerV2 gameController;

    private int[] scoreArray = new int[leaderboardEntryCount];
    private string[] nameArray = new string[leaderboardEntryCount];
    private string[] dirNameArray = new string[] { " Northbound", " Southbound", " Westbound", " Eastbound"};

	Animator leaderboardAnimator;
	private int finalScore = 0;
	private int indexOfNewHighScore;
	private string newName;

	private void Start ()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerV2>();

        LoadPlayerPrefs();
		UpdateLeaderboardText();
		leaderboardAnimator = GetComponentInChildren<Animator>();
		finalScoreText.gameObject.SetActive(false);
		nameEntryPanel.SetActive(false);
	}

	private void CheckIfNewHighScore ()
    {
		int lowestLeaderboardScore = scoreArray[leaderboardEntryCount - 1];
		if(finalScore > lowestLeaderboardScore)
        {
			Debug.Log("Adding new high score");
			nameEntryPanel.SetActive(true);
			AddNewHighScore();
		}
		else
        {
			endButtonsObject.GetComponent<Animator>().SetTrigger("SlideIn");
		}
	}

    private void AddNewHighScore ()
    {
		indexOfNewHighScore = -1;

        // Checking every high score entry 
        for (int i = 0; i < leaderboardEntryCount; i++)
        {
            if (indexOfNewHighScore < 0 && finalScore > scoreArray[i])
            {
                indexOfNewHighScore = i; // Mark the new entry location
                break; // Stop checking
            }
        }

        // If there is a highscore
        if (indexOfNewHighScore >= 0)
        {
            // Make space for it
            for(int i = leaderboardEntryCount - 1; i > indexOfNewHighScore; i--)
            {
            	scoreArray[i] = scoreArray[i - 1];
            	nameArray[i] = nameArray[i - 1];
            }

            // Enter new highscore data
            scoreArray[indexOfNewHighScore] = finalScore;
            nameArray[indexOfNewHighScore] = newName;
            SaveToPlayerPrefs();
            UpdateLeaderboardText();
        }
	}

    private void LoadPlayerPrefs()
    {
        for (int i = 0; i < leaderboardEntryCount; i++)
        {
            scoreArray[i] = PlayerPrefs.GetInt("LeaderboardScore" + i);
            nameArray[i] = PlayerPrefs.GetString("LeaderboardName" + i);
        }
    }

    private void SaveToPlayerPrefs ()
    {
		for(int i = 0; i < leaderboardEntryCount; i++)
        {
			PlayerPrefs.SetInt("LeaderboardScore" + i, scoreArray[i]);
			PlayerPrefs.SetString("LeaderboardName" + i, nameArray[i]);
		}
	}

    private void UpdateLeaderboardText ()
    {
		for(int i = 0; i < leaderboardEntryCount; i++)
        {
			scoreTextArray[i].text = scoreArray[i].ToString();
			nameTextArray[i].text = nameArray[i];
		}
	}

    public void OpenLeaderboard ()
    {
		leftArrowObject.SetActive(false);
		rightArrowObject.SetActive(false);
		leaderboardPanel.SetActive(true);
		leaderboardAnimator.SetTrigger("SlideIn");
        StartCoroutine(DelayCheckIfNewHighScore());
    }

    private void SaveNewName ()
    {
		newName = nameEntryPanel.GetComponentInChildren<InputField>().text;
        newName += dirNameArray[Random.Range(0, dirNameArray.Length)];

		if(indexOfNewHighScore != -1) { nameArray[indexOfNewHighScore] = newName; }
		nameEntryPanel.SetActive(false);
		UpdateLeaderboardText();
		SaveToPlayerPrefs();
		endButtonsObject.GetComponent<Animator>().SetTrigger("SlideIn");
	}

	public void ResetLeaderboard ()
    {
		scoreArray = new int[leaderboardEntryCount];
		nameArray = new string[leaderboardEntryCount];
		for(int i = 0; i < leaderboardEntryCount; i++)
        {
			scoreArray[i] = 0;
			nameArray[i] = "";
		}
		SaveToPlayerPrefs();
	}

	public void RestartGame ()
    {
		StartCoroutine(RestartGameSequence());
	}

	public void QuitGame ()
    {
		SceneManager.LoadScene(1);
	}

    private IEnumerator RestartGameSequence()
    {
        transitionDoorsAnimator.SetTrigger("Close");

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator DelayCheckIfNewHighScore()
    {
        finalScoreText.gameObject.SetActive(true);
        finalScoreText.gameObject.GetComponent<Animator>().SetTrigger("SlideIn");

        yield return new WaitForSeconds(0.75f);

        finalScore = gameController.GetScore();
        finalScoreText.text = "Final Score\n" + finalScore;

        CheckIfNewHighScore();
    }
}
