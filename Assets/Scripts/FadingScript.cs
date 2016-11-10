using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadingScript : MonoBehaviour {
	public Color FadeBack;
	public float fadeVal;
	public float transValue = 0.0f;
	public float timer = 0.0f;
	public float duration = 5.0f;

	// Use this for initialization
	void Start () {
		timer = 0.0f;
		FadeBack = GetComponent<Image>().color;
	}
	
	// Update is called once per frame
	void Update () {
		FadeBack.a = transValue;
		GetComponent<Image>().color = FadeBack;

		if (timer <= duration)
		{
			timer += Time.deltaTime;
		}
		else
		{
			if (transValue <= 1.25f)
			{
				transValue += fadeVal;
			}
			else
			{
				SceneManager.LoadSceneAsync("MainMenu");
			}
		}
	}

}
