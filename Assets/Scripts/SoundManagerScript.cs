﻿using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public enum AudioClipID
{
	BGM_MAIN_MENU = 0,
	BGM_MANSION = 1,
	BGM_GAMEOVER = 2,

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
	public AudioClip[] Spotted;
	public AudioSource seenByTheEnemy;
	public AudioSource SpottedSource;
	public AudioSource Horrified;
	public AudioSource PlayerMove;
	public AudioSource EnemyMove;
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

	public float bgmVolume = 1.0f;
	public float sfxVolume = 1.0f;


	public List<AudioClipInfo> audioClipInfoList = new List<AudioClipInfo>();

	public AudioSource bgmAudioSource;
	public AudioSource sfxAudioSource;

	public List<AudioSource> sfxAudioSourceList = new List<AudioSource>();
	public List<AudioSource> bgmAudioSourceList = new List<AudioSource>();

	void Start()
	{
		m_QuarternNote = 60 / bpm;
		m_TransitionIn = m_QuarternNote;
		m_TransitionOut = m_QuarternNote * 32;

		//PlaySFX(AudioClipID.BGM_MANSION);
	}

	// Use this for initialization
	void Awake () 
	{
		AudioSource[] audioSourceList = this.GetComponentsInChildren<AudioSource>();

		if(audioSourceList[0].gameObject.name == "BGMAudioSource")
		{
			bgmAudioSource = audioSourceList[0];
			sfxAudioSource = audioSourceList[1];
		}
		else 
		{
			bgmAudioSource = audioSourceList[1];
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

	//! BACKGROUND MUSIC (BGM)
	public void PlayBGM(AudioClipID audioClipID)
	{
		bgmAudioSource.clip = FindAudioClip(audioClipID);
		Debug.Log (audioClipID);
		bgmAudioSource.volume = bgmVolume;
		bgmAudioSource.Play();
	}

	public void PauseBGM()
	{
		if(bgmAudioSource.isPlaying)
		{
			bgmAudioSource.Pause();
		}
	}

	public void StopBGM()
	{
		if(bgmAudioSource.isPlaying)
		{
			bgmAudioSource.Stop();
		}
	}

	public void PlayLoopingBGM(AudioClipID audioClipID)
	{
		AudioClip clipToPlay = FindAudioClip(audioClipID);

		for(int i=0; i<bgmAudioSourceList.Count; i++)
		{
			if(bgmAudioSourceList[i].clip == clipToPlay)
			{
				if(bgmAudioSourceList[i].isPlaying)
				{
					return;
				}

				bgmAudioSourceList[i].volume = sfxVolume;
				bgmAudioSourceList[i].Play();
				return;
			}
		}

		AudioSource newInstance = gameObject.AddComponent<AudioSource>();
		newInstance.clip = clipToPlay;
		newInstance.volume = sfxVolume;
		newInstance.loop = true;
		newInstance.Play();
		bgmAudioSourceList.Add(newInstance);
	}

	public void PauseLoopingBGM(AudioClipID audioClipID)
	{
		AudioClip clipToPause = FindAudioClip(audioClipID);

		for(int i=0; i<bgmAudioSourceList.Count; i++)
		{
			if(bgmAudioSourceList[i].clip == clipToPause)
			{
				bgmAudioSourceList[i].Pause();
				return;
			}
		}
	}


	public void StopLoopingBGM(AudioClipID audioClipID)
	{
		AudioClip clipToStop = FindAudioClip(audioClipID);

		for(int i=0; i<bgmAudioSourceList.Count; i++)
		{
			if(bgmAudioSourceList[i].clip == clipToStop)
			{
				bgmAudioSourceList[i].Stop();
				return;
			}
		}
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

	public void SetBGMVolume(float value)
	{
		bgmVolume = value;
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