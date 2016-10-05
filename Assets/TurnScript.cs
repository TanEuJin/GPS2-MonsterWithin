using UnityEngine;
using System.Collections;

public class TurnScript : MonoBehaviour 
{
	bool turnPlayer = true;

	void Start () 
	{
	
	}

	void Update () 
	{
		if (turnPlayer == false)
		{
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

			if(PlayerManager.Instance.gotOtherLight == true)
			{
				if(PlayerManager.Instance.currentSanityLevel + 1 <= PlayerManager.Instance.maxSanityLevel)
				{
					PlayerManager.Instance.currentSanityLevel ++;
				}
			}
			else
			{
				if(PlayerManager.Instance.flashLightOn == false)
				{
					if(PlayerManager.Instance.currentSanityLevel - 1 >= 0)
					{
						PlayerManager.Instance.currentSanityLevel --;
					}
				}
			}
			GUIManagerScript.Instance.UpdateSanityBar();

			turnPlayer = false;
		}
	}
}
