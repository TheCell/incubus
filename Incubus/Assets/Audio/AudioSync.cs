using UnityEngine;
using System.Collections;

public class AudioSync : MonoBehaviour
{
    //set these in the inspector!
    public AudioSource master;
    public AudioSource slave;

    void Update()
    {
        slave.timeSamples = master.timeSamples;
    }
}