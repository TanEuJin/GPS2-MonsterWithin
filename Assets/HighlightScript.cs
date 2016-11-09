using UnityEngine;
using System.Collections;

public class HighlightScript : MonoBehaviour {
	public float tX;
	public float tZ;
	public float pX;
	public float pZ;
	public float rMoves;
	public float counter;

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
	void Update ()
	{
		pX = Player.tileX;
		pZ = Player.tileZ;
		rMoves = Player.remainingMovement;

		if (Player.currentPath == null && rMoves > 0)
		{
			for (float i = 0; i<=rMoves; i++)
			{
					if (pX + i == tX && pZ == tZ)
					{
						//! gameObject.transform.position += Vector3.up * 0.01f;
						if (this.gameObject.tag == "UnclickableTile")
						{
							break;
						}
						else
						{
							counter++;
							Debug.Log(counter);

						}
					}
			}


		}
		else
		{
			//!defRenderer.material = Highlighted;

			defRenderer.material = defMat;
		}

		Highlight();

	}

	void Highlight()
	{
		if (counter == rMoves)
		{
			for (float i = 0; i<=rMoves; i++)
			{
				if (pX + i == tX && pZ == tZ)
				{
					defRenderer.material = Highlighted;
				}
				else
				{
					//!defRenderer.material = Highlighted;

					defRenderer.material = defMat;
				}
			}
		}
		else
		{
			for (float i = 0; i<=counter; i++)
			{
				if (pX + i == tX && pZ == tZ)
				{
					defRenderer.material = Highlighted;
				}
			}
		}
		counter = 0;
	}

}
