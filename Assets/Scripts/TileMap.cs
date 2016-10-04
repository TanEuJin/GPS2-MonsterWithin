using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TileMap : MonoBehaviour
{
	public GameObject player;
	public GameObject enemy;

	public TileType[] tileTypes;

	int[,] tiles;
	Node[,] graph;


	const int mapSizeX = 27;
	const int mapSizeY = 20;

	void Start()
	{
		// Setup the enemy's variable
		EnemyManager.Instance.tileX = (int)enemy.transform.position.x;
		EnemyManager.Instance.tileZ = (int)enemy.transform.position.z;
		EnemyManager.Instance.map = this;

		PlayerManager.Instance.tileX = (int)player.transform.position.x;
		PlayerManager.Instance.tileZ = (int)player.transform.position.z;
		PlayerManager.Instance.map = this;

		GenerateMapData();
		GeneratePathfindingGraph();
		GenerateMapVisual();
	}

	void GenerateMapData()
	{
		// Allocate our map tiles	
		tiles = new int[mapSizeX,mapSizeY]
		{
			{9, 9, 9, 9, 9,		9, 9, 9, 9, 9,		9, 9, 0, 0, 0,		0, 0, 0, 0, 0},
			{9, 9, 9, 9, 9,		9, 9, 9, 9, 9,		9, 9, 0, 2, 1,		5, 5, 1, 1, 0},
			{9, 9, 9, 9, 9,		9, 9, 9, 0, 0,		0, 0, 0, 2, 1,		1, 1, 1, 1, 0},
			{9, 9, 9, 9, 9,		9, 9, 9, 0, 9,		9, 9, 6, 1, 1,		2, 2, 1, 1, 0},
			{9, 9, 9, 9, 9,		9, 9, 9, 0, 9,		9, 9, 0, 1, 1,		2, 2, 1, 2, 0},

			{9, 9, 9, 9, 9,		9, 9, 9, 0, 9,		9, 9, 0, 1, 2,		2, 2, 1, 2, 0},
			{9, 9, 9, 9, 9,		9, 9, 0, 0, 6,		0, 0, 0, 4, 0,		0, 0, 0, 0, 0},
			{9, 9, 9, 9, 9,		9, 9, 0, 1, 1,		1, 1, 1, 1, 1,		1, 0, 9, 9, 9},
			{9, 9, 9, 9, 9,		9, 9, 0, 1, 1,		1, 2, 2, 1, 1,		1, 0, 9, 9, 9},
			{9, 9, 9, 9, 9,		9, 9, 0, 1, 1,		1, 1, 1, 1, 1,		1, 0, 9, 9, 9},

			{9, 9, 9, 9, 9,		9, 9, 0, 2, 1,		2, 1, 2, 2, 1,		2, 0, 9, 9, 9},
			{9, 9, 9, 9, 9,		9, 9, 0, 2, 1,		2, 1, 2, 2, 1,		2, 0, 9, 9, 9},
			{9, 9, 9, 9, 9,		9, 9, 0, 2, 1,		2, 1, 2, 2, 1,		2, 0, 9, 9, 9},
			{9, 9, 9, 9, 9,		9, 9, 0, 2, 1,		1, 1, 1, 1, 1,		1, 0, 9, 9, 9},
			{9, 9, 9, 9, 9,		9, 9, 0, 1, 1,		1, 2, 2, 1, 1,		1, 0, 9, 9, 9},

			{9, 9, 9, 9, 9,		9, 9, 0, 1, 1,		1, 1, 1, 1, 2,		2, 0, 9, 9, 9},
			{9, 9, 9, 0, 0,		0, 0, 0, 0, 3,		0, 0, 0, 3, 0,		0, 0, 9, 9, 9},
			{9, 9, 9, 8, 1,		7, 1, 1, 1, 1,		1, 2, 2, 1, 0,		9, 9, 9, 9, 9},
			{9, 9, 9, 0, 1,		1, 1, 2, 2, 1,		1, 1, 1, 1, 0,		9, 9, 9, 9, 9},
			{0, 0, 0, 0, 0,		4, 0, 0, 0, 0,		0, 0, 0, 4, 0,		9, 9, 9, 9, 9},

			{0, 1, 1, 1, 1,		1, 1, 0, 2, 1,		1, 1, 1, 1, 0,		9, 9, 9, 9, 9},
			{0, 5, 1, 2, 1,		2, 1, 0, 2, 1,		2, 2, 2, 1, 0,		9, 9, 9, 9, 9},
			{0, 5, 1, 2, 2,		2, 1, 0, 1, 1,		2, 2, 2, 1, 0,		9, 9, 9, 9, 9},
			{0, 5, 1, 2, 2,		2, 1, 3, 1, 1,		1, 1, 1, 1, 0,		9, 9, 9, 9, 9},
			{0, 5, 1, 1, 2,		2, 1, 0, 2, 2,		2, 2, 1, 1, 0,		9, 9, 9, 9, 9},

			{0, 2, 2, 1, 1,		1, 1, 0, 0, 0,		0, 0, 0, 0, 0,		9, 9, 9, 9, 9},
			{0, 0, 0, 0, 0,		0, 0, 0, 9, 9,		9, 9, 9, 9, 9,		9, 9, 9, 9, 9}
		};
	}

	public float CostToEnterTile(int sourceX, int sourceZ, int targetX, int targetZ)
	{

		TileType tt = tileTypes[tiles[targetX, targetZ]];

		if(UnitCanEnterTile(targetX, targetZ) == false)
			return Mathf.Infinity;

		float cost = tt.movementCost;

		if(sourceX!=targetX && sourceZ!=targetZ)
		{
			// We are moving diagonally!  Fudge the cost for tie-breaking
			// Purely a cosmetic thing!
			cost += 0.001f;
		}

		return cost;

	}

	void GeneratePathfindingGraph()
	{
		// Initialize the array
		graph = new Node[mapSizeX, mapSizeY];

		// Initialize a Node for each spot in the array
		for(int x=0; x < mapSizeX; x++)
		{
			for(int y=0; y < mapSizeY; y++)
			{
				graph[x,y] = new Node();
				graph[x,y].x = x;
				graph[x,y].z = y;
			}
		}

		// Now that all the nodes exist, calculate their neighbours
		for(int x=0; x < mapSizeX; x++)
		{
			for(int y=0; y < mapSizeY; y++)
			{

				// This is the 4-way connection version:
				if(x > 0)
					graph[x,y].neighbours.Add( graph[x-1, y] );
				if(x < mapSizeX-1)
					graph[x,y].neighbours.Add( graph[x+1, y] );
				if(y > 0)
					graph[x,y].neighbours.Add( graph[x, y-1] );
				if(y < mapSizeY-1)
					graph[x,y].neighbours.Add( graph[x, y+1] );


				// This is the 8-way connection version (allows diagonal movement)
				// Try left
				/*				if(x > 0) {
					graph[x,y].neighbours.Add( graph[x-1, y] );
					if(y > 0)
						graph[x,y].neighbours.Add( graph[x-1, y-1] );
					if(y < mapSizeY-1)
						graph[x,y].neighbours.Add( graph[x-1, y+1] );
				}

				// Try Right
				if(x < mapSizeX-1) {
					graph[x,y].neighbours.Add( graph[x+1, y] );
					if(y > 0)
						graph[x,y].neighbours.Add( graph[x+1, y-1] );
					if(y < mapSizeY-1)
						graph[x,y].neighbours.Add( graph[x+1, y+1] );
				}

				// Try straight up and down
				if(y > 0)
					graph[x,y].neighbours.Add( graph[x, y-1] );
				if(y < mapSizeY-1)
					graph[x,y].neighbours.Add( graph[x, y+1] );
*/
				// This also works with 6-way hexes and n-way variable areas (like EU4)
			}
		}
	}

	void GenerateMapVisual()
	{
		for(int x=0; x < mapSizeX; x++)
		{
			for(int y=0; y < mapSizeY; y++)
			{

				TileType tt = tileTypes[ tiles[x,y] ];
				GameObject go = (GameObject)Instantiate( tt.tileVisualPrefab, new Vector3(x, 0, y), Quaternion.identity );//mapSizeX-x-1
				go.transform.parent = transform;

				ClickableTile ct = go.GetComponent<ClickableTile>();
				ct.tileX = x;
				ct.tileZ = y;
				ct.map = this;
			}
		}
	}

	public Vector3 TileCoordToWorldCoord(int x, int y)
	{
		return new Vector3(x, 0, y);
	}

	public bool UnitCanEnterTile(int x, int y)
	{

		// We could test the unit's walk/hover/fly type against various
		// terrain flags here to see if they are allowed to enter the tile.

		return tileTypes[ tiles[x,y] ].isWalkable;
	}

	public void EnemyGeneratePathTo(int x, int y)
	{
		// Clear out our unit's old path.
		EnemyManager.Instance.currentPath = null;

		if( UnitCanEnterTile(x,y) == false )
		{
			// We probably clicked on a mountain or something, so just quit out.
			return;
		}

		Dictionary<Node, float> dist = new Dictionary<Node, float>();
		Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

		// Setup the "Q" -- the list of nodes we haven't checked yet.
		List<Node> unvisited = new List<Node>();

		Node source = graph
			[
				EnemyManager.Instance.tileX, 
				EnemyManager.Instance.tileZ
			];

		Node target = graph
			[
				x, 
				y
			];

		dist[source] = 0;
		prev[source] = null;

		// Initialize everything to have INFINITY distance, since
		// we don't know any better right now. Also, it's possible
		// that some nodes CAN'T be reached from the source,
		// which would make INFINITY a reasonable value
		foreach(Node v in graph)
		{
			if(v != source)
			{
				dist[v] = Mathf.Infinity;
				prev[v] = null;
			}

			unvisited.Add(v);
		}

		while(unvisited.Count > 0)
		{
			// "u" is going to be the unvisited node with the smallest distance.
			Node u = null;

			foreach(Node possibleU in unvisited)
			{
				if(u == null || dist[possibleU] < dist[u])
				{
					u = possibleU;
				}
			}

			if(u == target)
			{
				break;	// Exit the while loop!
			}

			unvisited.Remove(u);

			foreach(Node v in u.neighbours)
			{
				//float alt = dist[u] + u.DistanceTo(v);
				float alt = dist[u] + CostToEnterTile(u.x, u.z, v.x, v.z);
				if( alt < dist[v] )
				{
					dist[v] = alt;
					prev[v] = u;
				}
			}
		}

		// If we get there, the either we found the shortest route
		// to our target, or there is no route at ALL to our target.

		if(prev[target] == null)
		{
			// No route between our target and the source
			return;
		}

		List<Node> currentPath = new List<Node>();

		Node curr = target;

		// Step through the "prev" chain and add it to our path
		while(curr != null)
		{
			currentPath.Add(curr);
			curr = prev[curr];
		}

		// Right now, currentPath describes a route from out target to our source
		// So we need to invert it!

		currentPath.Reverse();

		EnemyManager.Instance.currentPath = currentPath;
	}

	public void PlayerGeneratePathTo(int x, int y)
	{
		// Clear out our unit's old path.
		PlayerManager.Instance.currentPath = null;

		if(UnitCanEnterTile(x,y) == false)
		{
			// We probably clicked on a mountain or something, so just quit out.
			return;
		}

		Dictionary<Node, float> dist = new Dictionary<Node, float>();
		Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

		// Setup the "Q" -- the list of nodes we haven't checked yet.
		List<Node> unvisited = new List<Node>();

		Node source = graph
			[
				PlayerManager.Instance.tileX, 
				PlayerManager.Instance.tileZ
			];

		Node target = graph
			[
				x, 
				y
			];

		dist[source] = 0;
		prev[source] = null;

		// Initialize everything to have INFINITY distance, since
		// we don't know any better right now. Also, it's possible
		// that some nodes CAN'T be reached from the source,
		// which would make INFINITY a reasonable value
		foreach(Node v in graph)
		{
			if(v != source)
			{
				dist[v] = Mathf.Infinity;
				prev[v] = null;
			}

			unvisited.Add(v);
		}

		while(unvisited.Count > 0)
		{
			// "u" is going to be the unvisited node with the smallest distance.
			Node u = null;

			foreach(Node possibleU in unvisited)
			{
				if(u == null || dist[possibleU] < dist[u])
				{
					u = possibleU;
				}
			}

			if(u == target)
			{
				break;	// Exit the while loop!
			}

			unvisited.Remove(u);

			foreach(Node v in u.neighbours)
			{
				//float alt = dist[u] + u.DistanceTo(v);
				float alt = dist[u] + CostToEnterTile(u.x, u.z, v.x, v.z);
				if( alt < dist[v] )
				{
					dist[v] = alt;
					prev[v] = u;
				}
			}
		}

		// If we get there, the either we found the shortest route
		// to our target, or there is no route at ALL to our target.

		if(prev[target] == null)
		{
			// No route between our target and the source
			return;
		}

		List<Node> currentPath = new List<Node>();

		Node curr = target;

		// Step through the "prev" chain and add it to our path
		while(curr != null)
		{
			currentPath.Add(curr);
			curr = prev[curr];
		}

		// Right now, currentPath describes a route from out target to our source
		// So we need to invert it!

		currentPath.Reverse();

		PlayerManager.Instance.currentPath = currentPath;
	}
}