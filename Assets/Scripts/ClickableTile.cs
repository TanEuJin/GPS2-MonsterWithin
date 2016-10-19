using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickableTile : MonoBehaviour 
{
	public int tileX;
	public int tileZ;
	public TileMap map;
	public int playerPosX;
	public int playerPosZ;
	//public int waypointCounter;

	private int LayerTile;
	void Start()
	{
		LayerTile = LayerMask.NameToLayer ("ClickableTile");
	}

	void Update()
	{
		playerPosX = (int)PlayerManager.Instance.transform.position.x;
		playerPosZ = (int)PlayerManager.Instance.transform.position.z;

		if (EnemyManager.Instance.state == EnemyBehavior.PATROLLING) {
			if (tileX == EnemyManager.Instance.tileX && tileZ == EnemyManager.Instance.tileZ) {
				this.map.EnemyGeneratePathTo ((int)map.waypoints [map.waypointCounter].x, (int)map.waypoints [map.waypointCounter].z);
			}
		}

		/* Reset Patrolling waypoint
		if(waypointCounter == 25)
		{
			waypointCounter = 0;
		}

		//! Patrolling Behavior
		if(EnemyManager.Instance.state == EnemyBehavior.PATROLLING)
		{

			if(EnemyManager.Instance.tileX == (int)map.waypoints[waypointCounter].x && EnemyManager.Instance.tileZ ==(int)map.waypoints[waypointCounter].z)
			{
				waypointCounter++;
				this.map.EnemyGeneratePathTo((int)map.waypoints[waypointCounter].x, (int)map.waypoints[waypointCounter].z);
			}
			else
			{
				if(tileX == EnemyManager.Instance.tileX && tileZ == EnemyManager.Instance.tileZ)
				{
					this.map.EnemyGeneratePathTo((int)map.waypoints[waypointCounter].x, (int)map.waypoints[waypointCounter].z);
				}
			}
		}
		else if(EnemyManager.Instance.state == EnemyBehavior.DISTRACTED)
		{
			
		}
		else if(EnemyManager.Instance.state == EnemyBehavior.CHASING)
		{
			if(this.gameObject.transform.position == PlayerManager.Instance.transform.position)
			{
				this.map.EnemyGeneratePathTo(playerPosX, playerPosZ);
			}
		}
		*/
	}

	void OnMouseUp()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hitInfo;
		Collider playerCol = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider>();
		if (PlayerManager.Instance.isHidden == false) 
		{
			Physics.IgnoreCollision(playerCol, playerCol, true);
			if (Physics.Raycast (ray, out hitInfo)) 
			{
				if(EventSystem.current.IsPointerOverGameObject())
				{
					Debug.Log("Test");
					return;
				}
				if (hitInfo.transform.gameObject.layer == LayerTile) 
				{
					//Newly added.
					GameObject ourHitObject = hitInfo.collider.transform.gameObject;
					Debug.Log(ourHitObject.name);
					Debug.Log (LayerTile);
					map.PlayerGeneratePathTo ((int)ourHitObject.transform.position.x, (int)ourHitObject.transform.position.z);
					return;
				}
			}
		} 
	} //! End of OnMouseDown
}



