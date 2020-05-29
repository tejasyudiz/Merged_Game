using UnityEngine;
using System.Collections;


public enum PowerupType 
{
	Bomb = 0,
	ClearColumn = 1,
	ClearRow = 2,
	ClearRowAndColumn = 3,

};

public class Powerup : MonoBehaviour {
	
	

	public PowerupType powerupType;
	public RectTransform rectTransform;
	public bool isDraggable;
	private Vector2 centerPivot = new Vector2 (0.5f, 0.5f);

	void Start () {

		rectTransform = GetComponent<RectTransform>();

		
		isDraggable = true;
	}


	public void CenterImage() {
		rectTransform.pivot = centerPivot;
		rectTransform.anchorMin = rectTransform.anchorMax = centerPivot;
	}

	 
	public PowerupType GetPowerupType() {
		return powerupType;
	}
}
