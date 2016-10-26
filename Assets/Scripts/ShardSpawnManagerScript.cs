using UnityEngine;
using System.Collections;

public class ShardSpawnManagerScript : MonoBehaviour
{
	public Vector3[] shardSpawnpoints;
	public Vector3 lastShardSpawnpoint;
	public GameObject shard;

	int Rand_1;
	int Rand_2;
	int Rand_3;

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
		ObjectPoolManagerScript.Instance.CreatePool(shard, 1 , 1);
		shard.SetActive(false);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(shard.activeSelf == false)
		{
			RandomizePoint();
			CheckSpawn();
		}
	}

	void RandomizePoint()
	{
		Rand_1 = Random.Range(4, 5);
		Rand_2 = Random.Range(0, 1);
		Rand_3 = Random.Range(2, 3);
	}

	void CheckSpawn()
	{
		if(lastShardSpawnpoint == new Vector3(shardSpawnpoints[0].x, 1, shardSpawnpoints[0].z) || lastShardSpawnpoint == new Vector3(shardSpawnpoints[1].x, 1, shardSpawnpoints[1].z))
		{
			shard.transform.position = new Vector3(shardSpawnpoints[Rand_1].x, 1, shardSpawnpoints[Rand_1].z);
			SpawnShard();
		}
		else if(lastShardSpawnpoint == new Vector3(shardSpawnpoints[2].x, 1, shardSpawnpoints[2].z) || lastShardSpawnpoint == new Vector3(shardSpawnpoints[3].x, 1, shardSpawnpoints[3].z))
		{
			shard.transform.position = new Vector3(shardSpawnpoints[Rand_2].x, 1, shardSpawnpoints[Rand_2].z);
			SpawnShard();
		}
		else if(lastShardSpawnpoint == new Vector3(shardSpawnpoints[4].x, 1, shardSpawnpoints[4].z) || lastShardSpawnpoint == new Vector3(shardSpawnpoints[5].x, 1, shardSpawnpoints[5].z))
		{
			shard.transform.position = new Vector3(shardSpawnpoints[Rand_3].x, 1, shardSpawnpoints[Rand_3].z);
			SpawnShard();
		}
		else
		{
			shard.transform.position = new Vector3(shardSpawnpoints[0].x, 1, shardSpawnpoints[0].z);
			SpawnShard();
		}
	}

	void SpawnShard()
	{
		lastShardSpawnpoint = shard.transform.position;
		shard.SetActive(true);
	}
}
