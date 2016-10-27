using UnityEngine;
using System.Collections;

public class HighlightScript : MonoBehaviour {
	public float tX;
	public float tZ;
	public float pX;
	public float pZ;
	public float rMoves;

	public PlayerManager Player;

	public Material Highlighted;
	Material defMat;
	MeshRenderer defRenderer;
	// Use this for initialization
	void Start () {
		tX = GetComponent<ClickableTile>().tileX;
		tZ = GetComponent<ClickableTile>().tileZ;
		Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
		defMat = this.gameObject.GetComponent<MeshRenderer>().material;
		defRenderer = this.gameObject.GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		pX = Player.tileX;
		pZ = Player.tileZ;
		rMoves = Player.remainingMovement;
		if (Player.currentPath == null && rMoves > 0)
		{
			if (pX + rMoves == tX && pZ == tZ || pX - rMoves == tX && pZ == tZ)
			{
				//! gameObject.transform.position += Vector3.up * 0.01f;
				defRenderer.material = Highlighted;
			}
			else
			{
				defRenderer.material = defMat;
			}
		}
		else
		{
			defRenderer.material = defMat;
		}

	}
}
