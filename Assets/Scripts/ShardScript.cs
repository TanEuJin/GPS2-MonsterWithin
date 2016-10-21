using UnityEngine;
using System.Collections.Generic;

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

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
