using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialManagerScript : MonoBehaviour 
{
	private static TutorialManagerScript mInstance;

	public static TutorialManagerScript Instance
	{
		get
		{
			if(mInstance == null)
			{
				GameObject tempObject = GameObject.FindGameObjectWithTag("TutorialManager");

				if(tempObject == null)
				{
					GameObject obj = new GameObject("_TutorialManager");
					mInstance = obj.AddComponent<TutorialManagerScript>();
					obj.tag = "TutorialManager";
				}
				else
				{
					mInstance = tempObject.GetComponent<TutorialManagerScript >();
				}
			}
			return mInstance;
		}
	}

	public Text popUpText;
	public bool isMonsterSpawned = false;

	void Start ()
	{
		EnemyManager.Instance.enabled = false;
	}

	void Update () 
	{
		if(PlayerManager.Instance.tileZ < 4)
		{
			popUpText.text = "To move, click on the highlighted tiles" + "\nAll I have to do is move forwards.";
		}
		else if(PlayerManager.Instance.tileZ >= 4 && PlayerManager.Instance.tileZ < 7)
		{
			popUpText.text = "Staying in light restores sanity" + "\n I always prefer staying in the light.";
		}
		else if(PlayerManager.Instance.tileZ >= 7 && PlayerManager.Instance.tileZ < 9)
		{
			popUpText.text = "You can hide in closets" + "\nI hide when I'm scared";

			if(EnemyManager.Instance.enabled == false)
			{
				EnemyManager.Instance.tileX = 2;
				EnemyManager.Instance.enabled = true;
				isMonsterSpawned = true;
			}
		}
		else if(PlayerManager.Instance.tileZ >= 9)
		{
			popUpText.text = "Find memory shards to finish the level" + "\nI feel better, remembering the past";
		}
	}
}
