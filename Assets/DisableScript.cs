using UnityEngine;
using System.Collections;

public class DisableScript : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void DisableSelf()
	{
		if(GUIManagerScript.Instance.shardCollected == 3)
		{
			GUIManagerScript.Instance.gameUI.gameObject.SetActive(true);
			GUIManagerScript.Instance.gameEnd = true;
		}

		this.gameObject.SetActive(false);
	}
}
