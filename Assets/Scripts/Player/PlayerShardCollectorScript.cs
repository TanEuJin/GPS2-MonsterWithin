using UnityEngine;
using System.Collections;

public class PlayerShardCollectorScript : MonoBehaviour
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
		if(other.CompareTag("Light"))
		{
			PlayerManager.Instance.hasLight = true;
		}

	}

	void OnTriggerExit(Collider other)
	{
		if(other.CompareTag("Light"))
		{
			PlayerManager.Instance.hasLight = false;
		}
	}
}
