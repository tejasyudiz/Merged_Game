using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour {
	


	private UIManager uIManager;
	private const int NUM_OF_TILES_PER_LINE = 5;	//the number of tiles in a row or column
	private Tile[,] gameBoard = new Tile[NUM_OF_TILES_PER_LINE, NUM_OF_TILES_PER_LINE];
	private const float MERGE_DONE = 0.75f; 	//the time to wait for tiles to finish merging
	private GeneratePieces generatePieces;
	private int currentMergeLevel;
	private int highestMergeLevelAchieved = 1;
	private bool foundMatches = false;

	void Awake ()
	{
		for (int i = 0; i < NUM_OF_TILES_PER_LINE; i++) {
			for (int j = 0; j < NUM_OF_TILES_PER_LINE; j++) {	
				gameBoard [i, j] = GameObject.Find ("TileRow" + (i + 1) + "Column" + (j + 1)).GetComponent<Tile> ();
			}
		}
	}

	void Start ()
	{
		uIManager = GameObject.Find ("Canvas").GetComponent<UIManager> ();
		generatePieces = GameObject.Find ("Canvas").GetComponent<GeneratePieces> ();
	}

	
	public bool CanPlacePieceOnTile (int row, int col)
	{			
		if (ValidTile(row,col)) {
			if (gameBoard [row, col].gamePiece == null)
				return true;
			else
				return false;
		} else
			return false;
	}

	
	public Tile GetTileAt (int row, int col)
	{
		return gameBoard [row, col];
	}

	
	public void CopyBoard (Tile[,] source, Tile[,] destination)
	{
		for (int y = 0; y < NUM_OF_TILES_PER_LINE; y++) {
			for (int x = 0; x < NUM_OF_TILES_PER_LINE; x++) {
				destination [x, y] = source [x, y];
			}
		}
	}

	private void TestTile (int x, int y, Tile[,] toTest, Tile currentTile, List<Tile> collector)
	{
		
		if (toTest [x, y] == null || toTest [x, y].gamePiece == null) {
			return;
		}
		
		if (currentTile == null) {
			currentTile = toTest [x, y];
			toTest [x, y] = null;
			collector.Add (currentTile);
		}
		
		else if (((currentTile.gamePiece == null) || toTest [x, y].gamePiece == null) || (currentTile.gamePiece.GetPieceValue () != toTest [x, y].gamePiece.GetPieceValue ())) {
			return;
		}
		
		else {
			collector.Add (toTest [x, y]);
			toTest [x, y] = null;
		}

		
		if (x > 0)
			TestTile (x - 1, y, toTest, currentTile, collector);
		if (y > 0)
			TestTile (x, y - 1, toTest, currentTile, collector);
		if (x < NUM_OF_TILES_PER_LINE - 1)
			TestTile (x + 1, y, toTest, currentTile, collector);
		if (y < NUM_OF_TILES_PER_LINE - 1)
			TestTile (x, y + 1, toTest, currentTile, collector);
	}

	public void MatchAndClear ()
	{	

		Tile[,] toTest = new Tile[5, 5];
		CopyBoard (gameBoard, toTest);
		foundMatches = false;

	
		Tile currentTile = null;
	
		List<Tile> collector = new List<Tile> ();
	
		List<List<Tile>> allMerges = new List<List<Tile>> ();

		for (int y = 0; y < NUM_OF_TILES_PER_LINE; y++) {
			for (int x = 0; x < NUM_OF_TILES_PER_LINE; x++) {
				TestTile (x, y, toTest, currentTile, collector);
	
				if (collector.Count >= 3) {	
					allMerges.Add (new List<Tile> (collector));
				}
				currentTile = null;
				collector.Clear ();
			}
		}

		allMerges.Sort ((x, y) => x [0].gamePiece.pieceValue.CompareTo (y [0].gamePiece.pieceValue));

	
		StartCoroutine (StartMerging (allMerges));

	
		StartCoroutine (DoubleCheck ());
	}

	 
	private IEnumerator StartMerging (List<List<Tile>> allMerges)
	{				
		if (allMerges.Count > 0) {

			int highestValueIndex = -1;
			int highestValueIDNumber = -1;

			
			for (int a = 0; a < allMerges [0].Count; a++) {

				if (highestValueIndex == -1) {
					highestValueIndex = 0;
					highestValueIDNumber = allMerges [0] [0].gamePiece.idNum;
				} else if (highestValueIDNumber < allMerges [0] [a].gamePiece.idNum) {
					highestValueIndex = a;
					highestValueIDNumber = allMerges [0] [a].gamePiece.idNum;
				}
			}

			Tile highestValueTile = allMerges [0] [highestValueIndex];

			
			CalculateMergeLevelAchieved(highestValueTile);

			
			allMerges [0].RemoveAt (highestValueIndex);

			
			highestValueTile.MergeToPiece (allMerges [0]);

			
			if ((int)highestValueTile.gamePiece.GetPieceValue () != 6) {
			
				GameStats.Instance.IncrementPlayerScore ((int)highestValueTile.gamePiece.GetPieceValue () + 1);
			}

			
			if ((int)highestValueTile.gamePiece.GetPieceValue () == 6) {

				StartCoroutine (TopLevelMerge (highestValueTile));
			}

			foundMatches = true;
	
			DoubleCheck ();

			yield return new WaitForSeconds (MERGE_DONE + 1f);		
		}
	}

	private IEnumerator DoubleCheck ()	{

		while (AreTilesMerging ()) {

			yield return new WaitForEndOfFrame ();
		}

		if (foundMatches) {
			//Check for more matches
			MatchAndClear ();
		} else {
			if (isGameOver ()) {
				uIManager.ShowGameOverPanel ();
			}
		}
	}

	 
	private IEnumerator TopLevelMerge (Tile highestValueTile)
	{		
	
		yield return new WaitForSeconds (MERGE_DONE);

	
		List<Tile> surroundingTiles = SurroundingTiles (highestValueTile.row, highestValueTile.column);

	
		SoundManager.Instance.PlaySoundEffect (SoundManager.TOP_LEVEL_MERGE, true, 0);

	
		for (int i = 0; i < surroundingTiles.Count; i++) {

			if (surroundingTiles [i].gamePiece != null) {
	
				surroundingTiles [i].MagicEffectPowerup ();
				surroundingTiles [i].DestroyGamePiece ();
			}
		}

	
		generatePieces.GeneratePowerup();
	}

	public bool ValidTile (int row, int column)
	{
	
		return (row >= 0 && row < NUM_OF_TILES_PER_LINE && column >= 0 && column < NUM_OF_TILES_PER_LINE);
	}

	
	private List<Tile> SurroundingTiles (int row, int column)
	{
		List<Tile> surroundingTiles = new List<Tile> ();

		//If a surrounding tile is on the game board, it can be added to the list
		if (ValidTile (row - 1, column - 1))
			surroundingTiles.Add (gameBoard [row - 1, column - 1]);
		if (ValidTile (row - 1, column))
			surroundingTiles.Add (gameBoard [row - 1, column]);
		if (ValidTile (row - 1, column + 1))
			surroundingTiles.Add (gameBoard [row - 1, column + 1]);
		if (ValidTile (row, column - 1))
			surroundingTiles.Add (gameBoard [row, column - 1]);
		if (ValidTile (row, column + 1))
			surroundingTiles.Add (gameBoard [row, column + 1]);
		if (ValidTile (row + 1, column - 1))
			surroundingTiles.Add (gameBoard [row + 1, column - 1]);
		if (ValidTile (row + 1, column))
			surroundingTiles.Add (gameBoard [row + 1, column]);
		if (ValidTile (row + 1, column + 1))
			surroundingTiles.Add (gameBoard [row + 1, column + 1]);
		if (ValidTile (row, column))
			surroundingTiles.Add (gameBoard [row, column]);

		return surroundingTiles;
	}

	public bool isGameOver ()
	{

		for (int x = 0; x < NUM_OF_TILES_PER_LINE; x++) {
			
			for (int y = 0; y < NUM_OF_TILES_PER_LINE; y++) {
				if (gameBoard [x, y].gamePiece == null) {
					return false;
				}
			}
		}
		return true;
	}

	/*
	 * Returns true if game pieces are currently merging into one.
	 */ 
	public bool AreTilesMerging ()
	{	

		for (int y = 0; y < NUM_OF_TILES_PER_LINE; y++) {
			for (int x = 0; x < NUM_OF_TILES_PER_LINE; x++) {
				if (gameBoard [x, y].IsMerging ()) {
					return true;
				}
			}
		}
		return false;
	}

	public void BombUsed (int row, int col)
	{
		List<Tile> surroundingTiles = SurroundingTiles (row, col);

		//Destroy all of the tiles in the surrounding tiles list
		for (int i = 0; i < surroundingTiles.Count; i++) {

			if (surroundingTiles [i].gamePiece != null) {
				//Play a magic effect on each tile
				surroundingTiles [i].MagicEffectPowerup ();
				//Destroy the game piece
				surroundingTiles [i].DestroyGamePiece ();
			}
		}
	}

	public void ClearRowUsed (int row, int col)
	{		
		List<Tile> allRowTiles = AllRowTiles (row, col);

		//Destroy all of the tiles in the row
		for (int i = 0; i < allRowTiles.Count; i++) {

			if (allRowTiles [i].gamePiece != null) {
				//Play a magic effect on each tile
				allRowTiles [i].MagicEffectPowerup ();
				//Destroy the game piece
				allRowTiles [i].DestroyGamePiece ();
			}
		}
	}

	public void ClearColumnUsed (int row, int col)
	{		
		List<Tile> allColumnTiles = AllColumnTiles (row, col);

		//Destroy all of the tiles in the column
		for (int i = 0; i < allColumnTiles.Count; i++) {

			if (allColumnTiles [i].gamePiece != null) {
				//Play a magic effect on each tile
				allColumnTiles [i].MagicEffectPowerup ();
				//Destroy the game piece
				allColumnTiles [i].DestroyGamePiece ();
			}
		}
	}

	private List<Tile> AllRowTiles (int row, int column)
	{
		List<Tile> allRowTiles = new List<Tile> ();

		for (int i = 0; i < NUM_OF_TILES_PER_LINE; i++)
		{
			if (ValidTile (row, i))
			{
				allRowTiles.Add (gameBoard [row, i]);
			}
		}

		return allRowTiles;
	}

	private List<Tile> AllColumnTiles (int row, int column)
	{
		List<Tile> allColumnTiles = new List<Tile> ();

		for (int i = 0; i < NUM_OF_TILES_PER_LINE; i++)
		{
			if (ValidTile (i, column))
			{
				allColumnTiles.Add (gameBoard [i, column]);
			}
		}

		return allColumnTiles;
	}

	public bool TestIfTwoAdjacentTilesAreEmpty (int x, int y, Tile[,] toTest, Tile currentTile)
	{

		if (toTest [x, y] == null) {
			return false;
		}
		if (currentTile == null) {
			if(toTest[x,y].gamePiece == null)
				currentTile = toTest [x, y];
			toTest [x, y] = null;
		}

		else if (((currentTile.gamePiece == null) && toTest [x, y].gamePiece == null)) {			
		
			return true;

		} else if (toTest [x, y].gamePiece != null) {
			
			return false;
		}

		//Test surrounding tiles if a return has not been reached
		if (x > 0)
			if (TestIfTwoAdjacentTilesAreEmpty (x - 1, y, toTest, currentTile)) {
				return true;
			}
		if (y > 0)
			if (TestIfTwoAdjacentTilesAreEmpty (x, y - 1, toTest, currentTile)) {
				return true;
			}
		if (x < NUM_OF_TILES_PER_LINE - 1)
			if (TestIfTwoAdjacentTilesAreEmpty (x + 1, y, toTest, currentTile)) {
				return true;
			}
		if (y < NUM_OF_TILES_PER_LINE - 1)
			if (TestIfTwoAdjacentTilesAreEmpty (x, y + 1, toTest, currentTile)) {
				return true;
			}
		return false;
	}

	public bool CheckForDoubleSpace ()
	{
		// Make a copy of the board to test
		Tile[,] toTest = new Tile[5, 5];
		CopyBoard (gameBoard, toTest);
		foundMatches = false;

		Tile currentTile = null;

		for (int y = 0; y < NUM_OF_TILES_PER_LINE; y++) {
			for (int x = 0; x < NUM_OF_TILES_PER_LINE; x++) {
				if (TestIfTwoAdjacentTilesAreEmpty (x, y, toTest, currentTile)) {
					return true;
				}
				currentTile = null;
			}
		}
		return false;
	}

	public void CalculateMergeLevelAchieved(Tile highestValueTile) {

		
		currentMergeLevel = (int)highestValueTile.gamePiece.GetPieceValue() + 1;

		
		if (currentMergeLevel > highestMergeLevelAchieved) {
			highestMergeLevelAchieved = currentMergeLevel;
		}
	}

	/*
	 * Returns the highest merge level achieved.
	 */ 
	public int GetHighestMergeLevelAchieved() {
		return highestMergeLevelAchieved;
	}
}
