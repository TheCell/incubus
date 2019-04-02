using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnEffect : MonoBehaviour
{
	ParticleSystem[] particles;

	private void Start()
    {
		particles = gameObject.GetComponentsInChildren<ParticleSystem>();
		if (particles == null)
		{
			particles = new ParticleSystem[0];
		}
    }
	
	public void PlayOnce()
	{
		if (particles == null)
		{
			return;
		}

		for (int i = 0; i < particles.Length; i++)
		{
			particles[i].Play();
		}
	}
}
