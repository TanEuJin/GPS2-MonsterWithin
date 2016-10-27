using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneManagerScript : MonoBehaviour 
{
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

		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void MainMenu()
	{
		if(Time.timeScale == 0)
		{
			Time.timeScale = 1;
		}

		SceneManager.LoadScene("MainMenu");
	}
}
