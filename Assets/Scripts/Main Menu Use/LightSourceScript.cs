using UnityEngine;
using System.Collections;

public class LightSourceScript : MonoBehaviour {

	public bool canDecrease, canIncrease;

	// Use this for initialization
	void Start () {
		canDecrease = true;
	}

	// Update is called once per frame
	void Update () {

		if (canDecrease == true) {
			GetComponent<Light> ().intensity -= 0.5f * Time.deltaTime;
		}

		if (GetComponent<Light> ().intensity >= 3.0f) 
		{
			canDecrease = true;
			canIncrease = false;
		}

		if (canIncrease == true) {
			GetComponent<Light> ().intensity += 0.5f * Time.deltaTime;
		}

		if (GetComponent<Light> ().intensity <= 2.5f) 
		{
			canDecrease = false;
			canIncrease = true;
		}
	}

}
