using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicControl : MonoBehaviour {

    public AudioMixerSnapshot outOfCompbat;
    public AudioMixerSnapshot inCombat;
    public AudioClip[] stings;
    public AudioSource stingSource;
    public float bpm = 128;
    private float m_TransitionIn;
    private float m_TransitionOut;
    private float m_QuarterNote;


    // Start is called before the first frame update
    void Start()
    {
        m_QuarterNote = 60 / bpm;
        m_TransitionIn = m_QuarterNote;
        m_TransitionOut = m_QuarterNote * 32;

            
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Sound"))
        {
            inCombat.TransitionTo(m_TransitionIn);

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Sound"))
        {
            outOfCompbat.TransitionTo(m_TransitionOut);
        
            }
    }
}
