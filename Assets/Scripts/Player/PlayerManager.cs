﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
	private static PlayerManager mInstance;

	public static PlayerManager Instance
	{
		get
		{
			if(mInstance == null)
			{
				GameObject tempObject = GameObject.FindGameObjectWithTag("Player");

				if(tempObject == null)
				{
					GameObject obj = new GameObject("_PlayerManager");
					mInstance = obj.AddComponent<PlayerManager>();
					obj.tag = "PlayerManager";
				}
				else
				{
					mInstance = tempObject.GetComponent<PlayerManager>();
				}
			}
			return mInstance;
		}
	}

	// tileX and tileZ represent the correct map-tile position
	// for this piece.  Note that this doesn't necessarily mean
	// the world-space coordinates, because our map might be scaled
	// or offset or something of that nature.  Also, during movement
	// animations, we are going to be somewhere in between tiles.
	public int tileX;
	public int tileZ;

	public TileMap map;

	// Our pathfinding info.  Null if we have no destination ordered.
	public List<Node> currentPath = null;

	// How far this unit can move in one turn. Note that some tiles cost extra.
	public float remainingMovement = 2;

	//! Object Interaction
	public GameObject InteractButton, playerModel;
	public bool HideInteract, isHidden, detectionTrigger;
	public List<GameObject>Interact = new List<GameObject>();
	public List<GameObject>HideObject = new List<GameObject>();
	public Vector3 playerLastKnownPos;
	//public GameObject[] Interact;
	//public GameObject[] HideObject;

	// Sanity
	public bool hasLight, AlreadyScreamed, AlreadyMoved = false;
	public int currentSanityLevel = 4;
	public int maxSanityLevel = 6;
	public bool enemyInRange = false;
	public int turnsInDark = 0;
	public int maxTurnInDark = 3;
	Animator anim;
	Transform playerTransform;

	//! Raycast
	public LayerMask LayerTile;
	RaycastHit tileHit;

	public float speed = 1.0f;

	bool gotCaught = false;
	float loseDelayTimer = 0.0f;
	public float loseDelayDuration = 1.0f;

	void Start()
	{
		GUIManagerScript.Instance.UpdateSanityBar();
		GUIManagerScript.Instance.UpdateMovementSteps();

		anim = GetComponentInChildren<Animator>();
		playerTransform = transform.GetChild(0).gameObject.transform;

		LayerTile = LayerMask.NameToLayer ("ClickableTile");
		LayerTile.value = 1<<LayerTile.value;
	}

	void Update()
	{
		//if (remainingMovement <= 0) {
		//	GUIManagerScript.Instance.EndTurn ();
		//}

		if(!enabled)
		{
			return;
		}

		if(Input.GetMouseButtonUp(0) && GUIManagerScript.Instance.endTurnButton.interactable != false)
		{
			if (isHidden == false) 
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				if(EventSystem.current.IsPointerOverGameObject())
				{
					return;
				}

				if(Physics.Raycast(ray,out tileHit, Mathf.Infinity, LayerTile.value))
				{
					if(tileHit.transform.gameObject != null)
					{
						if (AlreadyMoved == false) 
						{
							if (remainingMovement == 1) 
							{
								SoundManagerScript.Instance.player1Step.Play ();
							}

							if (remainingMovement == 2) 
							{
								SoundManagerScript.Instance.player2Step.Play ();
							}

							if (remainingMovement == 3) 
							{
								SoundManagerScript.Instance.player3Step.Play ();
							}

							if (remainingMovement == 4) {
								SoundManagerScript.Instance.player4Step.Play ();
							}

							//SoundManagerScript.Instance.PlayerMove.Play();
							AlreadyMoved = true;
						}

						map.PlayerGeneratePathTo ((int)tileHit.transform.position.x, (int)tileHit.transform.position.z);
					}
				}
			}

		}

		// NOTE: This won't appear in the actual game view.
		if(currentPath != null)
		{
			int currNode = 0;

			while( currNode < currentPath.Count-1 )
			{
				currNode++;
			}
		}

		// Have we moved our visible piece close enough to the target tile that we can
		// advance to the next step in our pathfinding?
		if(Vector3.Distance(transform.position, map.TileCoordToWorldCoord( tileX, tileZ )) < 0.1f)
		{
			AdvancePathing();
		}

		// Smoothly animate towards the correct map tile.
		transform.position = Vector3.Lerp(transform.position, map.TileCoordToWorldCoord( tileX, tileZ ), speed * Time.fixedDeltaTime);

		if(gotCaught)
		{
			loseDelayTimer += Time.deltaTime;

			if(loseDelayTimer >= loseDelayDuration)
			{
				GUIManagerScript.Instance.WinLoseGame();
			}

			if(GUIManagerScript.Instance.losingMenu.GetComponent<CanvasGroup>().alpha == 1)
			{
				gotCaught = false;
			}
		}
	}

	// Advances our pathfinding progress by one tile.
	void AdvancePathing()
	{
		if(currentPath == null || remainingMovement <= 0)
		{
			anim.SetBool("IsWalk", false);
			return;
		}

		// Teleport us to our correct "current" position, in case we haven't finished the animation yet.
		transform.position = map.TileCoordToWorldCoord( tileX, tileZ );

		// Get cost from current tile to next tile
		remainingMovement -= map.CostToEnterTile(currentPath[0].x, currentPath[0].z, currentPath[1].x, currentPath[1].z );

		// Move us to the next tile in the sequence
		if(tileX + 1 == currentPath[1].x)
		{
			playerTransform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
		}
		else if(tileX - 1 == currentPath[1].x)
		{
			playerTransform.rotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
		}
		else if(tileZ + 1 == currentPath[1].z)
		{
			playerTransform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
		}
		else if(tileZ - 1 == currentPath[1].z)
		{
			playerTransform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
		}

		tileX = currentPath[1].x;
		tileZ = currentPath[1].z;
		anim.SetBool("IsWalk", true);

		if(CheckLosingCondition() == true)
		{
			remainingMovement = 0;
			gotCaught = true;
			return;
		}

		// Remove the old "current" tile from the pathfinding list
		currentPath.RemoveAt(0);

		if(currentPath.Count == 1)
		{
			// We only have one tile left in the path, and that tile MUST be our ultimate
			// destination -- and we are standing on it!
			// So let's just clear our pathfinding info.
			currentPath = null;
		}

		//GUIManagerScript.Instance.movesCount.text = "Movement Steps: " + remainingMovement;
		GUIManagerScript.Instance.UpdateMovementSteps();
	}

	// The "Next Turn" button calls this.
	public void NextTurn()
	{
		AlreadyMoved = false;
		// Make sure to wrap-up any outstanding movement left over.
		while(currentPath != null && remainingMovement > 0)
		{
			AdvancePathing();
		}

		// Reset our available movement points.
		if (currentSanityLevel == 1) {
			if (AlreadyScreamed == false) 
			{
				SoundManagerScript.Instance.Horrified.Play();
				SoundManagerScript.Instance.sanityLow.TransitionTo (SoundManagerScript.Instance.m_TransitionIn);
				AlreadyScreamed = true;
			}

			if (AlreadyScreamed == true) 
			{
				//! Dont play audio
			}
		}

		if(currentSanityLevel <= 2)
		{
			remainingMovement = 2;

		}
		else if(currentSanityLevel >=3 && currentSanityLevel <= 4)
		{
			SoundManagerScript.Instance.notSeenByEnemy.TransitionTo (SoundManagerScript.Instance.m_TransitionOut);
			AlreadyScreamed = false;
			remainingMovement = 3;
		}
		else if(currentSanityLevel >=5 && currentSanityLevel <= 6)
		{
			remainingMovement = 4;
		}

		GUIManagerScript.Instance.UpdateMovementSteps();
		GUIManagerScript.Instance.PlayerTurnText(true);
	}

	public void IncreaseSanity()
	{
		if(currentSanityLevel + 1 <= maxSanityLevel)
		{
			currentSanityLevel ++;
		}
	}

	public void ReduceSanity()
	{
		if(currentSanityLevel - 1 >= 1)
		{
			currentSanityLevel --;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		/*if (other.CompareTag ("HideObject").Equals(HideObject[1])) 
		{
			Debug.Log ("Test!");
			Interact [1].SetActive (true);
			//InteractButton.SetActive(true);

			for (int i = 0; i < HideObject.Count; i++) 
			{
				Debug.Log (HideObject[i]);
				if(Interact [i].Equals(HideObject[i]))
				{
					HideInteract = true;
					Interact [i].SetActive (true);
				}

			}

		}*/

		for (int i = 0; i < HideObject.Count; i++) 
		{
			if (other.gameObject == HideObject [i]) 
			{
				HideInteract = true;
				Interact [i].SetActive (true);
			}
		}

		//if (other.gameObject == HideObject [1]) {
		//	Debug.Log ("Test!");
		//}

		if (other.CompareTag ("Enemy")) 
		{
			if (detectionTrigger == false) {
				SoundManagerScript.Instance.seenByTheEnemy.Play ();
				SoundManagerScript.Instance.playTransition ();
				detectionTrigger = true;
			}
			SoundManagerScript.Instance.seenByEnemy.TransitionTo (SoundManagerScript.Instance.m_TransitionIn);
		}

		if (other.CompareTag ("EnemyDread")) 
		{
			SoundManagerScript.Instance.Dread.Play();
			SoundManagerScript.Instance.proximityDread.TransitionTo (SoundManagerScript.Instance.m_TransitionIn);
		}
	}

	void OnTriggerExit(Collider other)
	{
		for (int i = 0; i < HideObject.Count; i++) 
		{
			if (other.gameObject == HideObject [i]) 
			{
				
				HideInteract = false;
				Interact [i].SetActive (false);
			}
		}

		/*if (other.gameObject.CompareTag ("HideObject")) 
		{
			HideInteract = false;
			InteractButton.SetActive(false);
		}*/


		if (other.CompareTag ("EnemyDread")) 
		{
			SoundManagerScript.Instance.Dread.Stop();
			SoundManagerScript.Instance.proximityDread.TransitionTo (SoundManagerScript.Instance.m_TransitionOut);
			SoundManagerScript.Instance.notSeenByEnemy.TransitionTo (SoundManagerScript.Instance.m_TransitionIn);

			if (detectionTrigger == true) {
				detectionTrigger = false;
			}

		}
	}

	bool CheckLosingCondition()
	{
		if(tileX - 1 == EnemyManager.Instance.tileX && tileZ == EnemyManager.Instance.tileZ ||
			tileX + 1 == EnemyManager.Instance.tileX && tileZ == EnemyManager.Instance.tileZ)
		{
			return true;
		}
		else if(tileX == EnemyManager.Instance.tileX && tileZ + 1 == EnemyManager.Instance.tileZ || 
			tileX == EnemyManager.Instance.tileX && tileZ  - 1 == EnemyManager.Instance.tileZ)
		{
			return true;
		}
		else if(tileX == EnemyManager.Instance.tileX - 1 && tileZ == EnemyManager.Instance.tileZ - 1 ||
			tileX == EnemyManager.Instance.tileX - 1 && tileZ == EnemyManager.Instance.tileZ + 1)
		{
			return true;
		}
		else if(tileX == EnemyManager.Instance.tileX + 1 && tileZ == EnemyManager.Instance.tileZ - 1 ||
			tileX == EnemyManager.Instance.tileX + 1 && tileZ == EnemyManager.Instance.tileZ + 1)
		{
			return true;
		}

		return  false;
	}
}