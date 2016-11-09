using UnityEngine;
using System.Collections;

public class PlayerEnemyDissapear : MonoBehaviour {
	public float radius = 10.0f;
	public LayerMask layerMask;

	// Use this for initialization
	void Start () {
		//layerMask = LayerMask.NameToLayer ("Enemy");
	}

	// Update is called once per frame
	void Update () 
	{
		foreach(Collider col in Physics.OverlapSphere(transform.position, radius, layerMask))
		{
			//SendMessage("Observed", SendMessageOptions.DontRequireReceiver);
			col.GetComponent<EnemyPlayerDissapear>().observed = true;
		}
	}
}
