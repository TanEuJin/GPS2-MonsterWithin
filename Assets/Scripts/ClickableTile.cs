using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ClickableTile : MonoBehaviour 
{
	public int tileX;
	public int tileZ;
	public TileMap map;
	public int playerPosX;
	public int playerPosZ;
	public int waypointCounter;

	void Start()
	{
		
	}

	void Update()
	{
		
		if(waypointCounter == 25)
		{
			waypointCounter = 0;
		}
		playerPosX = (int)PlayerManager.Instance.transform.position.x;
		playerPosZ = (int)PlayerManager.Instance.transform.position.z;

		if(EnemyManager.Instance.state == EnemyBehavior.PATROLLING)
		{

			if(EnemyManager.Instance.tileX == (int)map.waypoints[waypointCounter].x && EnemyManager.Instance.tileZ ==(int)map.waypoints[waypointCounter].z)
			{
				waypointCounter++;
				this.map.EnemyGeneratePathTo((int)map.waypoints[waypointCounter].x, (int)map.waypoints[waypointCounter].z);
			}
			/*if(tileX == 0 && tileZ == 0)
			{
				this.map.EnemyGeneratePathTo(0, 0);
			}*/
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
	}

	void OnMouseUp()
	{
		if(EventSystem.current.IsPointerOverGameObject())
			return;

		map.PlayerGeneratePathTo(tileX, tileZ);
	}
}
