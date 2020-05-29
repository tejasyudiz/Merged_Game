using UnityEngine;
using System.Collections;

public class GameStats {

	private static GameStats instance;
	private int playerScore = 0;
	private const int SCORE_MULTIPLIER = 10;

	private GameStats ()
	{

	}


	public static GameStats Instance {
		get {
			if (instance == null) {
				instance = new GameStats ();
			}
			return instance;
		}
	}

	
	public string PrintGameStats ()
	{
		string gameStats = "";
		gameStats += "PlayerScore: " + playerScore + "\n";
		return gameStats;
	}

	
	public void ResetPlayerScore ()
	{
		playerScore = 0;
	}

	
	public int GetPlayerScore ()
	{
		return playerScore;
	}

 
	public void IncrementPlayerScore (int amount)
	{
		playerScore += amount * SCORE_MULTIPLIER;
	}
}