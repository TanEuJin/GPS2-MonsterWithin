using UnityEngine;
using System.Collections;

public class HideObjectScript : MonoBehaviour {

	private static HideObjectScript mInstance;

	public static HideObjectScript Instance
	{
		get
		{
			if(mInstance == null)
			{
				GameObject tempObject = GameObject.FindGameObjectWithTag("HideObject");

				if(tempObject == null)
				{
					GameObject obj = new GameObject("_HideObject");
					mInstance = obj.AddComponent<HideObjectScript>();
					obj.tag = "HideObject";
				}
				else
				{
					mInstance = tempObject.GetComponent<HideObjectScript>();
				}
			}
			return mInstance;
		}
	}

	public Animator anim;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void HideInteract()
	{
		if (PlayerManager.Instance.HideInteract == true) 
		{
			anim.Play ("Open");
			if (PlayerManager.Instance.isHidden == false)
			{
				SoundManagerScript.Instance.ClosetSound.Play ();
				PlayerManager.Instance.playerModel.SetActive(false);
				PlayerManager.Instance.isHidden = true;
			} 

			else if (PlayerManager.Instance.isHidden == true) 
			{
				SoundManagerScript.Instance.ClosetSound.Play ();
				PlayerManager.Instance.playerModel.SetActive(true);
				PlayerManager.Instance.isHidden = false;
			}
		}
	}

}
