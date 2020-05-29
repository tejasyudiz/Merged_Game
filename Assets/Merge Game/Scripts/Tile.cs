using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Tile : MonoBehaviour, IDropHandler {
	
	

	public int row;//tile Row
	public int column;// tile column
	//Reference to a gamePiece on the tile will return null if their is none
	public GamePiece gamePiece { get; set; }
	//Reference to a powerUp on the tile will return null if their is none
	public Powerup powerUp { get; set; }
	//various special effects prefabs
	public Object mergePoof;
	public Object smallStars;
	public Object mediumStars;
	public Object largeStars;
	public Object superStars;

	private const float MERGE_TIME = 0.5f; //the time it takes for pieces to merge
	private const float MERGE_DONE = 0.5f; //the time to wait before doing something else after a merge
	private const float STAR_TIME = 0.5f; //the time to wait before instantiating the star particle effect
	private GameBoard gameBoard;
	private bool isMerging = false;

	void Start ()
	{
		gameBoard = GameObject.Find ("GameBoardPanel").GetComponent<GameBoard> ();
	}

	
	public void OnDrop (PointerEventData eventData)
	{	
		
		if (!gamePiece) {	
			//If the piece is a powerup
			if (DragHandler.gameObjectBeingDragged.CompareTag("Powerup")) {

				//Grab the item being dragged's transform and set its parent to the tile's transform
				DragHandler.gameObjectBeingDragged.transform.SetParent (transform);

				//Add a reference to the dropped powerup in this tile
				powerUp = DragHandler.gameObjectBeingDragged.GetComponent<Powerup> ();

				//Center its image and set its transform position to this tile's position
				powerUp.CenterImage ();
				DragHandler.gameObjectBeingDragged.transform.position = transform.position;

				//If dropping a bomb powerup
				if (powerUp.GetPowerupType () == PowerupType.Bomb) {

					//Play a sound effect
					SoundManager.Instance.PlaySoundEffect (SoundManager.BOMB_POWERUP, true, 0);

					gameBoard.BombUsed (row, column);
					//Play the magic effect when the powerup is destroyed
					MagicEffect ();
					Destroy (powerUp.gameObject);
				}

				//If it's a clear column
				if (powerUp.GetPowerupType () == PowerupType.ClearColumn) {

					//Play a sound effect
					SoundManager.Instance.PlaySoundEffect (SoundManager.OTHER_POWERUP, true, 0);
					
					//The powerup clears all tiles in the column and then disappears
					gameBoard.ClearColumnUsed (row, column);
					MagicEffect ();
					Destroy (powerUp.gameObject);
				}

				//If it's a clear row
				if (powerUp.GetPowerupType () == PowerupType.ClearRow) {

					//Play a sound effect
					SoundManager.Instance.PlaySoundEffect (SoundManager.OTHER_POWERUP, true, 0);
					
					gameBoard.ClearRowUsed (row, column);
					MagicEffect ();
					Destroy (powerUp.gameObject);
				}

				//If it's a clear row and column
				if (powerUp.GetPowerupType () == PowerupType.ClearRowAndColumn) {

					SoundManager.Instance.PlaySoundEffect (SoundManager.BOMB_POWERUP, true, 0);

					gameBoard.ClearRowUsed (row, column);
					gameBoard.ClearColumnUsed (row, column);
					MagicEffect ();
					Destroy (powerUp.gameObject);
				}
			}
			//if it's a singlePiece
			if (DragHandler.gameObjectBeingDragged.GetComponent<GamePiece> () != null && DragHandler.gameObjectBeingDragged.GetComponent<GamePiece> ().isGamePieceDraggable()) {
				if (DragHandler.gameObjectBeingDragged.GetComponentInParent<TwoPiece> () == null) {

					
					SoundManager.Instance.PlaySoundEffect (SoundManager.DROP_PIECE, true);

					DragHandler.gameObjectBeingDragged.transform.SetParent (transform);

					gamePiece = DragHandler.gameObjectBeingDragged.GetComponent<GamePiece> ();

					gamePiece.CenterImage ();
					DragHandler.gameObjectBeingDragged.transform.position = transform.position;

					gamePiece.SetIsGamePieceDraggable(false);
					gamePiece.idNum = GeneratePieces.totalPieces;
					//Look for matches on the board
					gameBoard.MatchAndClear ();
				} else {
					//If the game piece is a double piece
					TwoPiece selectedTwoPiece = DragHandler.gameObjectBeingDragged.GetComponentInParent<TwoPiece> ();
					GamePiece selectedGamePiece = DragHandler.gameObjectBeingDragged.GetComponent<GamePiece> ();
					int newRow = row, newCol = column;
					//If the player is dragging piece one
					if (selectedGamePiece.gameObject == selectedTwoPiece.piece1.gameObject) {
						if (selectedTwoPiece.transform.eulerAngles.z == 0) {
							newCol++;
						} else if (selectedTwoPiece.transform.eulerAngles.z == 90) {
							newRow--;
						} else if (selectedTwoPiece.transform.eulerAngles.z == 180) {
							newCol--;
						} else if (selectedTwoPiece.transform.eulerAngles.z == 270) {
							newRow++;
						}
						//If there is room on the board for the piece
						if (gameBoard.CanPlacePieceOnTile (newRow, newCol) && gameBoard.CanPlacePieceOnTile (row, column)) {

							//Play a sound effect
							SoundManager.Instance.PlaySoundEffect (SoundManager.DROP_PIECE, true);

							//Set the tiles as the piece's parents and center its image
							selectedTwoPiece.piece2.transform.SetParent (gameBoard.GetTileAt (newRow, newCol).transform);
							selectedTwoPiece.piece2.GetComponent<GamePiece> ().CenterImage ();
							selectedTwoPiece.piece2.transform.position = gameBoard.GetTileAt (newRow, newCol).transform.position;
							//Set the gamePiece reference
							gameBoard.GetTileAt (newRow, newCol).gamePiece = selectedTwoPiece.piece2.GetComponent<GamePiece> ();
							gameBoard.GetTileAt (newRow, newCol).gamePiece.SetIsGamePieceDraggable (false);
							//Do the same for piece1
							selectedTwoPiece.piece1.transform.SetParent (transform);
							selectedTwoPiece.piece1.GetComponent<GamePiece> ().CenterImage ();
							selectedTwoPiece.piece1.transform.position = transform.position;
							gamePiece = selectedTwoPiece.piece1.GetComponent<GamePiece> ();
							gamePiece.SetIsGamePieceDraggable (false);

							//Destroy the left over gameObject
							Destroy (selectedTwoPiece.gameObject);
							//Set the idNum for each piece
							if ((int)selectedTwoPiece.piece1.GetComponent<GamePiece> ().GetPieceValue () > (int)selectedTwoPiece.piece2.GetComponent<GamePiece> ().GetPieceValue ()) {
								selectedTwoPiece.piece1.GetComponent<GamePiece> ().idNum = GeneratePieces.totalPieces;
								selectedTwoPiece.piece2.GetComponent<GamePiece> ().idNum = GeneratePieces.totalPieces - 1;
							} else {
								selectedTwoPiece.piece1.GetComponent<GamePiece> ().idNum = GeneratePieces.totalPieces - 1;
								selectedTwoPiece.piece2.GetComponent<GamePiece> ().idNum = GeneratePieces.totalPieces;
							}

							//Look for matches on the board
							gameBoard.MatchAndClear ();
						}
						//If the player is dragging piece two of a double piece
					} else if (selectedGamePiece.gameObject == selectedTwoPiece.piece2.gameObject) {
						//Check to make sure there's room on the board for both it and piece one
						if (selectedTwoPiece.transform.eulerAngles.z == 0) {
							newCol--;
						} else if (selectedTwoPiece.transform.eulerAngles.z == 90) {
							newRow++;
						} else if (selectedTwoPiece.transform.eulerAngles.z == 180) {
							newCol++;
						} else if (selectedTwoPiece.transform.eulerAngles.z == 270) {
							newRow--;
						}
						//If there's room on the board for the piece
						if (gameBoard.CanPlacePieceOnTile (newRow, newCol) && gameBoard.CanPlacePieceOnTile (row, column)) {

							//Play a sound effect
							SoundManager.Instance.PlaySoundEffect (SoundManager.DROP_PIECE, true);

							selectedTwoPiece.piece1.transform.SetParent (gameBoard.GetTileAt (newRow, newCol).transform);
							selectedTwoPiece.piece1.GetComponent<GamePiece> ().CenterImage ();
							selectedTwoPiece.piece1.transform.position = gameBoard.GetTileAt (newRow, newCol).transform.position;
							gameBoard.GetTileAt (newRow, newCol).gamePiece = selectedTwoPiece.piece1.GetComponent<GamePiece> ();
							gameBoard.GetTileAt (newRow, newCol).gamePiece.SetIsGamePieceDraggable (false);

							selectedTwoPiece.piece2.transform.SetParent (transform);
							selectedTwoPiece.piece2.GetComponent<GamePiece> ().CenterImage ();
							selectedTwoPiece.piece2.transform.position = transform.position;
							gamePiece = selectedTwoPiece.piece2.GetComponent<GamePiece> ();
							gamePiece.SetIsGamePieceDraggable (false);

							//Destroy the left over gameObject
							Destroy (selectedTwoPiece.gameObject);
							//Set the idNum for each piece
							if ((int)selectedTwoPiece.piece1.GetComponent<GamePiece> ().GetPieceValue () < (int)selectedTwoPiece.piece2.GetComponent<GamePiece> ().GetPieceValue ()) {
								selectedTwoPiece.piece1.GetComponent<GamePiece> ().idNum = GeneratePieces.totalPieces;
								selectedTwoPiece.piece2.GetComponent<GamePiece> ().idNum = GeneratePieces.totalPieces - 1;
							} else {
								selectedTwoPiece.piece1.GetComponent<GamePiece> ().idNum = GeneratePieces.totalPieces - 1;
								selectedTwoPiece.piece2.GetComponent<GamePiece> ().idNum = GeneratePieces.totalPieces;
							}

							//Look for matches on the board
							gameBoard.MatchAndClear ();
						}
					}
				}
			}
		}
	}

	/*
	 * Begins the process of merging to a single piece.
	 */ 
	public void MergeToPiece (List<Tile> list)
	{	
		bool isLevelSevenPiece = false;

		isMerging = true;
		//For each gamePiece in the list
		for (int i = 0; i < list.Count; i++) {			
			list [i].gamePiece.transform.SetParent (transform); //Make this tile the game piece's parent
			if (list[i].gamePiece.pieceValue == PieceValue.levelSeven) {
				isLevelSevenPiece = true;
			}
			//Move the piece to the center of this tile
			StartCoroutine (MoveToPosition (Vector3.zero, MERGE_TIME, list [i].gamePiece.GetComponent<RectTransform> ()));
		}
		//Play a magic effect once the pieces have finished merging
		Invoke ("TriggerMagic", MERGE_DONE);

		//If the piece is not a top level piece
		if (!isLevelSevenPiece) {
			//Play a sound effect too
			Invoke ("PlayMergeSound", MERGE_DONE);
			isLevelSevenPiece = false;
		}
	}

 
	private IEnumerator MoveToPosition (Vector3 target, float time, RectTransform transformToMove)
	{	

		float elapsedTime = 0;
		Vector3 startingPos = transformToMove.anchoredPosition;

		while (elapsedTime <= time) {
			elapsedTime += Time.deltaTime;
			if (transformToMove != null) {
				transformToMove.anchoredPosition = Vector3.Lerp (startingPos, target, (elapsedTime / time));
			}

			yield return new WaitForEndOfFrame ();
		}
		if (transformToMove != null) {
			//Destroy the gamepiece when it reaches "target"
			transformToMove.gameObject.GetComponent<GamePiece> ().DestroyPiece ();
		}	
	}

	public void TriggerMagic() {
		//Play the effect
		MagicEffect();
		Invoke ("MergingToFalse", 0.2f);
	}

	
	public void PlayMergeSound() {
		//Play a sound effect
		SoundManager.Instance.PlaySoundEffect (SoundManager.NORMAL_MERGE, true, 0);
	}

	
	public void MergingToFalse ()
	{	
		gamePiece.IncreasePieceValue ();
		isMerging = false;
	}


	public void MagicEffect() {	
		GameObject poof = Instantiate (mergePoof, GetComponent<RectTransform> ().transform.position, Quaternion.identity) as GameObject;
		poof.transform.SetParent (transform);
		poof.GetComponent<RectTransform>().localPosition = Vector3.zero;
		poof.transform.localScale = Vector2.one;
		Invoke ("StarEffect", STAR_TIME);
		Destroy (poof, 1.0f);
	}

	
	public void MagicEffectPowerup() {
		GameObject poof = Instantiate (mergePoof, GetComponent<RectTransform> ().transform.position, Quaternion.identity) as GameObject;
		poof.transform.SetParent (transform);
		poof.GetComponent<RectTransform>().localPosition = Vector3.zero;
		poof.transform.localScale = Vector2.one;
		Invoke ("StarEffectPowerup", STAR_TIME);
		Destroy (poof, 1.0f);
	}

	
	public void StarEffect() {

		GameObject stars = null;

		if (!gamePiece) {
			stars = Instantiate (superStars, transform.position, Quaternion.identity) as GameObject;
		} else {
			if (gamePiece.pieceValue == PieceValue.levelTwo || gamePiece.pieceValue == PieceValue.levelThree) {
				stars = Instantiate (smallStars, transform.position, Quaternion.identity) as GameObject;
			} else if (gamePiece.pieceValue == PieceValue.levelFour || gamePiece.pieceValue == PieceValue.levelFive || gamePiece.pieceValue == PieceValue.levelSix) {
				stars = Instantiate (mediumStars, transform.position, Quaternion.identity) as GameObject;
			} 
			else if (gamePiece.pieceValue == PieceValue.levelSeven) {
				stars = Instantiate (largeStars, transform.position, Quaternion.identity) as GameObject;
			}
		}	
			
			stars.transform.SetParent (transform);
			stars.transform.localPosition = Vector3.zero;
			stars.transform.localScale = Vector3.one;
			stars.transform.rotation = Quaternion.Euler (0, 180, 180);
			Destroy (stars, 3.0f);
	}

	
	public void StarEffectPowerup() {
		GameObject stars = null;
		stars = Instantiate (smallStars, transform.position, Quaternion.identity) as GameObject;
		stars.transform.SetParent (transform);
		stars.transform.localPosition = Vector3.zero;
		stars.transform.localScale = Vector3.one;
		stars.transform.rotation = Quaternion.Euler (0, 180, 180);
		Destroy (stars, 3.0f);
	}

	
	public bool IsMerging ()
	{
		return isMerging;
	}


	public void DestroyGamePiece ()
	{
		gamePiece.DestroyPiece ();
	}
}
