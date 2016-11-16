﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HighlightTiles : MonoBehaviour {
	public GameObject[] playerTileArray;
	public List <GameObject> highlightList;
	int mapSize;
	float rMove;
	List <Node> curPath;

	//! Checking vars
	public float OriginX;
	public float OriginZ;
	public int cCounter;
	int lCounter;

	//! Materials
	public Material Highlighted;
	public List <Material> defMat;
	MeshRenderer defRenderer;

	// Use this for initialization
	void Start () {
		cCounter = 0;
		curPath = PlayerManager.Instance.currentPath;
		mapSize = GameObject.Find("Map").GetComponent<TileMap>().mapSize;


	}
	
	// Update is called once per frame
	void Update () {
		rMove = PlayerManager.Instance.remainingMovement + 1;

		OriginX = PlayerManager.Instance.tileX;
		OriginZ = PlayerManager.Instance.tileZ;
		if (playerTileArray.Length != mapSize)
		{
			playerTileArray = GameObject.Find("Map").GetComponent<TileMap>().tileArray;
		}

		if (curPath == null && rMove > 0)
		{
			Checking();
		}
		else if (curPath != null && rMove <= 0)
		{
			FlushList();
		}


	}


	void Checking()
	{
		lCounter = 0;
		if (cCounter == 0)
		{
			for (float i = 0; i<=rMove; i++)
			{
				for (int j = 0; j<=	mapSize - 1; j++)
				{
					if (OriginX - i == playerTileArray[j].gameObject.transform.position.x && OriginZ == playerTileArray[j].gameObject.transform.position.z)
					{
						
						//! gameObject.transform.position += Vector3.up * 0.01f;
						if (playerTileArray[j].gameObject.tag == "UnclickableTile")
						{
							cCounter++;
							return;
						}
						else
						{
							highlightList.Add(playerTileArray[j]);
							lCounter++;
							if (lCounter >= rMove)
							{
								cCounter++;
								return;
							}
						}
					}
				}
			}
			cCounter++;
		}
		else if (cCounter == 1)
		{
			for (float i = 0; i<=rMove; i++)
			{
				for (int j = 0; j<=	mapSize - 1; j++)
				{
					if (OriginX + i == playerTileArray[j].gameObject.transform.position.x && OriginZ == playerTileArray[j].gameObject.transform.position.z)
					{

						//! gameObject.transform.position += Vector3.up * 0.01f;
						if (playerTileArray[j].gameObject.tag == "UnclickableTile")
						{
							cCounter++;
							return;
						}
						else
						{
							highlightList.Add(playerTileArray[j]);
							lCounter++;
							if (lCounter >= rMove)
							{
								cCounter++;
								return;
							}

						}
					}
				}
			}
			cCounter++;
		}
		else if (cCounter == 2)
		{
			for (float i = 0; i<=rMove; i++)
			{
				for (int j = 0; j<=	mapSize - 1; j++)
				{
					if (OriginZ - i == playerTileArray[j].gameObject.transform.position.z && OriginX == playerTileArray[j].gameObject.transform.position.x)
					{

						//! gameObject.transform.position += Vector3.up * 0.01f;
						if (playerTileArray[j].gameObject.tag == "UnclickableTile")
						{
							cCounter++;
							return;
						}
						else
						{
							highlightList.Add(playerTileArray[j]);
							lCounter++;
							if (lCounter >= rMove)
							{
								cCounter++;
								return;
							}
						}
					}
				}
			}
			cCounter++;
		}
		else if (cCounter == 3)
		{
			for (float i = 0; i<=rMove; i++)
			{
				for (int j = 0; j<=	mapSize - 1; j++)
				{
					if (OriginZ + i == playerTileArray[j].gameObject.transform.position.z && OriginX == playerTileArray[j].gameObject.transform.position.x)
					{

						//! gameObject.transform.position += Vector3.up * 0.01f;
						if (playerTileArray[j].gameObject.tag == "UnclickableTile")
						{
							cCounter++;
							return;
						}
						else
						{
							highlightList.Add(playerTileArray[j]);
							lCounter++;
							if (lCounter >= rMove)
							{
								cCounter++;
								return;
							}
						}
					}
				}
			}
			cCounter++;
		}
		else
		{
			PopulateDefMat();
			HighlightTile();
			return;
		}

	}

	public void FlushList()
	{
		UnhighlightTile();
		defMat.Clear();
		highlightList.Clear();
		cCounter = 0;
	}

	void HighlightTile()
	{
		for (int i = 0; i < highlightList.Count; i++)
		{
			
			highlightList[i].gameObject.GetComponent<MeshRenderer>().material = Highlighted;
		}
	}

	void UnhighlightTile()
	{
		for (int i = 0; i < highlightList.Count; i++)
		{
			highlightList[i].gameObject.GetComponent<MeshRenderer>().material = defMat[i];
		}
	}

	void PopulateDefMat()
	{
		if (defMat.Count != highlightList.Count)
		{
			for (int i = 0; i < highlightList.Count; i++)
			{
				defMat.Add (highlightList[i].gameObject.GetComponent<MeshRenderer>().material);
			}
		}
		else if (defMat.Count != highlightList.Count)
		{
			defMat.Clear();
		}
		else
		{
			return;
		}


	}
}
