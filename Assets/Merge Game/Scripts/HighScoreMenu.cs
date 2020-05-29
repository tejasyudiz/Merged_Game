using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HighScoreMenu : MonoBehaviour {

	

	public Text initials1, initials2, initials3, initials4, initials5;
	public Text highScore1, highScore2, highScore3, highScore4, highScore5;

	void OnEnable() {
		SetScores ();
	}

	
	public void SetScores()
	{		
		initials1.text = PlayerPrefs.GetString ("Initials1");
		initials2.text = PlayerPrefs.GetString ("Initials2");
		initials3.text = PlayerPrefs.GetString ("Initials3");
		initials4.text = PlayerPrefs.GetString ("Initials4");
		initials5.text = PlayerPrefs.GetString ("Initials5");
		highScore1.text = ""+PlayerPrefs.GetInt ("HighScore1").ToString("n0");
		highScore2.text = ""+PlayerPrefs.GetInt ("HighScore2").ToString("n0");
		highScore3.text = ""+PlayerPrefs.GetInt ("HighScore3").ToString("n0");
		highScore4.text = ""+PlayerPrefs.GetInt ("HighScore4").ToString("n0");
		highScore5.text = ""+PlayerPrefs.GetInt ("HighScore5").ToString("n0");
	}
}
