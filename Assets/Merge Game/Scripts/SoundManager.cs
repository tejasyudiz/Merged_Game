using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {
	

	//Background music
	public const string MAIN_MENU_MUSIC = "Ouroboros";
	public const string IN_GAME_MUSIC = "Latin Industries";

	//Main Menu sounds    
	public const string ITEM_SELECT = "MenuButton";
	public const string BACK_BUTTON = "BackButton";

	//In-game sounds
	public const string ROTATE_PIECE = "GamePieceRotate";
	public const string DROP_PIECE = "GamePieceDropped";
	public const string NORMAL_MERGE = "MergeSmall";
	public const string TOP_LEVEL_MERGE = "MergeBig";
	public const string BOMB_POWERUP = "PowerUpBig";
	public const string OTHER_POWERUP = "PowerUpSmall";

	//List of soundEffects
	public List<AudioClip> uiAndOtherSounds;
	//List of background music tracks
	public List<AudioClip> backgroundMusic;
	//The AudioSource that will play soundeffects
	public AudioSource uiAndOtherSource;
	//The AudioSource that will play background music
	public AudioSource backgroundMusicSource;
	private static SoundManager instance = null;

	public static SoundManager Instance 
	{
		get { return instance; }
	}

	void Awake() 
	{
		if (instance != null && instance != this) 
		{
			Destroy(this.gameObject);
			return;
		} 
		else 
		{
			instance = this;
		}
		DontDestroyOnLoad(this.gameObject);
	}


	public AudioSource GetBackgroundMusicAudioSource()
	{
		return backgroundMusicSource;
	}

	
	public AudioSource GetUIAndOtherAudioSource()
	{
		return uiAndOtherSource;
	}

	
	public void PlayBackgroundMusic(string name)
	{
	
		if(backgroundMusicSource.enabled && backgroundMusicSource.clip != null && backgroundMusicSource.clip.name == name)
		{				
			backgroundMusicSource.Play();
		}
		else
		{
			
			AudioClip backgroundMusic = GetBackgroundMusic (name);
			if(backgroundMusic != null)
			{
				backgroundMusicSource.clip = backgroundMusic;
				if (backgroundMusicSource.enabled)					
					backgroundMusicSource.Play();
			}
			else 
			{
				Debug.LogError("Background music was not found");
			}
		}
	}

	
	private AudioClip GetBackgroundMusic(string name)
	{
		for(int i = 0; i < backgroundMusic.Count; ++i)
		{
			if(backgroundMusic[i].name == name)
			{
				return backgroundMusic[i];
			}
		}
		return null;
	}

	 
	public void PlaySoundEffect(string name, bool playOneShot, float delay = 0.0f)
	{
		if (uiAndOtherSource.enabled)
		{
			
			if (!playOneShot && uiAndOtherSource.clip != null && uiAndOtherSource.clip.name == name)
			{
				uiAndOtherSource.Play();
			}
			else
			{
				AudioClip soundEffect = GetAudio(name);

				if (soundEffect != null)
				{
					if (delay > 0.0f)
					{
						StartCoroutine(PlaySoundEffectWithDelay(soundEffect, playOneShot, delay));
					}
					else if (playOneShot)
					{
						uiAndOtherSource.PlayOneShot(soundEffect);
					}
					else
					{
						uiAndOtherSource.clip = soundEffect;
						uiAndOtherSource.Play();
					}
				}
				else
				{
					Debug.LogError("Sound was not found");
				}
			}
		}
	}

	private IEnumerator PlaySoundEffectWithDelay(AudioClip soundEffect, bool playOneShot, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (playOneShot)
		{
			uiAndOtherSource.PlayOneShot(soundEffect);
		}
		else
		{
			uiAndOtherSource.clip = soundEffect;
			uiAndOtherSource.Play();
		}
	}

	private AudioClip GetAudio(string name)
	{
		for(int i = 0; i < uiAndOtherSounds.Count; ++i)
		{
			if(uiAndOtherSounds[i].name == name)
			{
				return uiAndOtherSounds[i];
			}
		}
		return null;
	}
}