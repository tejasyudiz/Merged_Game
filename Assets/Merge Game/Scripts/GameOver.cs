using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameOver : MonoBehaviour {

	
	public Text playerScore;
	public Button restartButton, quitButton;
	private int score;
	private string playerInitials;
	private TouchScreenKeyboard keyBoard;
	private bool playerHasANewHighScore = false;

	void Awake()
	{
		GameIsOver ();
	}

	
	public void GameIsOver()
	{
		playerHasANewHighScore = false;
		Debug.Log(GameStats.Instance.PrintGameStats ());

		score = (GameStats.Instance.GetPlayerScore());

		if(score > PlayerPrefs.GetInt("HighScore5"))
		{
			playerHasANewHighScore = true;
		}
		playerScore.text = " ";
	}

	
	public void ShowKeyBoard(bool isRestarting)
	{
		playerScore.text = "Please Wait...";
		keyBoard = TouchScreenKeyboard.Open("");
		StartCoroutine(CheckStatus(isRestarting));
	}


	private IEnumerator CheckStatus(bool isRestarting)
	{
		while (keyBoard != null)
		{
			if (keyBoard.done)
			{
				AddingNameAndScoreToHighScore(keyBoard.text, isRestarting);
				keyBoard = null;
			}
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForEndOfFrame();
	}

	
	private void AddingNameAndScoreToHighScore(string name, bool isRestarting)
	{
		string newName = string.Empty;
		if (name.Length >= 3)
			newName = name.Substring (0, 3);
		else
			newName = name;
		int newScore = score;
		int oldScore;
		string oldName;

		//Update the Highscore values
		for(int i = 1; i < 6; i++)
		{
			if(PlayerPrefs.HasKey("HighScore"+i))
			{
				if(PlayerPrefs.GetInt("HighScore"+i) < newScore)
				{
					oldScore = PlayerPrefs.GetInt("HighScore"+i);
					oldName = PlayerPrefs.GetString("Initials"+i);
					PlayerPrefs.SetInt("HighScore"+i,newScore);
					PlayerPrefs.SetString("Initials"+i,newName);
					newScore = oldScore;
					newName = oldName;
				}
			}
		}

		playerHasANewHighScore = false;

		if(isRestarting)
		{
			Restart();
		}
		else
		{
			ProcessQuit();
		}
	}

	 
	public void Restart()
	{
		if (!playerHasANewHighScore)
		{
			SoundManager.Instance.PlaySoundEffect(SoundManager.ITEM_SELECT, true);
			RestartGame ();
		}
		else
		{
			SoundManager.Instance.PlaySoundEffect(SoundManager.ITEM_SELECT, true);
			restartButton.interactable = false;
			quitButton.interactable = false;
			ShowKeyBoard(true);
		}
	}


	public void RestartGame()
	{	
		GameStats.Instance.ResetPlayerScore ();
		SceneManager.LoadScene ("GameScene");
	}

	
	public void ProcessQuit()
	{
		if (!playerHasANewHighScore)
		{
			SoundManager.Instance.PlaySoundEffect (SoundManager.BACK_BUTTON, true);
			GameStats.Instance.ResetPlayerScore ();
			BackToMainMenu ();
		}
		else
		{
			SoundManager.Instance.PlaySoundEffect(SoundManager.BACK_BUTTON, true);
			restartButton.interactable = false;
			quitButton.interactable = false;
			ShowKeyBoard(false);
		}
	}

	
	public void BackToMainMenu()
	{	
		SoundManager.Instance.PlaySoundEffect (SoundManager.BACK_BUTTON, true);
		SceneManager.LoadSceneAsync ("MainMenu");
	}
}
