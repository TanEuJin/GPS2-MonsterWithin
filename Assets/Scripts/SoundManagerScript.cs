using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public enum AudioClipID
{
	SFX_PLAYERMOVE = 101,
	SFX_DOOROPEN = 100,
	SFX_ENEMYMOVE = 99,
	SFX_HIDE = 98,
	SFX_PLAYERPANIC = 97,
	SFX_LANTERNONOFF = 96,
	SFX_HEARTBEAT60 = 95,
	SFX_HEARTBEAT120 = 94,
	SFX_HEARTBEAT180 = 93,

	TOTAL = 9001
}

[System.Serializable]
public class AudioClipInfo
{
	public AudioClipID audioClipID;
	public AudioClip audioClip;
}


public class SoundManagerScript : MonoBehaviour 
{
	public AudioMixerSnapshot notSeenByEnemy;
	public AudioMixerSnapshot seenByEnemy;
	public AudioMixerSnapshot sanityLow;
	public AudioMixerSnapshot proximityDread;
	public AudioClip[] Spotted;
	public AudioSource seenByTheEnemy;
	public AudioSource SpottedSource;
	public AudioSource Horrified;
	public AudioSource Dread;
	public AudioSource EnemyMove;
	public AudioSource ClosetSound;
	public AudioSource CreakyFloor;
	public AudioSource BookFlipUI;

	public AudioSource player1Step, player2Step, player3Step, player4Step;
	//public AudioSource Monster2Step, Monster3Step, Monster4step;

	public float bpm = 128;

	public float m_TransitionIn;
	public float m_TransitionOut;
	public float m_QuarternNote;

	/////////////////////////////////////////
	/////////////////////////////////////////

	private static SoundManagerScript mInstance;

	public static SoundManagerScript Instance
	{
		get
		{
			if(mInstance == null)
			{
				if(GameObject.FindWithTag("SoundManager") != null)
				{
					mInstance = GameObject.FindWithTag("SoundManager").GetComponent<SoundManagerScript>();
				}
				else 
				{
					GameObject obj = new GameObject("_SoundManager");
					mInstance = obj.AddComponent<SoundManagerScript>();
				}
				//!DontDestroyOnLoad(obj);
			}
			return mInstance;
		}
	}

	public float sfxVolume = 1.0f;


	public List<AudioClipInfo> audioClipInfoList = new List<AudioClipInfo>();

	public AudioSource sfxAudioSource;
	public List<AudioSource> sfxAudioSourceList = new List<AudioSource>();

	void Start()
	{
		m_QuarternNote = 60 / bpm;
		m_TransitionIn = m_QuarternNote;
		m_TransitionOut = m_QuarternNote * 16;

		//PlaySFX(AudioClipID.BGM_MANSION);
	}

	// Use this for initialization
	void Awake () 
	{
		AudioSource[] audioSourceList = this.GetComponentsInChildren<AudioSource>();

		if(audioSourceList[0].gameObject.name == "BGMAudioSource")
		{
			sfxAudioSource = audioSourceList[1];
		}
		else 
		{
			sfxAudioSource = audioSourceList[0];
		}
	}

	// Update is called once per frame
	void Update () 
	{

	}

	AudioClip FindAudioClip(AudioClipID audioClipID)
	{
		for(int i=0; i<audioClipInfoList.Count; i++)
		{
			if(audioClipInfoList[i].audioClipID == audioClipID)
			{
				return audioClipInfoList[i].audioClip;
			}
		}

		Debug.LogError("Cannot Find Audio Clip : " + audioClipID);

		return null;
	}

	//! SOUND EFFECTS (SFX)
	public void PlaySFX(AudioClipID audioClipID)
	{
		sfxAudioSource.PlayOneShot(FindAudioClip(audioClipID), sfxVolume);
	}

	public void PlayLoopingSFX(AudioClipID audioClipID)
	{
		AudioClip clipToPlay = FindAudioClip(audioClipID);

		for(int i=0; i<sfxAudioSourceList.Count; i++)
		{
			if(sfxAudioSourceList[i].clip == clipToPlay)
			{
				if(sfxAudioSourceList[i].isPlaying)
				{
					return;
				}

				sfxAudioSourceList[i].volume = sfxVolume;
				sfxAudioSourceList[i].Play();
				return;
			}
		}

		AudioSource newInstance = gameObject.AddComponent<AudioSource>();
		newInstance.clip = clipToPlay;
		newInstance.volume = sfxVolume;
		newInstance.loop = true;
		newInstance.Play();
		sfxAudioSourceList.Add(newInstance);
	}

	public void PauseLoopingSFX(AudioClipID audioClipID)
	{
		AudioClip clipToPause = FindAudioClip(audioClipID);

		for(int i=0; i<sfxAudioSourceList.Count; i++)
		{
			if(sfxAudioSourceList[i].clip == clipToPause)
			{
				sfxAudioSourceList[i].Pause();
				return;
			}
		}
	}


	public void StopLoopingSFX(AudioClipID audioClipID)
	{
		AudioClip clipToStop = FindAudioClip(audioClipID);

		for(int i=0; i<sfxAudioSourceList.Count; i++)
		{
			if(sfxAudioSourceList[i].clip == clipToStop)
			{
				sfxAudioSourceList[i].Stop();
				return;
			}
		}
	}
		
	public void SetSFXVolume(float value)
	{
		sfxVolume = value;
	}

	/*void OnTriggerEnter(Collider Other)
	{
		if (Other.CompareTag ("Monster")) 
		{
			seenByTheEnemy.Play();
			seenByEnemy.TransitionTo (m_TransitionIn);
			playTransition ();
		}
	}

	void OnTriggerExit(Collider Other)
	{
		if (Other.CompareTag ("Monster")) 
		{
			notSeenByEnemy.TransitionTo (m_TransitionOut);
		}
	}*/

	public void playTransition()
	{
		int randSpottedSource = Random.Range (0, Spotted.Length);
		SpottedSource.clip = Spotted[randSpottedSource];
		SpottedSource.Play();
	}
		
}