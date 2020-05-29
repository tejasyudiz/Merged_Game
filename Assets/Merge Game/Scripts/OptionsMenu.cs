using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {


	public Button musicOn, musicOff, soundEffectsOn, soundEffectsOff;
	private float lowVolume = 0.3f;

	void Update()
	{	
		
		if(PlayerPrefs.GetInt("Music") == 1)
		{
			musicOn.gameObject.SetActive(true);
			musicOff.gameObject.SetActive(false);
		}
		else
		{
			musicOn.gameObject.SetActive(false);
			musicOff.gameObject.SetActive(true);
		}

		if(PlayerPrefs.GetInt("Sound") == 1)
		{
			soundEffectsOn.gameObject.SetActive(true);
			soundEffectsOff.gameObject.SetActive(false);
		}
		else
		{
			soundEffectsOn.gameObject.SetActive(false);
			soundEffectsOff.gameObject.SetActive(true);
		}			
	}

	
	public void TurnMusicOn()	{		

		PlayerPrefs.SetInt ("Music", 1);
		SoundManager.Instance.PlaySoundEffect (SoundManager.ITEM_SELECT, true, 0);
		SoundManager.Instance.GetBackgroundMusicAudioSource ().enabled = true;

	
		if (PlayerPrefs.GetInt ("Sound") == 1) {
			SoundManager.Instance.GetBackgroundMusicAudioSource ().volume = lowVolume;
		} else {
			SoundManager.Instance.GetBackgroundMusicAudioSource ().volume = 1f;
		}

		SoundManager.Instance.GetBackgroundMusicAudioSource ().Play ();
	}


	public void TurnMusicOff()	{	

		PlayerPrefs.SetInt ("Music", 0);
		SoundManager.Instance.PlaySoundEffect (SoundManager.ITEM_SELECT, true, 0);
		SoundManager.Instance.GetBackgroundMusicAudioSource ().enabled = false;
	}

	
	public void TurnSoundOn()	{	

		PlayerPrefs.SetInt ("Sound", 1);
		SoundManager.Instance.GetUIAndOtherAudioSource ().enabled = true;
		SoundManager.Instance.PlaySoundEffect (SoundManager.ITEM_SELECT, true, 0);

		if (PlayerPrefs.GetInt ("Music") == 1) {
			SoundManager.Instance.GetBackgroundMusicAudioSource ().volume = lowVolume;
		}
	}

	
	public void TurnSoundOff()
	{
		PlayerPrefs.SetInt ("Sound", 0);
		SoundManager.Instance.GetUIAndOtherAudioSource ().enabled = false;
	
		if (PlayerPrefs.GetInt ("Music") == 1) {
			SoundManager.Instance.GetBackgroundMusicAudioSource ().volume = 1f;
		}
	}
}
