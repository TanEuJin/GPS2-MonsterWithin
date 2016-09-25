using UnityEngine;
using System.Collections;

public class CubeMovement : MonoBehaviour {

	public WaypointCheck waypointCheck;

	public bool canMoveUp, canMoveLeft, canMoveRight, canMoveDown;
	public int Lightertoggle, SanityMeter;
	public float LighterGas;
	public GameObject LightSource;


	public float lerpTime;
	public float currentLerpTime;
	public float moveDistance;

	public Vector3 playerPos, playerEndPos;

	//public Transform playerPos, playerEndPos;
	//public float speed, startTime, journeyLength;

	// Use this for initialization
	void Start () {
		LighterGas = 10f;
		SanityMeter = 0;

		playerPos = transform.position;
		playerEndPos = transform.position + Vector3.forward * moveDistance;

		//speed = 1f;
		//startTime = 0f;
		//journeyLength = 1f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Movement ();
		NewMovement();

		if (Input.GetKeyDown (KeyCode.F)) 
		{
			if (Lightertoggle == 0) 
			{
				if (LighterGas > 0.0f) 
				{
					//! Set Active Object
					Lightertoggle += 1;
					LightSource.SetActive(true);
				} 

				if (LighterGas <= 0.0f) 
				{
					//! Set Deactivate object
					Lightertoggle -= 1;
					LightSource.SetActive(false);
				}
			}

			else if (Lightertoggle == 1) 
			{
				//! Set Deactivate object
				LightSource.SetActive(false);
				Lightertoggle -= 1;
			}
		}

		if (LighterGas <= 0.0f) 
		{
			//! Set Deactivate object
			Lightertoggle = 0;
			LightSource.SetActive(false);
		}
	}
		

	void OnTriggerEnter(Collider Waypoint)
	{
		if (Waypoint.gameObject.CompareTag ("Waypoint")) {
			if (Waypoint.GetComponent<WaypointCheck>().upAvailable == true) {
				canMoveUp = true;
			}

			if (Waypoint.GetComponent<WaypointCheck>().downAvailable == true) {
				canMoveDown = true;
			}

			if (Waypoint.GetComponent<WaypointCheck>().leftAvailable == true) {
				canMoveLeft = true;
			}

			if (Waypoint.GetComponent<WaypointCheck>().rightAvailable == true) {
				canMoveRight = true;
			}

			if (Waypoint.GetComponent<WaypointCheck> ().isLighted == true) {
				SanityMeter -= 2;
			}

		}
	}

	void OnTriggerExit(Collider Waypoint)
	{
		if (Waypoint.gameObject.CompareTag ("Waypoint")) {
			canMoveUp = false;
			canMoveDown = false;
			canMoveLeft = false;
			canMoveRight = false;
		}
	}

	// Scrapped
	void Movement()
	{
		//float distCovered = (Time.time - startTime) * speed;
		//float fracJourney = distCovered / journeyLength;

		if (Input.GetKeyDown (KeyCode.W)) 
		{
			if(canMoveUp == true)
			{
				//transform.position = Vector3.Lerp (player.playerPos, player.position + Vector3.forward * 3.0f, fracJourney);

				//transform.translate(direction.normalized*speed*Time.Deltatime)
				//transform.Translate(Vector3.forward * 3.0f, Space.World);
				if (Lightertoggle == 1) {
					LighterGas -= 0.5f;
				} else if (Lightertoggle == 0) {
					SanityMeter += 1;
				}
			}

			else
			{
				//Dont do anything
			}
		}

		if (Input.GetKeyDown (KeyCode.S)) 
		{
			if(canMoveDown == true)
			{
				//transform.position = Vector3.Lerp (player.playerPos, player.position + Vector3.back * 3.0f, fracJourney);

				//transform.Translate(Vector3.back * 3.0f, Space.World);
				if (Lightertoggle == 1) {
					LighterGas -= 0.5f;
				} else if (Lightertoggle == 0) {
					SanityMeter += 1;
				}
			}

			else
			{
				//Dont do anything
			}
		}

		if (Input.GetKeyDown (KeyCode.D)) 
		{
			if(canMoveRight == true)
			{
				//transform.position = Vector3.Lerp (player.playerPos, player.position + Vector3.right * 3.0f, fracJourney);

				//transform.Translate(Vector3.right * 3.0f, Space.World);
				if (Lightertoggle == 1) {
					LighterGas -= 0.5f;
				} else if (Lightertoggle == 0) {
					SanityMeter += 1;
				}
			}

			else				
			{
				//Dont do anything
			}
		}

		if (Input.GetKeyDown (KeyCode.A)) 
		{
			if(canMoveLeft == true)
			{
				//transform.position = Vector3.Lerp (player.playerPos, player.position + Vector3.left * 3.0f, fracJourney);

				//transform.Translate(Vector3.left * 3.0f, Space.World);
				if (Lightertoggle == 1) {
					LighterGas -= 0.5f;
				} else if (Lightertoggle == 0) {
					SanityMeter += 1;
				}
			}

			else
			{
				//Dont do anything
			}
		}

		if (Input.GetKeyUp (KeyCode.E)) {
			transform.Rotate (Vector3.up * 90f);
		}

		if (Input.GetKeyUp (KeyCode.Q)) {
			transform.Rotate (Vector3.down * 90f);
		}
	}

	void NewMovement()
	{
		currentLerpTime += Time.deltaTime;
		if (currentLerpTime > lerpTime) {
			currentLerpTime = lerpTime;
		}

		float perc = currentLerpTime / lerpTime;
		if(currentLerpTime>0f || lerpTime>0f)
		transform.position = Vector3.Lerp (playerPos, playerEndPos, perc);

		if (Input.GetKeyDown (KeyCode.W)) {
			if (canMoveUp == true) {
				lerpTime += 0.05f;

				playerPos = transform.position;
				playerEndPos = transform.position + Vector3.forward * moveDistance;

				if (Lightertoggle == 1) {
					LighterGas -= 0.5f;
				} else if (Lightertoggle == 0) {
					SanityMeter += 1;
				}

			} else {
				//dont do anything
			}
		}

		if (Input.GetKeyDown(KeyCode.A)){
			if (canMoveLeft == true) {
				lerpTime += 0.05f;

				playerPos = transform.position;
				playerEndPos = transform.position + Vector3.left * moveDistance;

				if (Lightertoggle == 1) {
					LighterGas -= 0.5f;
				} else if (Lightertoggle == 0) {
					SanityMeter += 1;
				}

			} else {
				//dont do anything
			}
		}

		if (Input.GetKeyDown(KeyCode.S)){
			if (canMoveDown == true) {
				lerpTime += 0.05f;

				playerPos = transform.position;
				playerEndPos = transform.position + Vector3.back * moveDistance;

				if (Lightertoggle == 1) {
					LighterGas -= 0.5f;
				} else if (Lightertoggle == 0) {
					SanityMeter += 1;
				}

			} else {
				//dont do anything
			}

		}

		if (Input.GetKeyDown(KeyCode.D)){
			if (canMoveRight == true) {
				lerpTime += 0.05f;

				playerPos = transform.position;
				playerEndPos = transform.position + Vector3.right * moveDistance;

				if (Lightertoggle == 1) {
					LighterGas -= 0.5f;
				} else if (Lightertoggle == 0) {
					SanityMeter += 1;
				}

			} else {
				//dont do anything
			}

		}

		if (Input.GetKeyUp (KeyCode.E)) {
			transform.Rotate (Vector3.up * 90f);
		}

		if (Input.GetKeyUp (KeyCode.Q)) {
			transform.Rotate (Vector3.down * 90f);
		}

		if ((Input.GetKeyUp (KeyCode.W)) || (Input.GetKeyUp (KeyCode.A)) || (Input.GetKeyUp (KeyCode.S)) || (Input.GetKeyUp (KeyCode.D))){
			lerpTime = 0f;
		}
	}


}