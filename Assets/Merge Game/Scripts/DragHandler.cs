using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler {
	
	//==========================================================================
	// Handles dragging and dropping game pieces, locking them to the game board
	// and rotating double game pieces when clicked.
	// This script is attached to the game piece prefabs (SinglePiece, and both
	// PieceOne and PieceTwo of DoublePiece) and the four powerup prefabs.
	//==========================================================================

	//A static reference to the current gameObject that is being dragged
	public static GameObject gameObjectBeingDragged;
	private UIManager uIManager;
	//Starting position of the gameObjectBeingDragged
	private Vector3 startPosition;
	//Parent of the gameObjectBeingDragged
	private Transform startParent;
	//Transform of the gameObject that will move
	private Transform gameObjectToMove;
	private RectTransform backgroundPanelRectTransform;
	private Vector2 leftPivot = new Vector2 (0, 0.5f);
	private Vector2 centerPivot = new Vector2 (0.5f, 0.5f);
	private Vector2 rightPivot = new Vector2 (1, 0.5f);

	void Start()
	{
		uIManager = GameObject.Find ("Canvas").GetComponent<UIManager> ();
		backgroundPanelRectTransform = GameObject.Find ("BackgroundPanel").GetComponent<RectTransform> ();
	}

	/*
	 * Handles the behavior of a game piece while it is being dragged.
	 */ 
	public void OnBeginDrag (PointerEventData eventData)
	{
		//Set gameObjectBeingDragged is the current game object
		gameObjectBeingDragged = gameObject;

		//if the parent of the dragged gameObject is a twoPiece then
		//Change the pivot on the parent
		if (gameObjectBeingDragged.GetComponentInParent<TwoPiece> () != null) {
			startPosition = transform.parent.position;
			startParent = transform.parent.parent;
			gameObjectToMove = transform.parent;
			TwoPiece selectedTwoPiece = gameObjectBeingDragged.GetComponentInParent<TwoPiece> ();
			GamePiece selectedGamePiece = gameObjectBeingDragged.GetComponent<GamePiece> ();

			if (selectedGamePiece.gameObject == selectedTwoPiece.piece1.gameObject) {
				gameObjectToMove.GetComponent<RectTransform> ().pivot = leftPivot;
				gameObjectToMove.GetComponent<RectTransform> ().anchorMin = gameObjectToMove.GetComponent<RectTransform> ().anchorMax = leftPivot;
				gameObjectToMove.GetComponent<CanvasGroup> ().blocksRaycasts = false;
			} else if (selectedGamePiece.gameObject == selectedTwoPiece.piece2.gameObject) {
				gameObjectToMove.GetComponent<RectTransform> ().pivot = rightPivot;
				gameObjectToMove.GetComponent<RectTransform> ().anchorMin = gameObjectToMove.GetComponent<RectTransform> ().anchorMax = rightPivot;
				gameObjectToMove.GetComponent<CanvasGroup> ().blocksRaycasts = false;
			} 

		} else {	
			if (gameObject.tag != "Powerup") {
				//If the GamePiece is flagged as not draggable(i.e the gamePiece has already been placed on the board), stop here
				if (this.GetComponent<GamePiece> ().isGamePieceDraggable() == false)
					return;				
			}
			startPosition = transform.position;
			startParent = transform.parent;
			gameObjectToMove = transform;
			GetComponent<CanvasGroup> ().blocksRaycasts = false;
		}
	}

	/*
	 * Ensures that the game piece is always visible in front of the game board.
	 */ 
	public void OnDrag (PointerEventData eventData)
	{

		//If the GamePiece is flagged as not draggable(i.e the gamePiece has already been placed on the board), stop here
		if (gameObject.tag != "Powerup") {
			if (this.GetComponent<GamePiece> () != null && !this.GetComponent<GamePiece> ().isGamePieceDraggable())
				return;
		}
		Vector3 world = Vector3.zero;
		RectTransformUtility.ScreenPointToWorldPointInRectangle (backgroundPanelRectTransform, eventData.position, Camera.main, out world);
		gameObjectToMove.position = world;
	}

	/*
	 * Handles the behavior of the game piece once it has been dropped.
	 */ 
	public void OnEndDrag (PointerEventData eventData)
	{
		//If the GamePiece is flagged as not draggable(i.e the gamePiece has already been placed on the board), stop here
		if (gameObject.tag != "Powerup") {
			if (this.GetComponent<GamePiece> () != null && !this.GetComponent<GamePiece> ().isGamePieceDraggable())
				return;
		}

		gameObjectBeingDragged = null;
		gameObjectToMove.GetComponent<CanvasGroup> ().blocksRaycasts = true;
		if (gameObjectToMove.parent == startParent) {
			gameObjectToMove.GetComponent<RectTransform> ().pivot = centerPivot;
			gameObjectToMove.GetComponent<RectTransform> ().anchorMin = gameObjectToMove.GetComponent<RectTransform> ().anchorMax = centerPivot;
			gameObjectToMove.position = startPosition;
		}
		gameObjectToMove = null;
	}

	/*
	 * Handles the behavior of a double piece when it's clicked. Clicking a double piece rotates it 90 degrees clockwise.
	 */ 
	public void OnPointerClick (PointerEventData eventData)
	{

		gameObjectBeingDragged = gameObject;

		//If this is a TwoPiece, click to rotate it 90 degrees clockwise
		if (gameObjectBeingDragged.GetComponentInParent<TwoPiece> () != null) {
			//Play a sound effect
			SoundManager.Instance.PlaySoundEffect (SoundManager.ROTATE_PIECE, true);
			//rotate the TwoPiece 90 degrees clockwise
			gameObjectBeingDragged.transform.parent.Rotate (0, 0, -90);
			//But rotate each of the pieces the opposite direction so that their orientation doesn't change
			gameObjectBeingDragged.GetComponentInParent<TwoPiece>().piece1.transform.Rotate (0, 0, 90);
			gameObjectBeingDragged.GetComponentInParent<TwoPiece>().piece2.transform.Rotate (0, 0, 90);
			//Rotate the boxes behind the pieces along with them
			uIManager.RotateBoxes ();
		}
	}
}