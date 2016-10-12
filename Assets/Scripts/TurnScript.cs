using UnityEngine;
using System.Collections;

public class TurnScript : MonoBehaviour 
{
	bool turnPlayer = true;

	void Start () 
	{
	
	}

	void Update () 
	{
		if (turnPlayer == false)
		{
			EnemyManager.Instance.NextTurn ();
			turnPlayer = true;
		}
	}

	public void NTurn()
	{
		if (turnPlayer == true) 
		{
			/*if(PlayerManager.Instance.currentSanityLevel <= 2)
			{
				if(SoundManagerScript.Instance.gameObject.GetComponent<AudioSource>().name != "Heartbeat 180bpm")
				{
					SoundManagerScript.Instance.StopLoopingSFX(AudioClipID.SFX_HEARTBEAT120);
					SoundManagerScript.Instance.PlayLoopingSFX(AudioClipID.SFX_HEARTBEAT180);
				}
			}
			else if(PlayerManager.Instance.currentSanityLevel >=3 && PlayerManager.Instance.currentSanityLevel <= 4)
			{
				if(SoundManagerScript.Instance.gameObject.GetComponent<AudioSource>().name != "Heartbeat 120bpm")
				{
					SoundManagerScript.Instance.StopLoopingSFX(AudioClipID.SFX_HEARTBEAT60);
					SoundManagerScript.Instance.StopLoopingSFX(AudioClipID.SFX_HEARTBEAT180);
					SoundManagerScript.Instance.PlayLoopingSFX(AudioClipID.SFX_HEARTBEAT120);
				}
			}
			else if(PlayerManager.Instance.currentSanityLevel >=5 && PlayerManager.Instance.currentSanityLevel <= 6)
			{
				if(SoundManagerScript.Instance.gameObject.GetComponent<AudioSource>().name != "Heartbeat 60bpm")
				{
					SoundManagerScript.Instance.StopLoopingSFX(AudioClipID.SFX_HEARTBEAT120);
					SoundManagerScript.Instance.PlayLoopingSFX(AudioClipID.SFX_HEARTBEAT60);
				}
			}*/

			PlayerManager.Instance.NextTurn ();
			PlayerManager.Instance.currentPath = null;
			GUIManagerScript.Instance.UpdateSanityBar();

			turnPlayer = false;
		}
	}
}
