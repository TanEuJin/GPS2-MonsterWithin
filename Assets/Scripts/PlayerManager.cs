using UnityEngine;
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
	public int moveSpeed = 2;
	public float remainingMovement=2;

	//! Object Interaction
	public GameObject InteractButton;
	public GameObject Door, Closet;
	public bool closetInteract, doorInteract, isHidden;
	public int keys;

	// Sanity
	public GameObject flashLight;
	public bool gotLight = false;
	public int currentSanityLevel = 4;
	public int maxSanityLevel = 6;


	void Update()
	{
		// Draw our debug line showing the pathfinding!
		// NOTE: This won't appear in the actual game view.
		if(currentPath != null)
		{
			int currNode = 0;

			while( currNode < currentPath.Count-1 )
			{
				Vector3 start = map.TileCoordToWorldCoord( currentPath[currNode].x, currentPath[currNode].z ) + 
					new Vector3(0, 0, -0.5f) ;
				Vector3 end   = map.TileCoordToWorldCoord( currentPath[currNode+1].x, currentPath[currNode+1].z )  + 
					new Vector3(0, 0, -0.5f) ;

				Debug.DrawLine(start, end, Color.red);

				currNode++;
			}
		}

		// Have we moved our visible piece close enough to the target tile that we can
		// advance to the next step in our pathfinding?
		if(Vector3.Distance(transform.position, map.TileCoordToWorldCoord( tileX, tileZ )) < 0.1f)
			AdvancePathing();

		// Smoothly animate towards the correct map tile.
		transform.position = Vector3.Lerp(transform.position, map.TileCoordToWorldCoord( tileX, tileZ ), 5f * Time.deltaTime);
	}

	// Advances our pathfinding progress by one tile.
	void AdvancePathing()
	{
		if(currentPath==null)
			return;

		if(remainingMovement <= 0)
			return;

		// Teleport us to our correct "current" position, in case we
		// haven't finished the animation yet.
		transform.position = map.TileCoordToWorldCoord( tileX, tileZ );

		// Get cost from current tile to next tile
		remainingMovement -= map.CostToEnterTile(currentPath[0].x, currentPath[0].z, currentPath[1].x, currentPath[1].z );

		// Move us to the next tile in the sequence
		tileX = currentPath[1].x;
		tileZ = currentPath[1].z;

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
		/*if(other.CompareTag("Light"))
		{
			gotLight = true;
		}*/

		if (other.gameObject.CompareTag ("Key")) {
			Destroy (other.gameObject);
			keys++;
		}

		if (other.gameObject.CompareTag ("Closet")) {
			closetInteract = true;
			InteractButton.SetActive(true);
		}

		if (other.gameObject.CompareTag ("Door")) {
			doorInteract = true;
			InteractButton.SetActive(true);
		}

	}

	void OnTriggerExit(Collider other)
	{
		/*if(other.CompareTag("Light"))
		{
			if(flashLight.activeSelf == false)
			{
				gotLight = false;
			}
		}*/

		if (other.gameObject.CompareTag ("Closet")) {
			closetInteract = false;
			InteractButton.SetActive(false);
		}

		if (other.gameObject.CompareTag ("Door")) {
			doorInteract = false;
			InteractButton.SetActive(false);
		}
	}

	public void flashLightToggle(bool isOn)
	{
		flashLight.SetActive(isOn);
		gotLight = isOn;
	}
}
