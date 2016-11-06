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

	public Material transparent;
	public Material opaque;

	private void Start()
	{
		//Initialize the list
		hiddenObjects = new List<Transform>();

		layerMask = LayerMask.NameToLayer ("Wall");
		//Debug.Log ("ccccccccc " + layerMask.value);
		layerMask = (1 << layerMask.value);
	}

	void Update()
	{
		//Find the direction from the camera to the player
		Vector3 direction = player.transform.position - camera1.transform.position;

		//The magnitude of the direction is the distance of the ray
		float distance = direction.magnitude;

		//Raycast and store all hit objects in an array. Also include the layermaks so we only hit the layers we have specified
		RaycastHit[] hits = Physics.RaycastAll(camera1.transform.position, direction, distance, layerMask);

		//Go through the objects
		for (int i = 0; i < hits.Length; i++)
		{
			Transform currentHit = hits[i].transform;

			//Only do something if the object is not already in the list
			if (!hiddenObjects.Contains(currentHit))
			{
				//Add to list and disable renderer
				hiddenObjects.Add(currentHit);
				currentHit.gameObject.GetComponent<Renderer>().material = transparent;
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
				wasHidden.gameObject.GetComponent<Renderer>().material = opaque;
				hiddenObjects.RemoveAt(i);
				i--;
			}
		}
	}
}