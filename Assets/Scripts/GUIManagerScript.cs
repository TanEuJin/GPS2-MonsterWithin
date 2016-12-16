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

	public Button endTurnButton;

	public Text winloseText;
	public Text whoseTurn;

	public Image memoryShard;
	public Text memoryShardText;
	public Sprite[] memoryShardSprite;
	public string[] memoryShardDescription;
	public int memShardIndex;
	public bool gameEnd = false;

	void Start()
	{
		memShardIndex = 0;
		if(SceneManager.GetActiveScene().name == "_SCENE_")
		{
			memoryShardDescription[0] = " \"A rose for her was the best decision of my life.\" ";
			memoryShardDescription[1] = " \"Under the starry night, she gave me her locket. It was the first time we touched.\" ";
			memoryShardDescription[2] = " \"When she wore my ring, my heart filled with glee.\" ";
			memoryShardDescription[3] = " \"In her last letter, she told me to live for her and that is what I must do.\" ";
		}
	}

	void Update()
	{
		if(SceneManager.GetActiveScene().name == "_SCENE_")
		{
			memoryShard.sprite = memoryShardSprite[memShardIndex];
			memoryShardText.text = memoryShardDescription[memShardIndex];
		}

		if(!playerTurn)
		{
			EnemyManager.Instance.NextTurn();
			playerTurn = true;
		}

		if(Input.GetKeyUp(KeyCode.A))
		{
			memoryShard.gameObject.SetActive(true);
			memShardIndex++;
			if (memShardIndex > 3)
			{
				memShardIndex = 0;
			}
		}

		if(gameEnd)
		{
			WinLoseGame();
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
		if(SceneManager.GetActiveScene().name == "_SCENE_")
		{
			if (memoryShard.gameObject.activeSelf == true)
			{
				//! Player can do movement here
				//! As there is no memory shard descriptions
				memoryShard.GetComponent<Animator>().Play("FadeOut");
			}
		}

		if(PlayerManager.Instance.hasLight || PlayerManager.Instance.currentSanityLevel <= 1)
		{
			PlayerManager.Instance.playerLastKnownPos = PlayerManager.Instance.transform.position;
		}
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
				if(!PlayerManager.Instance.isHidden)
				{
					PlayerManager.Instance.ReduceSanity();
					UpdateSanityBar();

					if(PlayerManager.Instance.turnsInDark != 0)
					{
						PlayerManager.Instance.turnsInDark = 0;
					}
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

		if(SceneManagerScript.Instance.GetSceneName() != "TUTORIAL_SCENE_")
		{
			PlayerTurnText(false);
			PlayerManager.Instance.currentPath = null;
			endTurnButton.interactable = false;
			playerTurn = false;
		}
		else
		{
			if(TutorialManagerScript.Instance.isMonsterSpawned == false)
			{
				PlayerManager.Instance.NextTurn();
				PlayerManager.Instance.currentPath = null;
			}
			else
			{
				PlayerTurnText(false);
				PlayerManager.Instance.currentPath = null;
				endTurnButton.interactable = false;
				playerTurn = false;
			}
		}

	}

	public void PauseMenu()
	{
		Time.timeScale = 0;

		gameUI.gameObject.SetActive(false);
		pauseMenu.gameObject.SetActive(true);
		PlayerManager.Instance.enabled = false;
		SoundManagerScript.Instance.BookFlipUI.Play ();
	}

	public void ResumeGame()
	{
		Time.timeScale = 1;

		gameUI.gameObject.SetActive(true);
		pauseMenu.gameObject.SetActive(false);
		PlayerManager.Instance.enabled = true;
		SoundManagerScript.Instance.BookFlipUI.Play ();
	}

	public void BackMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
		SoundManagerScript.Instance.BookFlipUI.Play ();
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

	public void initalMemoryActivation()
	{
		memShardIndex = 0;
	}

	public void ActivateMemory()
	{
		if(shardCollected <= 2)
		{
			shardCollected++;
			EnemyManager.Instance.moveSpeed++;
		}


		if(shardCollected == 1)
		{
			// Trigger first memory
			memShardIndex = 1;
		}
		else if(shardCollected == 2)
		{
			// Trigger second memory
			memShardIndex = 2;
		}
		else if(shardCollected == 3)
		{
			// Trigger third memory
			memShardIndex = 3;
		}
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
