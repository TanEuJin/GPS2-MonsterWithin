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
	//public GameObject player;

	void Start()
	{
		
	}

	void Update()
	{
		playerPosX = (int)PlayerManager.Instance.transform.position.x;
		playerPosZ = (int)PlayerManager.Instance.transform.position.z;

		if(EnemyManager.Instance.state == EnemyBehavior.PATROLLING)
		{
			
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
