using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ClickableTile : MonoBehaviour {

	public int tileX;
	public int tileZ;
	public TileMap map;
	public int playerPosX;
	public int playerPosZ;
	public GameObject player;

	void Start()
	{
		player = GameObject.FindWithTag("Player");
	}

	void Update()
	{
		playerPosX = (int)player.transform.position.x;//(int)GameObject.FindWithTag("Player").transform.position.x;
		playerPosZ = (int)player.transform.position.z;//(int)GameObject.FindWithTag("Player").transform.position.z;

		if(this.gameObject.transform.position == player.transform.position)
		{
			this.map.EnemyGeneratePathTo(playerPosX, playerPosZ);
		}
	}

	void OnMouseUp()
	{
		Debug.Log ("Click!");

		if(EventSystem.current.IsPointerOverGameObject())
			return;

		map.PlayerGeneratePathTo(tileX, tileZ);
	}
}
