using UnityEngine;
using System.Collections;

public class EnemyPlayerDissapear : MonoBehaviour {
	public bool observed = false;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if(observed)
		{
			GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
		}
		else
		{
			GetComponentInChildren<SkinnedMeshRenderer>().enabled  = false;
		}

		observed = false;
	}

	void Observed()
	{
		observed = true;
	}
}
