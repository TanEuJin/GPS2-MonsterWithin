using UnityEngine;
using System.Collections;

public class ShardRotation : MonoBehaviour {

	public bool canDecrease, canIncrease;

	// Use this for initialization
	void Start () {
		canDecrease = true;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (Vector3.up * Time.deltaTime * 10);

		if (canDecrease == true) {
			GetComponent<Light> ().intensity -= 0.5f * Time.deltaTime;
		}

		if (GetComponent<Light> ().intensity >= 2.0f) 
		{
			canDecrease = true;
			canIncrease = false;
		}

		if (canIncrease == true) {
			GetComponent<Light> ().intensity += 0.5f * Time.deltaTime;
		}

		if (GetComponent<Light> ().intensity <= 1.0f) 
		{
			canDecrease = false;
			canIncrease = true;
		}
	}
}
