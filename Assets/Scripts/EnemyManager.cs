﻿using UnityEngine;
using System.Collections.Generic;

public enum EnemyBehavior
{
	PATROLLING 	= 0,
	CHASING		= 1,

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
	public EnemyBehavior state;

	Animator anim;

	void Start()
	{
		anim = GetComponentInChildren<Animator>();
	}

	void Update()
	{
		//! Setting Enemy Behavior
		if(PlayerManager.Instance.hasLight == true)//gotLight as in torch is on
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
		}
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
		remainingMovement = moveSpeed;
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			PlayerManager.Instance.currentSanityLevel --;
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
