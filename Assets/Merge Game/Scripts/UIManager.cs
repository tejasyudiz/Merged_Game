using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {
	
	

	public Text scoreText;
	public Image centerBox;
	public GameObject boxHolder;
	public GameObject gameOverPanel;
	public GameObject optionsPanel;

	void Start ()
	{

	
		scoreText.text = "0";

	
		optionsPanel.SetActive (false);
		HideGameOverPanel ();

	}

	
	public void SetScore (int score)
	{
		scoreText.text = string.Format ("{0:n0}", score);
	}

	public void ShowOneBox()
	{	
		boxHolder.SetActive(false);
		centerBox.gameObject.SetActive (true);
	}

	public void ShowTwoBoxes()
	{	
		boxHolder.SetActive(true);
		centerBox.gameObject.SetActive (false);
	}

	public void RotateBoxes() {

		boxHolder.transform.Rotate (0, 0, -90);
	}

	 
	public void ResetBoxes()
	{	
		boxHolder.transform.localEulerAngles = new Vector3 (0, 0, 0);
	}

 
	public void HideInGameOptionsMenu() {

		SoundManager.Instance.PlaySoundEffect (SoundManager.BACK_BUTTON, true);
		optionsPanel.SetActive (false);
	}

	public void ShowInGameOptionsMenu() {

		SoundManager.Instance.PlaySoundEffect (SoundManager.ITEM_SELECT, true);
		optionsPanel.SetActive (true);
	}

	
	public void BackToMainMenu() {

		SoundManager.Instance.PlaySoundEffect (SoundManager.BACK_BUTTON, true);
		GameStats.Instance.ResetPlayerScore ();
		SceneManager.LoadScene ("MainMenu");
	}

	public void Reload() {

		SoundManager.Instance.PlaySoundEffect (SoundManager.ITEM_SELECT, true);
		GameStats.Instance.ResetPlayerScore ();
		SceneManager.LoadScene("GameScene");
	}

	public void HideGameOverPanel()
	{
		gameOverPanel.SetActive (false);
	}

	public void ShowGameOverPanel()
	{
		gameOverPanel.SetActive (true);
	}
}
