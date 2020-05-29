using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HowToPlayMenu : MonoBehaviour {

	
	public Text howToPlayTextLine1,howToPlayTextLine2;
	public Image howToPlayImage;
	public List<string> howToPlayTextLine1List;
	public List<string> howToPlayTextLine2List;
	public List<Sprite> howToPlayImages;
	private int index = 0;
	private int numOfPages = -1;

	void Start () 
	{
		howToPlayImage.sprite = howToPlayImages [index];
		howToPlayTextLine1.text = howToPlayTextLine1List [index];
		howToPlayTextLine2.text = howToPlayTextLine2List [index];
		numOfPages = howToPlayImages.Count;
	}

	
	public void NextPage()
	{
		SoundManager.Instance.PlaySoundEffect (SoundManager.ITEM_SELECT, true, 0);
		index++;
		if(index == numOfPages)
		{
			index = 0;
		}
		SetText ();
		SetImage ();
	}

	
	public void PrevPage()
	{
		SoundManager.Instance.PlaySoundEffect (SoundManager.ITEM_SELECT, true, 0);
		index--;
		if(index < 0)
		{
			index = numOfPages - 1;
		}
		SetText ();
		SetImage ();
	}

	
	private void SetImage()
	{
		howToPlayImage.sprite = howToPlayImages [index];
	}

	
	private void SetText()
	{
		howToPlayTextLine1.text = howToPlayTextLine1List [index];
		howToPlayTextLine2.text = howToPlayTextLine2List [index];
	}
}
