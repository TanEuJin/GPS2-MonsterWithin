using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickableTile : MonoBehaviour 
{
	public int tileX;
	public int tileZ;
	public TileMap map;
	//public int waypointCounter;

	private int LayerTile;
	void Start()
	{
		//! LayerTile = LayerMask.NameToLayer ("ClickableTile");
	}

	void Update()
	{
		
	}
	/*
	void OnMouseUp()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hitInfo;
		//Collider playerCol = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider>();

		if (PlayerManager.Instance.isHidden == false) 
		{
			//Physics.IgnoreCollision(playerCol, playerCol, true);

			if (Physics.Raycast (ray, out hitInfo)) 
			{
				if(EventSystem.current.IsPointerOverGameObject())
				{
					return;
				}
				if (hitInfo.transform.gameObject.layer == LayerTile) 
				{
					//Newly added.
					SoundManagerScript.Instance.PlayerMove.Play();
					GameObject ourHitObject = hitInfo.collider.transform.gameObject;
					map.PlayerGeneratePathTo ((int)ourHitObject.transform.position.x, (int)ourHitObject.transform.position.z);
					return;
				}
			}
		} 
	} //! End of OnMouseDown
	*/
}



