using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicControl1 : MonoBehaviour {

    public AudioMixerSnapshot outOfCompbat2;
    public AudioMixerSnapshot inCombat2;
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

        if (other.CompareTag("Sound2"))
        {
            inCombat2.TransitionTo(m_TransitionIn);

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Sound2"))
        {
            outOfCompbat2.TransitionTo(m_TransitionOut);
        
            }
    }
}
