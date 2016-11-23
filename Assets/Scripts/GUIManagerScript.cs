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

	public int shardCollected = 0;

	public Image currentMovementCountImage;
	public Sprite[] movementCountImagesList;

	public Image currentSanityImage;
	public Sprite[] sanityImagesList;

	public bool playerTurn = true;

	public Canvas gameUI;
	public Canvas pauseMenu;
	public Canvas losingMenu;
	public Canvas memoryCanvas;

	public Button endTurnButton;

	public Text winloseText;
	public Text whoseTurn;

	public Image memoryPic;
	public Image[] memoriesPicsList;
	public Text memoryDescription;

	void Start()
	{
		
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
		if(PlayerManager.Instance.currentSanityLevel == 1)
		{
			currentSanityImage.sprite = sanityImagesList[0];
		}
		else if(PlayerManager.Instance.currentSanityLevel == 2)
		{
			currentSanityImage.sprite = sanityImagesList[1];
		}
		else if(PlayerManager.Instance.currentSanityLevel == 3)
		{
			currentSanityImage.sprite = sanityImagesList[2];
		}
		else if(PlayerManager.Instance.currentSanityLevel == 4)
		{
			currentSanityImage.sprite = sanityImagesList[3];
		}
		else if(PlayerManager.Instance.currentSanityLevel == 5)
		{
			currentSanityImage.sprite = sanityImagesList[4];
		}
		else if(PlayerManager.Instance.currentSanityLevel == 6)
		{
			currentSanityImage.sprite = sanityImagesList[5];
		}
	}

	public void UpdateMovementSteps()
	{
		if(PlayerManager.Instance.remainingMovement == 0)
		{
			currentMovementCountImage.sprite = movementCountImagesList[0];
		}
		else if(PlayerManager.Instance.remainingMovement == 1)
		{
			currentMovementCountImage.sprite = movementCountImagesList[1];
		}
		else if(PlayerManager.Instance.remainingMovement == 2)
		{
			currentMovementCountImage.sprite = movementCountImagesList[2];
		}
		else if(PlayerManager.Instance.remainingMovement == 3)
		{
			currentMovementCountImage.sprite = movementCountImagesList[3];
		}
		else if(PlayerManager.Instance.remainingMovement == 4)
		{
			currentMovementCountImage.sprite = movementCountImagesList[4];
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
		if(PlayerManager.Instance.hasLight && PlayerManager.Instance.isHidden == false)
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
			if(PlayerManager.Instance.enemyInRange)
			{
				PlayerManager.Instance.ReduceSanity();
				UpdateSanityBar();

				if(PlayerManager.Instance.turnsInDark != 0)
				{
					PlayerManager.Instance.turnsInDark = 0;
				}
			}
			else
			{
				PlayerManager.Instance.turnsInDark ++;

				if(PlayerManager.Instance.turnsInDark == PlayerManager.Instance.maxTurnInDark)
				{
					PlayerManager.Instance.turnsInDark = 0;
					PlayerManager.Instance.ReduceSanity();
					UpdateSanityBar();
				}
			}
		}

		PlayerTurnText(false);
		PlayerManager.Instance.currentPath = null;
		endTurnButton.interactable = false;
		playerTurn = false;

		//! To Optimize
		GameObject.Find("Player").GetComponent<HighlightTiles>().FlushList();
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

	public void BackMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}

	public void WinLoseGame()
	{
		gameUI.gameObject.SetActive(false);
		losingMenu.gameObject.SetActive(true);

		losingMenu.GetComponent<CanvasGroup>().alpha += Time.deltaTime/3;

		if(shardCollected == 3)
		{
			winloseText.text = "You Won";
		}
		else if(shardCollected <= 2)
		{
			winloseText.text = "You Died";
		}

		if(losingMenu.GetComponent<CanvasGroup>().alpha >= 0.75f)
		{
			losingMenu.GetComponent<CanvasGroup>().interactable = true;
		}
	}

	public void ActivateMemory()
	{
		gameUI.gameObject.SetActive(false);
		memoryCanvas.gameObject.SetActive(true);
		if(shardCollected <= 2)
		{
			shardCollected++;
			EnemyManager.Instance.moveSpeed++;
		}

		if(shardCollected == 1)
		{
			// Trigger first memory
		}
		else if(shardCollected == 2)
		{
			// Trigger second memory
		}
		else if(shardCollected == 3)
		{
			// Trigger third memory followed by winning condition upon pressing continue
			WinLoseGame();
		}
	}

	public void DeactivateMemory()
	{
		gameUI.gameObject.SetActive(true);
		memoryCanvas.gameObject.SetActive(false);
	}

	public void PlayerTurnText(bool isPlayer)
	{
		if(isPlayer)
		{
			whoseTurn.text = "Player's Turn";
			whoseTurn.enabled = true;
		}
		else
		{
			whoseTurn.text = "Monster's Turn";
			whoseTurn.enabled = true;
		}
	}
}
