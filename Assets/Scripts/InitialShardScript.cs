﻿using UnityEngine;
using System.Collections;

public class InitialShardScript : MonoBehaviour
{

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
			Destroy(this.gameObject);
		}
	}
}
