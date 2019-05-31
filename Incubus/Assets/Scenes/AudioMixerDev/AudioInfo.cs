using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioInfo : MonoBehaviour
{
	[SerializeField] private AudioMixerSnapshot audioSnapshot;
	[SerializeField] private float bpm;
	private float fadeInTime;
	private float fadeOutTime;
	private float quarterNote;

	public AudioMixerSnapshot AudioSnapshot { get => audioSnapshot; }
	public float Bpm { get => bpm; }
	public float FadeInTime { get => fadeInTime; }
	public float FadeOutTime { get => fadeOutTime; }
	public float QuarterNote { get => quarterNote; }

	void Start()
    {
		quarterNote = 60 / bpm;
		fadeInTime = quarterNote;
		fadeOutTime = quarterNote;
	}
}
