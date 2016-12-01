using UnityEngine;
using System.Collections;

public class DisableScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DisableSelf()
	{
		this.gameObject.SetActive(false);
	}
}
