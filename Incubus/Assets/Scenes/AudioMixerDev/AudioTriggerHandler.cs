using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTriggerHandler : MonoBehaviour
{
	[SerializeField] private AudioMixer audioMixer;

	private void OnTriggerEnter(Collider other)
	{
		AudioInfo audioInfo = other.GetComponent<AudioInfo>();
		if (audioInfo == null)
		{
			return;
		}

		audioMixer.SetSnapshot(audioInfo);
	}

	private void OnTriggerExit(Collider other)
	{
		AudioInfo audioInfo = other.GetComponent<AudioInfo>();
		if (audioInfo == null)
		{
			return;
		}

		audioMixer.RemoveSnapshot(audioInfo);
	}
}
