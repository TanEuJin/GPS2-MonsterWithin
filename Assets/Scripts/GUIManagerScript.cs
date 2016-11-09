using UnityEngine;
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

	public bool playerTurn = true;

	public Canvas gameUI;
	public Canvas pauseMenu;
	public Canvas losingMenu;

	public Button endTurnButton;

	void Start()
	{
		Screen.orientation = ScreenOrientation.Landscape;
	}

	void Update()
	{
		if(!playerTurn)
		{
			EnemyManager.Instance.NextTurn();
			playerTurn = true;
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

	public void HideInteract()
	{
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
		
	public void EndTurn()
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

		PlayerManager.Instance.currentPath = null;
		endTurnButton.interactable = false;
		playerTurn = false;
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
		gameUI.gameObject.SetActive(false);
		losingMenu.gameObject.SetActive(true);

		losingMenu.GetComponent<CanvasGroup>().alpha += Time.deltaTime/3;

		if(losingMenu.GetComponent<CanvasGroup>().alpha >= 0.75f)
		{
			losingMenu.GetComponent<CanvasGroup>().interactable = true;
		}
	}
}
