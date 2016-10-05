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
			EnemyManager.Instance.NextTurn ();
			turnPlayer = true;
		}
	}

	public void NTurn()
	{
		if (turnPlayer == true) 
		{
			if(PlayerManager.Instance.gotOtherLight == true)
			{
				if(PlayerManager.Instance.currentSanityLevel + 1 <= PlayerManager.Instance.maxSanityLevel)
				{
					PlayerManager.Instance.currentSanityLevel ++;
				}
			}
			else
			{
				if(PlayerManager.Instance.lanternOn == false)
				{
					if(PlayerManager.Instance.currentSanityLevel - 1 >= 0)
					{
						PlayerManager.Instance.currentSanityLevel --;
					}
				}
			}

			PlayerManager.Instance.NextTurn ();
			PlayerManager.Instance.currentPath = null;
			GUIManagerScript.Instance.UpdateSanityBar();

			turnPlayer = false;
		}
	}
}
