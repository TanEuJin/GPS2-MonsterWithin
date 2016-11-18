using UnityEngine;
using System.Collections.Generic;

public enum EnemyBehavior
{
	PATROLLING 		= 0,
	CHASING			= 1,
	CHASING_LOST	= 2,
	ROAM_DINING		= 3,
	ROAM_KITCHEN	= 4,
	ROAM_MORNING	= 5,
	ROAM_LIVING		= 6,
	ROAM_LIBRARY	= 7,
	ROAM_BEDROOM	= 8,

	TOTAL = 10
}

public class EnemyManager : MonoBehaviour
{
	private static EnemyManager mInstance;

	public static EnemyManager Instance
	{
		get
		{
			if(mInstance == null)
			{
				GameObject tempObject = GameObject.FindGameObjectWithTag("Enemy");

				if(tempObject == null)
				{
					GameObject obj = new GameObject("_EnemyManager");
					mInstance = obj.AddComponent<EnemyManager>();
					obj.tag = "EnemyManager";
				}
				else
				{
					mInstance = tempObject.GetComponent<EnemyManager>();
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
	public int moveSpeed;
	public int currentRoam_Value = 0; // for ROAM_ behaviour
	public bool reachedRoam_ = false;
	public float remainingMovement = 2;
	public float distFromPlayer;
	public EnemyBehavior state;

	bool playerDetectable = false;
	Animator anim;
	Transform modalTransform;

	int RandChance;
	bool caughtPlayer = false;
	float loseDelayTimer = 0.0f;
	public float loseDelayDuration = 1.0f;

	void Start()
	{
		anim = GetComponentInChildren<Animator>();
		modalTransform = transform.GetChild(0).gameObject.transform;
		moveSpeed = 2;
	}

	void Update()
	{
		if(!enabled)
		{
			return;
		}

		RandomChanceGenerator();
		PlayerDetection();

		//! Setting Enemy Behavior
		if(playerDetectable)
		{
			state = EnemyBehavior.CHASING;
		}
		else
		{
			if(ShardScript.Instance.location == ShardLocation.DINING)
			{
				state = EnemyBehavior.ROAM_DINING;
			}
			else if(ShardScript.Instance.location == ShardLocation.KITCHEN)
			{
				state = EnemyBehavior.ROAM_KITCHEN;
			}
			else if(ShardScript.Instance.location == ShardLocation.MORNING)
			{
				state = EnemyBehavior.ROAM_MORNING;
			}
			else if(ShardScript.Instance.location == ShardLocation.LIVING)
			{
				state = EnemyBehavior.ROAM_LIVING;
			}
			else if(ShardScript.Instance.location == ShardLocation.LIBRARY)
			{
				state = EnemyBehavior.ROAM_LIBRARY;
			}
			else if(ShardScript.Instance.location == ShardLocation.BEDROOM)
			{
				state = EnemyBehavior.ROAM_BEDROOM;
			}
			else
			{
				state = EnemyBehavior.PATROLLING;
			}
		}

		// Draw our debug line showing the pathfinding!
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
		transform.position = Vector3.Lerp(transform.position, map.TileCoordToWorldCoord( tileX, tileZ ), 5f * Time.deltaTime);

		if(caughtPlayer)
		{
			loseDelayTimer += Time.deltaTime;

			if(loseDelayTimer >= loseDelayDuration)
			{
				GUIManagerScript.Instance.WinLoseGame();
			}

			if(GUIManagerScript.Instance.losingMenu.GetComponent<CanvasGroup>().alpha == 1)
			{
				caughtPlayer = false;
			}
		}
	}

	// Advances our pathfinding progress by one tile.
	public void AdvancePathing()
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

		// Reenable the End Turn button once enemy finish his movement
		if(remainingMovement == 0)
		{
			GUIManagerScript.Instance.endTurnButton.interactable = true;
			PlayerManager.Instance.NextTurn();
		}
		
		// Move us to the next tile in the sequence
		if(tileX + 1 == currentPath[1].x)
		{
			modalTransform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
		}
		else if(tileX - 1 == currentPath[1].x)
		{
			modalTransform.rotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
		}
		else if(tileZ + 1 == currentPath[1].z)
		{
			modalTransform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
		}
		else if(tileZ - 1 == currentPath[1].z)
		{
			modalTransform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
		}

		tileX = currentPath[1].x;
		tileZ = currentPath[1].z;
		anim.SetBool("IsWalk", true);

		if(CheckLosingCondition() == true)
		{
			remainingMovement = 0;
			caughtPlayer = true;
			return;
		}
		
		// Remove the old "current" tile from the pathfinding list
		currentPath.RemoveAt(0);
		
		if(currentPath.Count == 1)
		{
			// We only have one tile left in the path, and that tile MUST be our ultimate
			// destination -- and we are standing on it!
			// So let's just clear our pathfinding info.
			GUIManagerScript.Instance.endTurnButton.interactable = true; // Reenable the End Turn button once enemy finish his movement
			PlayerManager.Instance.NextTurn();
			currentPath = null;
		}
	}

	void RandomChanceGenerator()
	{
		RandChance = Random.Range(1,5);
	}

	void EnemyBehaviourCheck()
	{
		if(EnemyManager.Instance.tileX == (int)map.waypoints[currentRoam_Value].x && EnemyManager.Instance.tileZ == (int)map.waypoints[currentRoam_Value].z)
		{
			reachedRoam_ = true;
		}

		// Behavior switch
		if(state == EnemyBehavior.PATROLLING)
		{
			reachedRoam_ = true;

			if(EnemyManager.Instance.tileX == (int)map.waypoints[map.waypointCounter].x && EnemyManager.Instance.tileZ == (int)map.waypoints[map.waypointCounter].z)
			{
				map.waypointCounter++;
			}

			map.GetComponent<TileMap>().EnemyGeneratePathTo((int)map.waypoints [map.waypointCounter].x, (int)map.waypoints [map.waypointCounter].z);

		}
		else if(state == EnemyBehavior.CHASING)
		{
			reachedRoam_ = true;

			if(EnemyManager.Instance.transform.position != PlayerManager.Instance.transform.position)
			{
				map.GetComponent<TileMap>().EnemyGeneratePathTo((int)PlayerManager.Instance.transform.position.x, (int)PlayerManager.Instance.transform.position.z);
			}
		}
		else if(state == EnemyBehavior.ROAM_DINING)
		{
			if(reachedRoam_)
			{
				if(EnemyManager.Instance.tileX == (int)map.waypoints[0].x && EnemyManager.Instance.tileZ == (int)map.waypoints[0].z)
				{
					currentRoam_Value = 1;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[1].x && EnemyManager.Instance.tileZ == (int)map.waypoints[1].z)
				{
					currentRoam_Value = 3;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[3].x && EnemyManager.Instance.tileZ == (int)map.waypoints[3].z)
				{
					currentRoam_Value = 22;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[22].x && EnemyManager.Instance.tileZ == (int)map.waypoints[22].z)
				{
					currentRoam_Value = 21;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[21].x && EnemyManager.Instance.tileZ == (int)map.waypoints[21].z)
				{
					currentRoam_Value = 19;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[19].x && EnemyManager.Instance.tileZ == (int)map.waypoints[19].z)
				{
					currentRoam_Value = 11;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[11].x && EnemyManager.Instance.tileZ == (int)map.waypoints[11].z)
				{
					currentRoam_Value = 7;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[7].x && EnemyManager.Instance.tileZ == (int)map.waypoints[7].z)
				{
					currentRoam_Value = 8;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[8].x && EnemyManager.Instance.tileZ == (int)map.waypoints[8].z)
				{
					if(RandChance == 1)
					{
						currentRoam_Value = 0;
					}
					else
					{
						currentRoam_Value = 12;
					}
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[12].x && EnemyManager.Instance.tileZ == (int)map.waypoints[12].z)
				{
					currentRoam_Value = 10;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[10].x && EnemyManager.Instance.tileZ == (int)map.waypoints[10].z)
				{
					currentRoam_Value = 6;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[6].x && EnemyManager.Instance.tileZ == (int)map.waypoints[6].z)
				{
					currentRoam_Value = 5;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[5].x && EnemyManager.Instance.tileZ == (int)map.waypoints[5].z)
				{
					currentRoam_Value = 2;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[2].x && EnemyManager.Instance.tileZ == (int)map.waypoints[2].z)
				{
					currentRoam_Value = 0;
				}
				else
				{
					currentRoam_Value = 0;
				}

				reachedRoam_ = false;
			}

			map.GetComponent<TileMap>().EnemyGeneratePathTo((int)map.waypoints[currentRoam_Value].x, (int)map.waypoints[currentRoam_Value].z);
		}
		else if(state == EnemyBehavior.ROAM_KITCHEN)
		{
			if(reachedRoam_)
			{
				if(EnemyManager.Instance.tileX == (int)map.waypoints[11].x && EnemyManager.Instance.tileZ == (int)map.waypoints[11].z)
				{
					currentRoam_Value = 7;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[7].x && EnemyManager.Instance.tileZ == (int)map.waypoints[7].z)
				{
					currentRoam_Value = 8;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[8].x && EnemyManager.Instance.tileZ == (int)map.waypoints[8].z)
				{
					currentRoam_Value = 12;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[12].x && EnemyManager.Instance.tileZ == (int)map.waypoints[12].z)
				{
					currentRoam_Value = 10;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[10].x && EnemyManager.Instance.tileZ == (int)map.waypoints[10].z)
				{
					currentRoam_Value = 6;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[6].x && EnemyManager.Instance.tileZ == (int)map.waypoints[6].z)
				{
					currentRoam_Value = 4;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[4].x && EnemyManager.Instance.tileZ == (int)map.waypoints[4].z)
				{
					currentRoam_Value = 3;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[3].x && EnemyManager.Instance.tileZ == (int)map.waypoints[3].z)
				{
					if(RandChance == 1)
					{
						currentRoam_Value = 8;
					}
					else
					{
						currentRoam_Value = 22;
					}
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[22].x && EnemyManager.Instance.tileZ == (int)map.waypoints[22].z)
				{
					currentRoam_Value = 21;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[21].x && EnemyManager.Instance.tileZ == (int)map.waypoints[21].z)
				{
					currentRoam_Value = 19;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[19].x && EnemyManager.Instance.tileZ == (int)map.waypoints[19].z)
				{
					currentRoam_Value = 13;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[13].x && EnemyManager.Instance.tileZ == (int)map.waypoints[13].z)
				{
					currentRoam_Value = 9;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[9].x && EnemyManager.Instance.tileZ == (int)map.waypoints[9].z)
				{
					currentRoam_Value = 11;
				}
				else
				{
					currentRoam_Value = 11;
				}

				reachedRoam_ = false;
			}

			map.GetComponent<TileMap>().EnemyGeneratePathTo((int)map.waypoints[currentRoam_Value].x, (int)map.waypoints[currentRoam_Value].z);
		}
		else if(state == EnemyBehavior.ROAM_MORNING)
		{
			if(reachedRoam_)
			{
				if(EnemyManager.Instance.tileX == (int)map.waypoints[13].x && EnemyManager.Instance.tileZ == (int)map.waypoints[13].z)
				{
					currentRoam_Value = 14;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[14].x && EnemyManager.Instance.tileZ == (int)map.waypoints[14].z)
				{
					currentRoam_Value = 16;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[16].x && EnemyManager.Instance.tileZ == (int)map.waypoints[16].z)
				{
					currentRoam_Value = 15;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[15].x && EnemyManager.Instance.tileZ == (int)map.waypoints[15].z)
				{
					currentRoam_Value = 17;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[17].x && EnemyManager.Instance.tileZ == (int)map.waypoints[17].z)
				{
					currentRoam_Value = 18;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[18].x && EnemyManager.Instance.tileZ == (int)map.waypoints[18].z)
				{
					currentRoam_Value = 20;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[20].x && EnemyManager.Instance.tileZ == (int)map.waypoints[20].z)
				{
					currentRoam_Value = 21;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[21].x && EnemyManager.Instance.tileZ == (int)map.waypoints[21].z)
				{
					currentRoam_Value = 19;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[19].x && EnemyManager.Instance.tileZ == (int)map.waypoints[19].z)
				{
					if(RandChance == 1)
					{
						currentRoam_Value = 16;
					}
					else
					{
						currentRoam_Value = 9;
					}
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[9].x && EnemyManager.Instance.tileZ == (int)map.waypoints[9].z)
				{
					currentRoam_Value = 11;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[11].x && EnemyManager.Instance.tileZ == (int)map.waypoints[11].z)
				{
					currentRoam_Value = 7;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[7].x && EnemyManager.Instance.tileZ == (int)map.waypoints[7].z)
				{
					currentRoam_Value = 12;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[12].x && EnemyManager.Instance.tileZ == (int)map.waypoints[12].z)
				{
					currentRoam_Value = 13;
				}
				else
				{
					currentRoam_Value = 13;
				}

				reachedRoam_ = false;
			}

			map.GetComponent<TileMap>().EnemyGeneratePathTo((int)map.waypoints[currentRoam_Value].x, (int)map.waypoints[currentRoam_Value].z);
		}
		else if(state == EnemyBehavior.ROAM_LIVING)
		{
			if(reachedRoam_)
			{
				if(EnemyManager.Instance.tileX == (int)map.waypoints[25].x && EnemyManager.Instance.tileZ == (int)map.waypoints[25].z)
				{
					currentRoam_Value = 30;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[30].x && EnemyManager.Instance.tileZ == (int)map.waypoints[30].z)
				{
					currentRoam_Value = 42;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[42].x && EnemyManager.Instance.tileZ == (int)map.waypoints[42].z)
				{
					currentRoam_Value = 41;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[41].x && EnemyManager.Instance.tileZ == (int)map.waypoints[41].z)
				{
					currentRoam_Value = 43;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[43].x && EnemyManager.Instance.tileZ == (int)map.waypoints[43].z)
				{
					currentRoam_Value = 28;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[28].x && EnemyManager.Instance.tileZ == (int)map.waypoints[28].z)
				{
					currentRoam_Value = 24;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[24].x && EnemyManager.Instance.tileZ == (int)map.waypoints[24].z)
				{
					currentRoam_Value = 23;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[23].x && EnemyManager.Instance.tileZ == (int)map.waypoints[23].z)
				{
					currentRoam_Value = 29;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[29].x && EnemyManager.Instance.tileZ == (int)map.waypoints[29].z)
				{
					if(RandChance == 1)
					{
						currentRoam_Value = 25;
					}
					else
					{
						currentRoam_Value = 27;
					}
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[27].x && EnemyManager.Instance.tileZ == (int)map.waypoints[27].z)
				{
					currentRoam_Value = 31;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[31].x && EnemyManager.Instance.tileZ == (int)map.waypoints[31].z)
				{
					currentRoam_Value = 26;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[26].x && EnemyManager.Instance.tileZ == (int)map.waypoints[26].z)
				{
					currentRoam_Value = 25;
				}
				else
				{
					currentRoam_Value = 25;
				}

				reachedRoam_ = false;
			}

			map.GetComponent<TileMap>().EnemyGeneratePathTo((int)map.waypoints[currentRoam_Value].x, (int)map.waypoints[currentRoam_Value].z);
		}
		else if(state == EnemyBehavior.ROAM_LIBRARY)
		{
			if(reachedRoam_)
			{
				if(EnemyManager.Instance.tileX == (int)map.waypoints[34].x && EnemyManager.Instance.tileZ == (int)map.waypoints[34].z)
				{
					currentRoam_Value = 33;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[33].x && EnemyManager.Instance.tileZ == (int)map.waypoints[33].z)
				{
					currentRoam_Value = 35;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[35].x && EnemyManager.Instance.tileZ == (int)map.waypoints[35].z)
				{
					currentRoam_Value = 38;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[38].x && EnemyManager.Instance.tileZ == (int)map.waypoints[38].z)
				{
					currentRoam_Value = 36;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[36].x && EnemyManager.Instance.tileZ == (int)map.waypoints[36].z)
				{
					currentRoam_Value = 39;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[39].x && EnemyManager.Instance.tileZ == (int)map.waypoints[39].z)
				{
					currentRoam_Value = 37;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[37].x && EnemyManager.Instance.tileZ == (int)map.waypoints[37].z)
				{
					currentRoam_Value = 17;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[17].x && EnemyManager.Instance.tileZ == (int)map.waypoints[17].z)
				{
					currentRoam_Value = 15;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[15].x && EnemyManager.Instance.tileZ == (int)map.waypoints[15].z)
				{
					if(RandChance == 1)
					{
						currentRoam_Value = 32;
					}
					else
					{
						currentRoam_Value = 20;
					}
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[20].x && EnemyManager.Instance.tileZ == (int)map.waypoints[20].z)
				{
					currentRoam_Value = 29;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[29].x && EnemyManager.Instance.tileZ == (int)map.waypoints[29].z)
				{
					currentRoam_Value = 32;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[32].x && EnemyManager.Instance.tileZ == (int)map.waypoints[32].z)
				{
					currentRoam_Value = 34;
				}
				else
				{
					currentRoam_Value = 34;
				}

				reachedRoam_ = false;
			}

			map.GetComponent<TileMap>().EnemyGeneratePathTo((int)map.waypoints[currentRoam_Value].x, (int)map.waypoints[currentRoam_Value].z);
		}
		else if(state == EnemyBehavior.ROAM_BEDROOM)
		{
			if(reachedRoam_)
			{
				if(EnemyManager.Instance.tileX == (int)map.waypoints[42].x && EnemyManager.Instance.tileZ == (int)map.waypoints[42].z)
				{
					currentRoam_Value = 41;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[41].x && EnemyManager.Instance.tileZ == (int)map.waypoints[41].z)
				{
					currentRoam_Value = 43;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[43].x && EnemyManager.Instance.tileZ == (int)map.waypoints[43].z)
				{
					currentRoam_Value = 30;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[30].x && EnemyManager.Instance.tileZ == (int)map.waypoints[30].z)
				{
					currentRoam_Value = 31;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[31].x && EnemyManager.Instance.tileZ == (int)map.waypoints[31].z)
				{
					currentRoam_Value = 27;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[27].x && EnemyManager.Instance.tileZ == (int)map.waypoints[27].z)
				{
					currentRoam_Value = 28;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[28].x && EnemyManager.Instance.tileZ == (int)map.waypoints[28].z)
				{
					currentRoam_Value = 33;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[33].x && EnemyManager.Instance.tileZ == (int)map.waypoints[33].z)
				{
					if(RandChance == 1)
					{
						currentRoam_Value = 42;
					}
					else
					{
						currentRoam_Value = 35;
					}
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[35].x && EnemyManager.Instance.tileZ == (int)map.waypoints[35].z)
				{
					currentRoam_Value = 38;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[38].x && EnemyManager.Instance.tileZ == (int)map.waypoints[38].z)
				{
					currentRoam_Value = 40;
				}
				else if(EnemyManager.Instance.tileX == (int)map.waypoints[40].x && EnemyManager.Instance.tileZ == (int)map.waypoints[40].z)
				{
					currentRoam_Value = 42;
				}
				else
				{
					currentRoam_Value = 42;
				}

				reachedRoam_ = false;
			}

			map.GetComponent<TileMap>().EnemyGeneratePathTo((int)map.waypoints[currentRoam_Value].x, (int)map.waypoints[currentRoam_Value].z);
		}
	}

	// The "Next Turn" button calls this.
	public void NextTurn()
	{
		SoundManagerScript.Instance.EnemyMove.Play();

		// Update distance calculation only when move
		distFromPlayer = Vector3.Distance(PlayerManager.Instance.transform.position, this.transform.position);

		// Make sure to wrap-up any outstanding movement left over.
		while(currentPath != null && remainingMovement > 0)
		{
			AdvancePathing();
		}

		EnemyBehaviourCheck();

		// Reset our available movement points.
		remainingMovement = moveSpeed;
	}

	void PlayerDetection()
	{
		if((PlayerManager.Instance.hasLight == true && distFromPlayer <= 8) || (PlayerManager.Instance.currentSanityLevel <= 1 && distFromPlayer <= 8))
		{
			playerDetectable = true;
		}
		else
		{
			playerDetectable = false;
		}
	}

	bool CheckLosingCondition()
	{
		if(tileX - 1 == PlayerManager.Instance.tileX && tileZ == PlayerManager.Instance.tileZ ||
			tileX + 1 == PlayerManager.Instance.tileX && tileZ == PlayerManager.Instance.tileZ)
		{
			return true;
		}
		else if(tileX == PlayerManager.Instance.tileX && tileZ + 1 == PlayerManager.Instance.tileZ || 
			tileX == PlayerManager.Instance.tileX && tileZ  - 1 == PlayerManager.Instance.tileZ)
		{
			return true;
		}
		else if(tileX == PlayerManager.Instance.tileX - 1 && tileZ == PlayerManager.Instance.tileZ - 1 ||
			tileX == PlayerManager.Instance.tileX - 1 && tileZ == PlayerManager.Instance.tileZ + 1)
		{
			return true;
		}
		else if(tileX == PlayerManager.Instance.tileX + 1 && tileZ == PlayerManager.Instance.tileZ - 1 ||
			tileX == PlayerManager.Instance.tileX + 1 && tileZ == PlayerManager.Instance.tileZ + 1)
		{
			return true;
		}

		return  false;
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			if(PlayerManager.Instance.isHidden == false)
			{
				PlayerManager.Instance.ReduceSanity();
				PlayerManager.Instance.enemyInRange = true;
				GUIManagerScript.Instance.UpdateSanityBar();
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			PlayerManager.Instance.enemyInRange = false;
		}
	}
}
