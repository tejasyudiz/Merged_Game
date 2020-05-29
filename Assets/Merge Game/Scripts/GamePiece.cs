using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public enum PieceValue
{
	levelOne = 0,
	levelTwo = 1,
	levelThree = 2,
	levelFour = 3,
	levelFive = 4,
	levelSix = 5,
	levelSeven = 6}
;

public class GamePiece : MonoBehaviour {

	

	//Value 
	public PieceValue pieceValue;
	public Image gamePieceImage;
	public Sprite[] sprites;
	public RectTransform rectTransform;
	//IdNum 
	public int idNum { get; set; }
	private bool isDraggable;
	private UIManager uIManager;
	private Vector2 centerPivot = new Vector2 (0.5f, 0.5f);

	void Start ()
	{
		uIManager = GameObject.Find ("Canvas").GetComponent<UIManager> ();

		rectTransform = GetComponent<RectTransform> ();

		isDraggable = true;
	}

	public void SetIsGamePieceDraggable(bool draggable)
	{
		isDraggable = draggable;
	}

	public bool isGamePieceDraggable()
	{
		return isDraggable;
	}

	// Returns  piece. 

	public PieceValue GetPieceValue ()
	{
		return pieceValue;
	}

	
	public void DestroyPiece ()
	{
		GameStats.Instance.IncrementPlayerScore ((int)pieceValue + 1);
		
		uIManager.SetScore (GameStats.Instance.GetPlayerScore ());
		Destroy (gameObject);

	}

	
	public void SetPieceValue (PieceValue value)
	{
		pieceValue = value;
	}

	
	public void CenterImage ()
	{
		rectTransform.pivot = centerPivot;
		rectTransform.anchorMin = rectTransform.anchorMax = centerPivot;
	}

	
	public void IncreasePieceValue ()
	{
		int currentPieceValue = (int)pieceValue;
		if (currentPieceValue < (int)PieceValue.levelSeven) {
			currentPieceValue++;
			pieceValue = (PieceValue)currentPieceValue;
			gamePieceImage.sprite = sprites [currentPieceValue];
		}
	}


	public void InitialPieceValue(PieceValue value) 
	{	
		pieceValue = value;
		
		gamePieceImage.sprite = sprites [(int)pieceValue];
	}

}
