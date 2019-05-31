using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixer : MonoBehaviour
{
	[SerializeField] AudioMixerSnapshot basicSnapshot;
	private List<AudioInfo> snapshotOrder;

	private void Start()
    {
		snapshotOrder = new List<AudioInfo>();
		basicSnapshot.TransitionTo(0f); // set first Snapshot at start
    }

	public void SetSnapshot(AudioInfo audioInfo)
	{
		//Debug.Log("setting snapshot " + audioInfo.AudioSnapshot.name);
		snapshotOrder.Add(audioInfo);
		Transition(false);
	}

	public void RemoveSnapshot(AudioInfo audioInfo)
	{
		//Debug.Log("removing snapshot " + audioInfo.AudioSnapshot.name);
		snapshotOrder.Remove(audioInfo);
		Transition(true);
	}

	private void Transition(bool isOutTransition)
	{
		//Debug.Log(snapshotOrder.Count);
		if (snapshotOrder.Count > 0)
		{
			AudioInfo mostRecentSnapshot = snapshotOrder[snapshotOrder.Count - 1];
			if (isOutTransition)
			{
				mostRecentSnapshot.AudioSnapshot.TransitionTo(mostRecentSnapshot.FadeOutTime);
			}
			else
			{
				mostRecentSnapshot.AudioSnapshot.TransitionTo(mostRecentSnapshot.FadeInTime);
			}
			//Debug.Log("now playing " + mostRecentSnapshot.AudioSnapshot.name);
		}
		else
		{
			basicSnapshot.TransitionTo(15f);
			//Debug.Log("now playing " + basicSnapshot.name);
		}
	}
}
