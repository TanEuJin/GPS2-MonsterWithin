using UnityEngine;
using UnityEngine.SceneManagement;
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
			if(SceneManager.GetActiveScene().name == "_SCENE_")
			{
				GUIManagerScript.Instance.memoryShard.gameObject.SetActive(true);
			}
			Destroy(other.gameObject);
		}
		else if (other.CompareTag("Shard"))
		{
			if(SceneManager.GetActiveScene().name == "_SCENE_")
			{
				GUIManagerScript.Instance.memoryShard.gameObject.SetActive(true);
			}
			else if(SceneManager.GetActiveScene().name == "TUTORIAL_SCENE_")
			{
				// Transition to _SCENE_ straightaway
				SceneManager.LoadScene ("_SCENE_");
			}
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
