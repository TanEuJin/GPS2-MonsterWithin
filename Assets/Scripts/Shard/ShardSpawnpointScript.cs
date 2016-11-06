using UnityEngine;
using System.Collections;

public class ShardSpawnpointScript : MonoBehaviour
{
	public int index = -1;

	// Use this for initialization
	void Start ()
	{
		if (transform.parent.GetComponent<ShardSpawnManagerScript> ().shardSpawnpoints [index] == Vector3.zero)
		{
			transform.parent.GetComponent<ShardSpawnManagerScript> ().shardSpawnpoints [index] = ConvertPosToGridPos (transform.position);
		}
		else
		{
			Debug.LogError ("Value taken, conflict number" + ConvertPosToGridPos (transform.position) + " " + this.name);
		}
	}

	// Update is called once per frame
	void Update ()
	{

	}

	Vector3 ConvertPosToGridPos(Vector3 pos)
	{
		return pos;
	}
}
