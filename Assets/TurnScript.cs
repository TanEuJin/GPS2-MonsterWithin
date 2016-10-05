using UnityEngine;
using System.Collections;

public class TurnScript : MonoBehaviour 
{
	bool turnPlayer = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (turnPlayer == false) {
			Debug.Log("Enemy Turn");
			EnemyManager.Instance.NextTurn ();
			turnPlayer = true;
		}
	}

	public void NTurn()
	{
		if (turnPlayer == true) 
		{
			Debug.Log("Player Turn");
			PlayerManager.Instance.NextTurn ();
			PlayerManager.Instance.currentPath = null;
			turnPlayer = false;
		}
	}
}
