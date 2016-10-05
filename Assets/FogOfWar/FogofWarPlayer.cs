using UnityEngine;
using System.Collections;

public class FogofWarPlayer : MonoBehaviour {

	public Transform FogofwarPlane;

	public LayerMask layerMask;
	//public int Number = 1;

	// Use this for initialization
	void Start () {
		layerMask = LayerMask.NameToLayer ("Wall");
		layerMask.value = ~(1 << layerMask.value);
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 screenPos = Camera.main.WorldToScreenPoint (transform.position);
		Ray rayToPlayerPos = Camera.main.ScreenPointToRay (screenPos);
		//int layermask = (int)(1 << 8);

		RaycastHit hit;
		if (Physics.Raycast (rayToPlayerPos, out hit, 1000, layerMask)) {
			//FogofwarPlane.GetComponent<Renderer> ().material.SetVector ("_Player" + Number.ToString() + "_Pos", hit.point);
			FogofwarPlane.GetComponent<Renderer> ().material.SetVector ("_Player1_Pos", hit.point);
		}
	}
}
