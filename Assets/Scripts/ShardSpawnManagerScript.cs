using UnityEngine;
using System.Collections;

public class ShardSpawnManagerScript : MonoBehaviour
{
	public Vector3[] shardSpawnpoints;
	public Vector3 lastShardSpawnpoint;
	public GameObject shard;

	void Awake()
	{
		shardSpawnpoints = new Vector3[GetComponentsInChildren<ShardSpawnpointScript> ().Length];
		for (int i = 0; i < shardSpawnpoints.Length; i++)
		{
			shardSpawnpoints [i] = Vector3.zero;
		}
	}

	// Use this for initialization
	void Start ()
	{
		shard = ShardScript.Instance.gameObject;
		ObjectPoolManagerScript.Instance.CreatePool(this.shard, 1 , 1);

		shard.transform.position = new Vector3(shardSpawnpoints[0].x, 1, shardSpawnpoints[0].z);
		shard.SetActive(true);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
