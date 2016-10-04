using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GUIManagerScript : MonoBehaviour 
{
	private static GUIManagerScript mInstance;

	public static GUIManagerScript Instance
	{
		get
		{
			if(mInstance == null)
			{
				GameObject tempObject = GameObject.FindGameObjectWithTag("GUIManager");

				if(tempObject == null)
				{
					GameObject obj = new GameObject("_GUIManager");
					mInstance = obj.AddComponent<GUIManagerScript>();
					obj.tag = "GUIManager";
				}
				else
				{
					mInstance = tempObject.GetComponent<GUIManagerScript >();
				}
			}
			return mInstance;
		}
	}

	public Image sanityBar;

	void Start()
	{
		
	}

	void Update()
	{
		
	}

	public void UpdateSanityBar()
	{
		sanityBar.fillAmount = (PlayerManager.Instance.currentSanityLevel / PlayerManager.Instance.maxSanityLevel) * 1.0f;

		if(PlayerManager.Instance.currentSanityLevel >=0 && PlayerManager.Instance.currentSanityLevel <=2)
		{
			sanityBar.color = Color.red;
		}
		else if(PlayerManager.Instance.currentSanityLevel >=3 && PlayerManager.Instance.currentSanityLevel <=4)
		{
			sanityBar.color = Color.yellow;
		}
		else if(PlayerManager.Instance.currentSanityLevel >=5 && PlayerManager.Instance.currentSanityLevel <=6)
		{
			sanityBar.color = Color.green;
		}
	}

	public void ClosetInteract()
	{
		Debug.Log ("Test");
		if (PlayerManager.Instance.closetInteract == true) 
		{
			if (PlayerManager.Instance.isHidden == false)
			{
				PlayerManager.Instance.transform.position = PlayerManager.Instance.Closet.transform.position;
				PlayerManager.Instance.isHidden = true;
			} 

			else if (PlayerManager.Instance.isHidden == true) 
			{
				PlayerManager.Instance.transform.position = Vector3.forward * 1.0f;
				PlayerManager.Instance.isHidden = false;
			}
		}
	}

	public void DoorInteract()
	{
		Debug.Log ("Test");
		if (PlayerManager.Instance.doorInteract == true) 
		{
			if (PlayerManager.Instance.keys == 3)
			{
				//! Door Unlocked
				PlayerManager.Instance.Door.transform.position = Vector3.up * 1.0f;
			} 

			else if (PlayerManager.Instance.keys < 3) 
			{
				Debug.Log("Insufficient Keys!");
			}
		}
	}

}
