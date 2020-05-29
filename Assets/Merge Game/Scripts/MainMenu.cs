using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public GameObject menu, optionsMenu, highScoresMenu, howToPlayMenu;
	private float lowVolume = 0.3f;

	void Start () {

		
		Time.timeScale = 1;

		Application.targetFrameRate = 60; 

		
		if(PlayerPrefs.GetInt("Music") == 0)
		{
			SoundManager.Instance.GetBackgroundMusicAudioSource().enabled = false;
		}
		else if(PlayerPrefs.GetInt("Music") == 1)
		{
			SoundManager.Instance.GetBackgroundMusicAudioSource().enabled = true;
		}

		if (PlayerPrefs.GetInt ("Sound") == 0) {

			SoundManager.Instance.GetUIAndOtherAudioSource ().enabled = false;
		} 
		else if (PlayerPrefs.GetInt ("Sound") == 1) {

			SoundManager.Instance.GetUIAndOtherAudioSource ().enabled = true;

		} 
	
		if (PlayerPrefs.GetInt ("Sound") == 1 && PlayerPrefs.GetInt ("Music") == 1) {
			SoundManager.Instance.GetBackgroundMusicAudioSource ().volume = lowVolume;
		} else if (PlayerPrefs.GetInt ("Sound") == 0 && PlayerPrefs.GetInt ("Music") == 1) {
			SoundManager.Instance.GetBackgroundMusicAudioSource ().volume = 1f;
		}
		SoundManager.Instance.PlayBackgroundMusic (SoundManager.MAIN_MENU_MUSIC);

	}

	
	public void Play() {

		ButtonClickEffect ();

		if (PlayerPrefs.GetInt ("Sound") == 1 && PlayerPrefs.GetInt ("Music") == 1) {
			SoundManager.Instance.GetBackgroundMusicAudioSource ().volume = lowVolume;
		} else if (PlayerPrefs.GetInt ("Sound") == 0 && PlayerPrefs.GetInt ("Music") == 1) {
			SoundManager.Instance.GetBackgroundMusicAudioSource ().volume = 1f;
		}

		SoundManager.Instance.PlayBackgroundMusic (SoundManager.IN_GAME_MUSIC);
		
		SceneManager.LoadScene("GameScene");
	}

	
	public void SwitchToHowToPlayMenu() {

		ButtonClickEffect ();
		howToPlayMenu.SetActive (true);
		optionsMenu.SetActive (false);
		highScoresMenu.SetActive (false);
	}

	
	public void SwitchToHighScoresPanel()
	{	
		ButtonClickEffect ();
		highScoresMenu.SetActive (true);
		optionsMenu.SetActive (false);
		howToPlayMenu.SetActive (false);
	}

	
	public void SwitchToOptionsPanel()
	{	
		ButtonClickEffect ();
		optionsMenu.SetActive (true);
		highScoresMenu.SetActive (false);
	}

	
	public void BackToMenu() {
		SoundManager.Instance.PlaySoundEffect (SoundManager.BACK_BUTTON, true, 0);
		optionsMenu.SetActive (false);
		highScoresMenu.SetActive (false);

	}

	
	public void ButtonClickEffect() {
		SoundManager.Instance.PlaySoundEffect (SoundManager.ITEM_SELECT, true, 0);
	}
}
