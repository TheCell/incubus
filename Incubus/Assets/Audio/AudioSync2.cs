using UnityEngine;
using System.Collections;

public class AudioSync2 : MonoBehaviour
{


    //set these in the inspector!
    private AudioSource master;
    public AudioSource[] slaves;

    private void Start()
    {
        master = GetComponent<AudioSource>();
    }

    private void Update()
    {
        foreach( AudioSource slave in slaves)
        {
            slave.timeSamples = master.timeSamples;
        }
    }

    private IEnumerator SyncSources()
    {
        while (true)
        {
            foreach (var slave in slaves)
            {
                slave.timeSamples = master.timeSamples;
                yield return null;
            }
        }
    }
}