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
		else if(other.CompareTag("InitialShard"))
		{
			GUIManagerScript.Instance.initalMemoryActivation();
			GUIManagerScript.Instance.memoryShard.gameObject.SetActive(true);
			Destroy(other.gameObject);
		}
		else if (other.CompareTag("Shard"))
		{
			GUIManagerScript.Instance.memoryShard.gameObject.SetActive(true);

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
