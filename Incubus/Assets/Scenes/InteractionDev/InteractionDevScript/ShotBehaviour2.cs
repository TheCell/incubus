using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotBehaviour2 : MonoBehaviour
{
	private Rigidbody bulletRigidbody;
	private ParticleSystem particles;

	private void Start()
	{
		bulletRigidbody = GetComponentInChildren<Rigidbody>();
	}

	private void OnEnable()
	{
		particles = GetComponentInChildren<ParticleSystem>();

		if (particles != null)
		{
			particles.Play();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 11)
		{
			// ignore triggers layer
			return;
		}

		if (other.gameObject.layer == 8) // player layer
		{
			RespawnInfo respawnInfo = other.GetComponent<RespawnInfo>();
			if (respawnInfo != null)
			{
				RespawnOrKill.Respawn(other.gameObject, respawnInfo);
			}

			bulletRigidbody.gameObject.SetActive(false);
		}
		else
		{
			gameObject.SetActive(false);
		}
	}
}
