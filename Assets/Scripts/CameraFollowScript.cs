using UnityEngine;
using System.Collections;

public class CameraFollowScript : MonoBehaviour {

	public float lerpTime;
	public float currentLerpTime;
	public float moveDistance;

	public Vector3 cameraPos, cameraEndPos;

	void start() {
		cameraPos = transform.position;
		cameraEndPos = transform.position + Vector3.forward * moveDistance;
	}

	void Update() {

		currentLerpTime += Time.deltaTime;
		if (currentLerpTime > lerpTime) {
			currentLerpTime = lerpTime;
		}


		float perc = currentLerpTime / lerpTime;

		if (currentLerpTime > 0f || lerpTime > 0f) {
			transform.position = Vector3.Lerp (cameraPos, cameraEndPos, perc);
		}

		lerpTime += 0.05f;

		//cameraPos = transform.position;
		//cameraEndPos = transform.position + Vector3.right * moveDistance;

		cameraPos = transform.position;
		cameraEndPos.y = 5.1f;
		cameraEndPos.z = PlayerManager.Instance.transform.position.z - 3f;
		cameraEndPos.x = PlayerManager.Instance.transform.position.x - 3f;

	}


}