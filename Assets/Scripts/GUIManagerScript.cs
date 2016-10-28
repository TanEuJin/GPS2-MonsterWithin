﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GUIManagerScript : MonoBehaviour 
{
	private static GUIManagerScript mInstance;

	public static GUIManagerScript Instance
	{
		get
		{
			if(mInstance == null)
			{
				GameObject tempObject = GameObject.FindGameObjectWithTag("GUIManager");

				if(tempObject == null)
				{
					GameObject obj = new GameObject("_GUIManager");
					mInstance = obj.AddComponent<GUIManagerScript>();
					obj.tag = "GUIManager";
				}
				else
				{
					mInstance = tempObject.GetComponent<GUIManagerScript >();
				}
			}
			return mInstance;
		}
	}

	public Image sanityBar;
	public Text movesCount;

	public bool turnPlayer = true;

	public Canvas gameUI;
	public Canvas pauseMenu;
	public Canvas losingMenu;

	void Start()
	{
		Screen.orientation = ScreenOrientation.Landscape;
	}

	void Update()
	{
		if(turnPlayer == false)
		{
			EnemyManager.Instance.NextTurn();
			turnPlayer = true;
		}
	}

	public void UpdateSanityBar()
	{
		sanityBar.fillAmount = (PlayerManager.Instance.currentSanityLevel * 1.0f / PlayerManager.Instance.maxSanityLevel * 1.0f);

		if(PlayerManager.Instance.currentSanityLevel >=0 && PlayerManager.Instance.currentSanityLevel <=2)
		{
			sanityBar.color = Color.red;
		}
		else if(PlayerManager.Instance.currentSanityLevel >=3 && PlayerManager.Instance.currentSanityLevel <=4)
		{
			sanityBar.color = Color.yellow;
		}
		else if(PlayerManager.Instance.currentSanityLevel >=5 && PlayerManager.Instance.currentSanityLevel <=6)
		{
			sanityBar.color = Color.green;
		}
	}

	/*public void ClosetInteract()
	{
		Debug.Log ("Test");
		if (PlayerManager.Instance.closetInteract == true) 
		{
			if (PlayerManager.Instance.isHidden == false)
			{
				PlayerManager.Instance.transform.position = PlayerManager.Instance.Closet.transform.position;
				PlayerManager.Instance.isHidden = true;
			} 

			else if (PlayerManager.Instance.isHidden == true) 
			{
				PlayerManager.Instance.transform.position = Vector3.forward * 1.0f;
				PlayerManager.Instance.isHidden = false;
			}
		}
	}*/

	public void HideInteract()
	{
		Debug.Log ("Test");
		if (PlayerManager.Instance.HideInteract == true) 
		{
			if (PlayerManager.Instance.isHidden == false)
			{
				PlayerManager.Instance.playerModel.SetActive(false);
				PlayerManager.Instance.isHidden = true;
			} 

			else if (PlayerManager.Instance.isHidden == true) 
			{
				PlayerManager.Instance.playerModel.SetActive(true);
				PlayerManager.Instance.isHidden = false;
			}
		}
	}

	public void DoorInteract()
	{
		Debug.Log ("Test");
		if (PlayerManager.Instance.doorInteract == true) 
		{
			if (PlayerManager.Instance.keys == 1)
			{
				SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_DOOROPEN);
				PlayerManager.Instance.Door.transform.position = Vector3.up * 1.0f;
				PlayerManager.Instance.keys -= 1;
			}

			else if (PlayerManager.Instance.keys < 1) 
			{
				Debug.Log("Insufficient Keys!");
			}
		}
	}

	public void EndTurn()
	{
		if (turnPlayer == true) 
		{
			if(PlayerManager.Instance.hasLight)
			{
				if(PlayerManager.Instance.turnsInDark != 0)
				{
					PlayerManager.Instance.turnsInDark = 0;
				}

				PlayerManager.Instance.IncreaseSanity();
				UpdateSanityBar();
			}
			else
			{
				PlayerManager.Instance.turnsInDark ++;

				if(PlayerManager.Instance.turnsInDark == PlayerManager.Instance.maxTurnInDark)
				{
					PlayerManager.Instance.ReduceSanity();
					UpdateSanityBar();
					PlayerManager.Instance.turnsInDark = 0;
				}
			}

			if(PlayerManager.Instance.enemyInRange)
			{
				PlayerManager.Instance.ReduceSanity();
				UpdateSanityBar();
			}

			PlayerManager.Instance.NextTurn ();
			PlayerManager.Instance.currentPath = null;

			turnPlayer = false;
		}
	}

	public void PauseMenu()
	{
		Time.timeScale = 0;

		gameUI.gameObject.SetActive(false);
		pauseMenu.gameObject.SetActive(true);
		PlayerManager.Instance.enabled = false;
	}

	public void ResumeGame()
	{
		Time.timeScale = 1;

		gameUI.gameObject.SetActive(true);
		pauseMenu.gameObject.SetActive(false);
		PlayerManager.Instance.enabled = true;
	}

	public void LoseGame()
	{
		Time.timeScale = 0;

		gameUI.gameObject.SetActive(false);
		losingMenu.gameObject.SetActive(false);
		PlayerManager.Instance.enabled = false;
	}
}
