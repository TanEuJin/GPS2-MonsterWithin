using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

//! Test
//! pls work

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
	public int moveSpeed = 2;
	public float remainingMovement=2;

	//! Object Interaction
	public GameObject InteractButton, playerModel;
	public GameObject Door, Closet;
	public bool closetInteract, doorInteract, HideInteract, isHidden;
	public int keys;

	// Sanity
	public bool hasLight = false;
	public int currentSanityLevel = 4;
	public int maxSanityLevel = 6;
	public bool enemyInRange = false;

	Animator anim;
	Transform modalTransform;

	void Start()
	{
		GUIManagerScript.Instance.UpdateSanityBar();
		GUIManagerScript.Instance.movesCount.text = "Remaining Movements: " + remainingMovement;
		//SoundManagerScript.Instance.PlayLoopingSFX(AudioClipID.SFX_HEARTBEAT120);

		anim = GetComponentInChildren<Animator>();
		modalTransform = transform.GetChild(0).gameObject.transform;
	}

	void Update()
	{
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
	void AdvancePathing()
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
			//modalTransform.Rotate(new Vector3(0.0f, 90.0f, 0.0f), Space.World);
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

		if(currentPath.Count == 1)
		{
			// We only have one tile left in the path, and that tile MUST be our ultimate
			// destination -- and we are standing on it!
			// So let's just clear our pathfinding info.
			currentPath = null;
			//anim.SetBool("IsWalk", false);
		}

		GUIManagerScript.Instance.movesCount.text = "Remaining Movements: " + remainingMovement;
	}

	// The "Next Turn" button calls this.
	public void NextTurn()
	{
		// Make sure to wrap-up any outstanding movement left over.
		while(currentPath!=null && remainingMovement > 0)
		{
			AdvancePathing();
		}

		// Reset our available movement points.
		if(currentSanityLevel <= 2)
		{
			remainingMovement = 2;
		}
		else if(currentSanityLevel >=3 && currentSanityLevel <= 4)
		{
			remainingMovement = 3;
		}
		else if(currentSanityLevel >=5 && currentSanityLevel <= 6)
		{
			remainingMovement = 4;
		}

		if(enemyInRange)
		{
			currentSanityLevel--;
		}

		GUIManagerScript.Instance.movesCount.text = "Remaining Movements: " + remainingMovement;
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Light"))
		{
			hasLight = true;
		}

		if(other.CompareTag("Shard"))
		{
			currentSanityLevel ++;
			GUIManagerScript.Instance.UpdateSanityBar();
			ShardScript.Instance.transform.gameObject.SetActive(false);
		}

		if (other.gameObject.CompareTag ("HideObject")) 
		{
			HideInteract = true;
			InteractButton.SetActive(true);
		}

		if (other.CompareTag ("Enemy")) 
		{
			SoundManagerScript.Instance.seenByTheEnemy.Play();
			SoundManagerScript.Instance.seenByEnemy.TransitionTo (SoundManagerScript.Instance.m_TransitionIn);
			SoundManagerScript.Instance.playTransition ();
		}

	}

	void OnTriggerExit(Collider other)
	{
		if(other.CompareTag("Light"))
		{
			hasLight = false;
		}

		if (other.gameObject.CompareTag ("HideObject")) 
		{
			HideInteract = false;
			InteractButton.SetActive(false);
		}

		if (other.CompareTag ("Enemy")) 
		{
			SoundManagerScript.Instance.notSeenByEnemy.TransitionTo (SoundManagerScript.Instance.m_TransitionOut);
		}
	}
}