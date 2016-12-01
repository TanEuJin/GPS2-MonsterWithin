using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneManagerScript : MonoBehaviour 
{
	private static SceneManagerScript mInstance;

	public static SceneManagerScript Instance
	{
		get
		{
			if(mInstance == null)
			{
				GameObject tempObject = GameObject.FindGameObjectWithTag("SceneManager");

				if(tempObject == null)
				{
					GameObject obj = new GameObject("_SceneManager");
					mInstance = obj.AddComponent<SceneManagerScript>();
					obj.tag = "SceneManager";
				}
				else
				{
					mInstance = tempObject.GetComponent<SceneManagerScript >();
				}
			}
			return mInstance;
		}
	}

	void Start ()
	{
	
	}

	void Update () 
	{

	}

	public void StartGame()
	{
		SceneManager.LoadScene("_SCENE_");
	}

	public void RestartScene()
	{
		if(Time.timeScale == 0)
		{
			Time.timeScale = 1;
		}
		SoundManagerScript.Instance.BookFlipUI.Play ();
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		SoundManagerScript.Instance.notSeenByEnemy.TransitionTo (SoundManagerScript.Instance.m_TransitionIn);
	}

	public void MainMenu()
	{
		if(Time.timeScale == 0)
		{
			Time.timeScale = 1;
		}

		SceneManager.LoadScene("MainMenu");
	}

	public void LoseScene()
	{
		SceneManager.LoadScene("LoseScene");
	}

	public string GetSceneName()
	{
		return SceneManager.GetActiveScene().name;
	}
}
