using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GeneratePieces : MonoBehaviour {
	
	//================================================================
	// Handles generating game pieces and powerups.
	// This script is attached to the Canvas in the GameScene hierarchy.
	//================================================================

	public GameObject startingTile;
	//Tiles to attach any spawned powerups to 
	public GameObject powerup1Tile, powerup2Tile, powerup3Tile, powerup4Tile;
	//Spawn position for singlePiece or doublePiece
	public GameObject singlePiecePos, doublePiecePos;
	//Single piece prefab
	public Object gamePiecePrefab;
	//Two piece prefab
	public Object twoPieceGamePiecePrefab;
	//Powerup prefabs
	public List<Object> powerupPrefabs = new List<Object>();
	//Glow effect prefab that displays over a generated powerup
	public Object glowEffectPrefab;
	//The total number of pieces that have been created (this is used for determining a piece's idNum)
	public static int totalPieces = 0;
	private UIManager uIManager;
	private int pieceOneValue;
	private int pieceTwoValue = -1;
	private int allMergeLevelIndex = 0;
	private GameBoard gameBoard;
	//These lists are for choosing the values of a single piece and the leftmost piece in a double piece.
	//Essentially these lists represent the chances of getting a piece to spawn with the particular pieceValue
	//Adding more of a certain pieceValue into a list will increase the chance of that value being generated
	//For that situation
	private List<PieceValue> highestMergedLevelAchievedTwo = new List<PieceValue> {PieceValue.levelOne,PieceValue.levelOne,PieceValue.levelOne,
																	PieceValue.levelTwo,PieceValue.levelTwo};

	private List<PieceValue> highestMergedLevelAchievedThree = new List<PieceValue> {PieceValue.levelOne,PieceValue.levelOne,PieceValue.levelOne,
																	PieceValue.levelTwo,PieceValue.levelTwo,PieceValue.levelThree,PieceValue.levelThree,PieceValue.levelThree};

	private List<PieceValue> highestMergedLevelAchievedFour = new List<PieceValue> {PieceValue.levelOne,PieceValue.levelOne,PieceValue.levelOne,PieceValue.levelTwo,
																	PieceValue.levelTwo,PieceValue.levelThree,PieceValue.levelThree,PieceValue.levelFour,PieceValue.levelFour};
	
	private List<PieceValue> highestMergedLevelAchievedFive = new List<PieceValue> {PieceValue.levelOne,PieceValue.levelOne,PieceValue.levelTwo,PieceValue.levelTwo,
																	PieceValue.levelTwo,PieceValue.levelThree,PieceValue.levelThree,PieceValue.levelThree,PieceValue.levelThree,
																	PieceValue.levelFour,PieceValue.levelFour,PieceValue.levelFour,PieceValue.levelFive,PieceValue.levelFive,
																	PieceValue.levelFive};
	
	private List<PieceValue> highestMergedLevelAchievedSix = new List<PieceValue> {PieceValue.levelOne,PieceValue.levelOne,PieceValue.levelOne,PieceValue.levelOne,
																	PieceValue.levelOne,PieceValue.levelOne,PieceValue.levelOne,PieceValue.levelOne,PieceValue.levelOne,
																	PieceValue.levelOne,PieceValue.levelOne,PieceValue.levelOne,PieceValue.levelTwo,PieceValue.levelTwo,
																	PieceValue.levelTwo,PieceValue.levelTwo,PieceValue.levelTwo,PieceValue.levelTwo,PieceValue.levelTwo,
																	PieceValue.levelTwo,PieceValue.levelTwo,PieceValue.levelTwo,PieceValue.levelTwo,PieceValue.levelTwo,
																	PieceValue.levelThree,PieceValue.levelThree,PieceValue.levelThree,PieceValue.levelThree,PieceValue.levelThree,
																	PieceValue.levelThree,PieceValue.levelThree,PieceValue.levelThree,PieceValue.levelThree,PieceValue.levelThree,
																	PieceValue.levelThree,PieceValue.levelThree,PieceValue.levelThree,PieceValue.levelThree,PieceValue.levelThree,
																	PieceValue.levelFour,PieceValue.levelFour,PieceValue.levelFour,PieceValue.levelFour,PieceValue.levelFour,
																	PieceValue.levelFour,PieceValue.levelFour,PieceValue.levelFour,PieceValue.levelFour,PieceValue.levelFour,
																	PieceValue.levelFour,PieceValue.levelFour,PieceValue.levelFive,PieceValue.levelFive,PieceValue.levelFive,
																	PieceValue.levelFive,PieceValue.levelSix};
	
	private List<PieceValue> highestMergedLevelAchievedSeven = new List<PieceValue> {PieceValue.levelOne,PieceValue.levelOne,PieceValue.levelOne,PieceValue.levelOne,
																	PieceValue.levelOne,PieceValue.levelOne,PieceValue.levelOne,PieceValue.levelOne,PieceValue.levelOne,
																	PieceValue.levelOne,PieceValue.levelOne,PieceValue.levelOne,PieceValue.levelOne,PieceValue.levelOne,
																	PieceValue.levelOne,PieceValue.levelTwo,PieceValue.levelTwo,PieceValue.levelTwo,PieceValue.levelTwo,
																	PieceValue.levelTwo,PieceValue.levelTwo,PieceValue.levelTwo,PieceValue.levelTwo,PieceValue.levelTwo,
																	PieceValue.levelTwo,PieceValue.levelTwo,PieceValue.levelTwo,PieceValue.levelTwo,PieceValue.levelTwo,
																	PieceValue.levelTwo,PieceValue.levelThree,PieceValue.levelThree,PieceValue.levelThree,PieceValue.levelThree,
																	PieceValue.levelThree,PieceValue.levelThree,PieceValue.levelThree,PieceValue.levelThree,PieceValue.levelThree,
																	PieceValue.levelThree,PieceValue.levelThree,PieceValue.levelThree,PieceValue.levelThree,PieceValue.levelThree,
																	PieceValue.levelThree,PieceValue.levelFour,PieceValue.levelFour,PieceValue.levelFour,PieceValue.levelFour,
																	PieceValue.levelFour,PieceValue.levelFour,PieceValue.levelFour,PieceValue.levelFour,PieceValue.levelFour,
																	PieceValue.levelFour,PieceValue.levelFour,PieceValue.levelFour,PieceValue.levelFour,PieceValue.levelFour,
																	PieceValue.levelFour,PieceValue.levelFive,PieceValue.levelFive,PieceValue.levelFive,PieceValue.levelFive,
																	PieceValue.levelFive,PieceValue.levelFive,PieceValue.levelFive,PieceValue.levelFive,PieceValue.levelFive,
																	PieceValue.levelFive,PieceValue.levelFive,PieceValue.levelFive,PieceValue.levelSix,PieceValue.levelSix,
																	PieceValue.levelSix,PieceValue.levelSix,PieceValue.levelSeven};
	//This is a list of the above lists
	private List<List<PieceValue>> allMergeLevels = new List<List<PieceValue>> ();

	void Start ()
	{		
		gameBoard = GameObject.Find ("GameBoardPanel").GetComponent<GameBoard> ();
		uIManager = GameObject.Find ("Canvas").GetComponent<UIManager> ();

		//Populate the list of lists
		allMergeLevels.Add (highestMergedLevelAchievedTwo);
		allMergeLevels.Add (highestMergedLevelAchievedThree);
		allMergeLevels.Add (highestMergedLevelAchievedFour);
		allMergeLevels.Add (highestMergedLevelAchievedFive);
		allMergeLevels.Add (highestMergedLevelAchievedSix);
		allMergeLevels.Add (highestMergedLevelAchievedSeven);	
	}

	void Update ()
	{
		//If there isn't a piece in the starting tile and if pieces on the board aren't currently merging
		if (startingTile.transform.childCount == 2 && !gameBoard.AreTilesMerging ()) {

			//Only generate pieces that have appropriate values depending on the current highest piecevalue on the board
			switch (gameBoard.GetHighestMergeLevelAchieved ()) {
			//if highest value on the board is 2 then use the highestMergedLevelAchievedThree list
			case 2:
				allMergeLevelIndex = 1;
				break;
			//if highest value on the board is 3 then use the highestMergedLevelAchievedFour list
			case 3:
				allMergeLevelIndex = 2;
				break;
			//if highest value on the board is 4 then use the highestMergedLevelAchievedFive list
			case 4:
				allMergeLevelIndex = 3;
				break;
			//if highest value on the board is 5 then use the highestMergedLevelAchievedSix list
			case 5:
				allMergeLevelIndex = 4;			
				break;
			//if highest value on the board is 6 or 7 then use the highestMergedLevelAchievedSeven list
			case 6:
			case 7:
				allMergeLevelIndex = 5;			
				break;
			//By default use the highestMergedLevelAchievedTwo list
			default:
				allMergeLevelIndex = 0;
				break;
			}

			GameObject generatedPiece = null;

			//Generate a single piece half of the time, or generate only single pieces if there are only single tiles open on the board
			if (Random.value > 0.5f || !gameBoard.CheckForDoubleSpace()) { 
				//Generate one piece
				generatedPiece = Instantiate (gamePiecePrefab, singlePiecePos.transform.position, Quaternion.identity) as GameObject;
				//Set the piece's value
				generatedPiece.GetComponent<GamePiece> ().InitialPieceValue (allMergeLevels[allMergeLevelIndex][Random.Range(0,allMergeLevels[allMergeLevelIndex].Count)]);
				//Increment the number of total pieces
				totalPieces++;
				//Show one tile image behind it
				uIManager.ShowOneBox();
			} else {	
				//Generate a double piece
				generatedPiece = Instantiate (twoPieceGamePiecePrefab, doublePiecePos.transform.position, Quaternion.identity) as GameObject;

				//Determine piece one's value, which can never be the highest value in the list (as piece two is always at least one higher)
				pieceOneValue = (int)allMergeLevels [allMergeLevelIndex] [Random.Range (0, allMergeLevels [allMergeLevelIndex].Count)];
				//If it's the highest value possible, re-roll for a new value
				while (pieceOneValue == gameBoard.GetHighestMergeLevelAchieved () || pieceOneValue == 6) {
					pieceOneValue = (int)allMergeLevels [allMergeLevelIndex] [Random.Range (0, allMergeLevels [allMergeLevelIndex].Count)];
				}

				//Set piece one's value
				generatedPiece.GetComponent<TwoPiece>().piece1.GetComponent<GamePiece>().InitialPieceValue((PieceValue)pieceOneValue);

				//Get piece two's value, which must always be at least 1 higher than piece one's
				DeterminePieceTwoValue(allMergeLevels[allMergeLevelIndex], pieceOneValue);

				//Set piece two's value 
				generatedPiece.GetComponent<TwoPiece>().piece2.GetComponent<GamePiece>().InitialPieceValue((PieceValue)pieceTwoValue);

				//Increment the number of total pieces
				totalPieces += 2;
				//Show a tile image behind each piece
				uIManager.ShowTwoBoxes();
				//The tile images should be at their initial rotation
				uIManager.ResetBoxes();
			}
				
			//Make the starting tile its parent
			generatedPiece.transform.SetParent(startingTile.transform);

			//Set its scale to one or it instantiates too large
			generatedPiece.transform.localScale = Vector2.one;
		}
	}

	/*
	 * Determines the value of the right-side piece in a double game piece.
	 * The right-side piece's value must always be at least one higher than the left-side piece's value. 
	 */
	public void DeterminePieceTwoValue(List<PieceValue> source, int pieceOneValue) {

		//Copy the list of possible piece values
		List<PieceValue> destination = new List<PieceValue> (source);

		//Remove all numbers lower than or equal to piece one's value
		destination.RemoveAll(item => (int)item <= pieceOneValue);

		//Select a random number from those remaining, which will be piece two's value
		pieceTwoValue = (int)destination[Random.Range(0, destination.Count)];
	}

	/*
	 * Generates a powerup when the user makes a top-level merge. 
	 * A powerup will only be granted when one of the four powerup slots is open.
	 * The player can only hold one of each powerup at a time. 
	 */
	public void GeneratePowerup() {	

		bool isValidPosition = false;
		GameObject generatedPowerup = null;
		GameObject parentTile = powerup1Tile;
		Vector3 position = new Vector3();
		int powerupIndex = -1;

		//If one of the powerup slots is open
		if (powerup1Tile.transform.childCount < 1 || powerup2Tile.transform.childCount < 1 || powerup3Tile.transform.childCount < 1 || powerup4Tile.transform.childCount < 1) {

			while (!isValidPosition) {

				//Choose one of the four powerups
				powerupIndex = Random.Range (0, powerupPrefabs.Count);

				if (powerupIndex == 0) {
					if (powerup1Tile.transform.childCount > 0) {
						isValidPosition = false;
					} else {
						position = powerup1Tile.transform.position;
						parentTile = powerup1Tile;
						isValidPosition = true;
					}
				} else if (powerupIndex == 1) {	
					if (powerup2Tile.transform.childCount > 0) {
						isValidPosition = false;
					} else {
						position = powerup2Tile.transform.position;
						parentTile = powerup2Tile;
						isValidPosition = true;
					}
				} else if (powerupIndex == 2) {	
					if (powerup3Tile.transform.childCount > 0) {
						isValidPosition = false;
					} else {
						position = powerup3Tile.transform.position;
						parentTile = powerup3Tile;
						isValidPosition = true;
					}
				} else if (powerupIndex == 3) {	
					if (powerup4Tile.transform.childCount > 0) {
						isValidPosition = false;
					} else {					
						position = powerup4Tile.transform.position;
						parentTile = powerup4Tile;
						isValidPosition = true;
					}
				}
			}

			//Instantiate a glow effect on top of the powerup
			GameObject glow = Instantiate (glowEffectPrefab, position, Quaternion.identity) as GameObject;
			glow.transform.SetParent (parentTile.transform.parent);
			glow.transform.position = parentTile.transform.position;
			glow.transform.localScale = Vector2.one;
			//Fade the glow in
			glow.GetComponent<Image>().CrossFadeAlpha(192, 0.5f, true);
			//Fade the glow back out
			StartCoroutine("FadeOut", glow);

			//Instantiate a powerup in the proper position and with the proper parent
			generatedPowerup = Instantiate (powerupPrefabs [powerupIndex], position, Quaternion.identity) as GameObject;
			generatedPowerup.transform.SetParent (parentTile.transform);
			//And at the proper scale
			generatedPowerup.transform.localScale = Vector2.one;
			isValidPosition = false;
		}
	}

	/*
	 * Fades the glow effect behind a generated powerup back out after three-quarters of a second.
	 */ 
	public IEnumerator FadeOut(GameObject glow) {
		yield return new WaitForSeconds (0.75f);
		glow.GetComponent<Image>().CrossFadeAlpha(1, 0.5f, true);
		//Destroy the glow object
		Destroy(glow, 0.75f);
	}
}
