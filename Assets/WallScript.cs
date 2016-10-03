using UnityEngine;
using System.Collections;

public class WallScript : MonoBehaviour {

	public GameObject wall;

	// Use this for initialization
	void Start () {
		Debug.Log("Start");
		//!Instantiate(wall, new Vector3(this.gameObject.transform.position.x, (this.gameObject.transform.position.y + 1), this.gameObject.transform.position.z), this.gameObject.transform.rotation);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
