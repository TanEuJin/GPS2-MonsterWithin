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
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
