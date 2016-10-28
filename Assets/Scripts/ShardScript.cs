using UnityEngine;
using System.Collections.Generic;

public enum ShardLocation
{
	DINING	 	= 0,
	KITCHEN 	= 1,
	MORNING 	= 2,
	LIVING	 	= 3,
	LIBRARY 	= 4,
	BEDROOM 	= 5,

	TOTAL = 10
}

public class ShardScript : MonoBehaviour
{
	private static ShardScript mInstance;

	public static ShardScript Instance
	{
		get
		{
			if(mInstance == null)
			{
				GameObject tempObject = GameObject.FindGameObjectWithTag("Shard");

				if(tempObject == null)
				{
					GameObject obj = new GameObject("_ShardScript");
					mInstance = obj.AddComponent<ShardScript>();
					obj.tag = "ShardScript";
				}
				else
				{
					mInstance = tempObject.GetComponent<ShardScript>();
				}
			}
			return mInstance;
		}
	}

	public ShardLocation location;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("ShardCollector"))
		{
			PlayerManager.Instance.IncreaseSanity();
			GUIManagerScript.Instance.UpdateSanityBar();
			this.gameObject.SetActive(false);
		}
	}
}
