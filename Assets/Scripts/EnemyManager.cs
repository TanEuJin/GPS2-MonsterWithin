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
	int moveSpeed = 2;
	public float remainingMovement=2;
	public float distFromPlayer;
	public EnemyBehavior state;

	bool playerDetectable = false;
	Animator anim;
	Transform modalTransform;

	void Start()
	{
		anim = GetComponentInChildren<Animator>();
		modalTransform = transform.GetChild(0).gameObject.transform;
	}

	void Update()
	{
		PlayerDetection();

		//! Setting Enemy Behavior
		if(playerDetectable)
		{
			state = EnemyBehavior.CHASING;
		}
		else
		{
			state = EnemyBehavior.PATROLLING;
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
	}

	// Advances our pathfinding progress by one tile.
	public void AdvancePathing()
	{		
		if(currentPath==null || remainingMovement <= 0)
		{
			anim.SetBool("IsWalk", false);
			return;
		}

		// Teleport us to our correct "current" position, in case we
		// haven't finished the animation yet.
		transform.position = map.TileCoordToWorldCoord( tileX, tileZ );

		// Get cost from current tile to next tile
		remainingMovement -= map.CostToEnterTile(currentPath[0].x, currentPath[0].z, currentPath[1].x, currentPath[1].z );
		
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
			//modalTransform.Rotate(new Vector3(0.0f, 90.0f, 0.0f), Space.World);
			modalTransform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
		}
		else if(tileZ - 1 == currentPath[1].z)
		{
			modalTransform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
		}

		tileX = currentPath[1].x;
		tileZ = currentPath[1].z;
		anim.SetBool("IsWalk", true);
		
		// Remove the old "current" tile from the pathfinding list
		currentPath.RemoveAt(0);
		CheckLosingCondition();
		
		if(currentPath.Count == 1)
		{
			// We only have one tile left in the path, and that tile MUST be our ultimate
			// destination -- and we are standing on it!
			// So let's just clear our pathfinding info.
			currentPath = null;
		}
	}

	// The "Next Turn" button calls this.
	public void NextTurn()
	{
		SoundManagerScript.Instance.EnemyMove.Play();
		// Make sure to wrap-up any outstanding movement left over.
		while(currentPath!=null && remainingMovement > 0)
		{
			AdvancePathing();
		}

		// Update distance calculation only when move
		distFromPlayer = Vector3.Distance(PlayerManager.Instance.transform.position, this.transform.position);

		// Behavior switch
		if(EnemyManager.Instance.state == EnemyBehavior.PATROLLING)
		{
			if(EnemyManager.Instance.tileX == (int)map.waypoints[map.waypointCounter].x && EnemyManager.Instance.tileZ == (int)map.waypoints[map.waypointCounter].z)
			{
				map.waypointCounter++;
				map.GetComponent<TileMap>().EnemyGeneratePathTo((int)map.waypoints[map.waypointCounter].x, (int)map.waypoints[map.waypointCounter].z);
			}
			else
			{
				map.GetComponent<TileMap>().EnemyGeneratePathTo((int)map.waypoints [map.waypointCounter].x, (int)map.waypoints [map.waypointCounter].z);
			}
		}
		else if(EnemyManager.Instance.state == EnemyBehavior.CHASING)
		{
			if(EnemyManager.Instance.transform.position != PlayerManager.Instance.transform.position)
			{
				map.GetComponent<TileMap>().EnemyGeneratePathTo((int)PlayerManager.Instance.transform.position.x, (int)PlayerManager.Instance.transform.position.z);
			}
		}

		// Reset our available movement points.
		remainingMovement = moveSpeed;
	}

	void PlayerDetection()
	{
		if(PlayerManager.Instance.hasLight == true && distFromPlayer <= 4)
		{
			playerDetectable = true;
		}
		else
		{
			playerDetectable = false;
		}
	}

	void CheckLosingCondition()
	{
		if(tileX - 1 == PlayerManager.Instance.tileX && tileZ == PlayerManager.Instance.tileZ)
		{
			PlayerLose();
		}
		else if(tileX + 1 == PlayerManager.Instance.tileX && tileZ == PlayerManager.Instance.tileZ)
		{
			PlayerLose();
		}
		else if(tileX == PlayerManager.Instance.tileX && tileZ + 1 == PlayerManager.Instance.tileZ)
		{
			PlayerLose();
		}
		else if(tileX == PlayerManager.Instance.tileX && tileZ - 1 == PlayerManager.Instance.tileZ)
		{
			PlayerLose();
		}
		else if(tileX - 1 == PlayerManager.Instance.tileX - 1 && tileZ - 1 == PlayerManager.Instance.tileZ - 1 ||
			tileX - 1 == PlayerManager.Instance.tileX - 1 && tileZ + 1 == PlayerManager.Instance.tileZ + 1)
		{
			PlayerLose();
		}
		else if(tileX + 1 == PlayerManager.Instance.tileX - 1 && tileZ - 1 == PlayerManager.Instance.tileZ - 1 ||
			tileX + 1 == PlayerManager.Instance.tileX - 1 && tileZ + 1 == PlayerManager.Instance.tileZ + 1)
		{
			PlayerLose();
		}
	}

	void PlayerLose()
	{
		PlayerManager.Instance.playerDie = true;
		currentPath = null;
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			PlayerManager.Instance.ReduceSanity();
			PlayerManager.Instance.enemyInRange = true;
			GUIManagerScript.Instance.UpdateSanityBar();
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
