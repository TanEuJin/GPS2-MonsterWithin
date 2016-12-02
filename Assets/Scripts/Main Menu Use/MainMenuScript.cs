using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour {

	//public float speedFactor = 0.1f; 
	//public float zoomFactor = 1.0f; 
	//public Transform currentMount; 
	//public Camera cameraComp; Vector3 lastPosition; 
	public GameObject creditCanvas;
	//public GameObject settingCanvas;
	public GameObject mainCanvas;
	public GameObject exitCanvas;
	public AudioSource BookFlipUI;


	// Use this for initialization
	void Start () {
		//lastPosition = transform.position; 
	}
	
	// Update is called once per frame
	void Update () {
		/*transform.position = Vector3.Lerp(transform.position, currentMount.position, speedFactor); 
		transform.rotation = Quaternion.Slerp(transform.rotation, currentMount.rotation, speedFactor); 
		var velocity = Vector3.Magnitude(transform.position - lastPosition); 
		cameraComp.fieldOfView = 60 + velocity * zoomFactor; lastPosition = transform.position;*/
	}
		
		
	public void startGame(){
		SceneManager.LoadScene("_SCENE_");
	}

	public void tutorialGame(){
		SceneManager.LoadScene("TUTORIAL_SCENE_");
	}

	public void exitGame(){
		Application.Quit();
	}

	public void goback(){
		creditCanvas.SetActive (false);
		//settingCanvas.SetActive (false);
		exitCanvas.SetActive (false);
		mainCanvas.SetActive (true);
		BookFlipUI.Play ();
	}

	public void settingScreen(){
		//settingCanvas.SetActive (true);
		mainCanvas.SetActive (false);
		BookFlipUI.Play ();
	}

	public void creditScreen(){
		creditCanvas.SetActive (true);
		mainCanvas.SetActive (false);
		BookFlipUI.Play ();
	}

	public void exitConfirmation(){
		exitCanvas.SetActive (true);
		mainCanvas.SetActive (false);
		BookFlipUI.Play ();
	}


	/*public void revertPosition(Transform newMount){ 
		settingCanvas.SetActive (false);
		creditCanvas.SetActive (false);
		currentMount = newMount; 
	}

	public void changePositionSetting(Transform newMount){
		currentMount = newMount; 
		settingCanvas.SetActive (true);
	}

	public void changePositionCredit(Transform newMount){
		currentMount = newMount;
		creditCanvas.SetActive (true);
	}*/

}
