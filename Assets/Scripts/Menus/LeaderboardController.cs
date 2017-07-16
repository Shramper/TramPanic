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

	[SerializeField] Streetcar streetcar;
	[SerializeField] GameObject leftArrowObject;
	[SerializeField] GameObject rightArrowObject;
	[SerializeField] Animator transitionDoorsAnimator;

	int[] scoreArray = new int[leaderboardEntryCount];
	string[] nameArray = new string[leaderboardEntryCount];

	Animator leaderboardAnimator;
	int finalScore = 0;
	int indexOfNewHighScore;
	string newName;

	void Start ()
    {
		LoadPlayerPrefs();
		UpdateLeaderboardText();
		leaderboardAnimator = GetComponentInChildren<Animator>();
		finalScoreText.gameObject.SetActive(false);
		nameEntryPanel.SetActive(false);
	}

	void CheckIfNewHighScore ()
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

	void AddNewHighScore ()
    {
		indexOfNewHighScore = -1;
		// Cycle through highscores to locate appropriate location for placement
		if(finalScore > scoreArray[0])
        {
			indexOfNewHighScore = 0;
		}
		else
        {
			for(int i = leaderboardEntryCount - 1; i > 0; i--)
            {
				Debug.Log("Checking " + i);
				if(finalScore > scoreArray[i] && finalScore <= scoreArray[i - 1])
                {
					indexOfNewHighScore = i;
					break;
				}
			}
		}
		// Shift array entries to make room for new high score entries
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

    void LoadPlayerPrefs()
    {
        for (int i = 0; i < leaderboardEntryCount; i++)
        {
            scoreArray[i] = PlayerPrefs.GetInt("LeaderboardScore" + i);
            nameArray[i] = PlayerPrefs.GetString("LeaderboardName" + i);
        }
    }

	void SaveToPlayerPrefs ()
    {
		for(int i = 0; i < leaderboardEntryCount; i++)
        {
			PlayerPrefs.SetInt("LeaderboardScore" + i, scoreArray[i]);
			PlayerPrefs.SetString("LeaderboardName" + i, nameArray[i]);
		}
	}

	void UpdateLeaderboardText ()
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

	public void SaveNewName ()
    {
		newName = nameEntryPanel.GetComponentInChildren<InputField>().text;

		float randomValue = Random.value;
		if(randomValue < 0.25f) { newName += " Northbound"; }
		else if(randomValue < 0.5f) { newName += " Southbound"; }
		else if(randomValue < 0.75f) { newName += " Eastbound"; }
		else { newName += " Westbound"; }

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

    IEnumerator RestartGameSequence()
    {
        transitionDoorsAnimator.SetTrigger("Close");

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator DelayCheckIfNewHighScore()
    {
        finalScoreText.gameObject.SetActive(true);
        finalScoreText.gameObject.GetComponent<Animator>().SetTrigger("SlideIn");

        yield return new WaitForSeconds(0.75f);

        finalScore = streetcar.GetScore();
        finalScoreText.text = "Final Score\n" + finalScore;

        CheckIfNewHighScore();
    }
}
