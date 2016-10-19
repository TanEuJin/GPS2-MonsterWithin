using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerWall : MonoBehaviour
{
	//The player to shoot the ray at
	public GameObject player;
	//The camera to shoot the ray from
	public GameObject camera1;

	//List of all objects that we have hidden.
	public List<Transform> hiddenObjects;

	//Layers to hide
	public LayerMask layerMask;

	private void Start()
	{
		//Initialize the list
		hiddenObjects = new List<Transform>();

		layerMask = LayerMask.NameToLayer ("Wall");
		layerMask.value = (1 << layerMask.value);
	}

	void Update()
	{
		Vector3 screenPos = Camera.main.WorldToScreenPoint (transform.position);
		Ray rayToPlayerPos = Camera.main.ScreenPointToRay (screenPos);
		//int layermask = (int)(1 << 8);

		RaycastHit hit;
		if (Physics.Raycast (rayToPlayerPos, out hit, 1000, layerMask)) {
			hit.collider.gameObject.SetActive (false);
		} else {
			hit.collider.gameObject.SetActive (true);
		}


		/*//Find the direction from the camera to the player
		Vector3 direction = player.transform.position - camera.transform.position;

		//The magnitude of the direction is the distance of the ray
		float distance = direction.magnitude;

		//Raycast and store all hit objects in an array. Also include the layermaks so we only hit the layers we have specified
		RaycastHit[] hits = Physics.RaycastAll(camera.transform.position, direction, distance, layerMask);

		//Go through the objects
		for (int i = 0; i < hits.Length; i++)
		{
			Transform currentHit = hits[i].transform;

			//Only do something if the object is not already in the list
			if (!hiddenObjects.Contains(currentHit))
			{
				//Add to list and disable renderer
				hiddenObjects.Add(currentHit);
				currentHit.gameObject.SetActive(false);
			}
		}

		//clean the list of objects that are in the list but not currently hit.
		for (int i = 0; i < hiddenObjects.Count; i++)
		{
			bool isHit = false;
			//Check every object in the list against every hit
			for (int j = 0; j < hits.Length; j++)
			{
				if (hits[j].transform == hiddenObjects[i])
				{
					isHit = true;
					break;
				}
			}

			//If it is not among the hits
			if (!isHit)
			{
				//Enable renderer, remove from list, and decrement the counter because the list is one smaller now
				Transform wasHidden = hiddenObjects[i];
				wasHidden.gameObject.SetActive(false);
				hiddenObjects.RemoveAt(i);
				i--;
			}
		}*/
	}
}