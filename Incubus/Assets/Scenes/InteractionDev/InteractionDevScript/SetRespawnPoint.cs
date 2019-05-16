using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRespawnPoint : MonoBehaviour
{
	[SerializeField] private Transform spawnPoint;
	[SerializeField] private RespawnEffect respawnEffect;
	[SerializeField] private ParticleSystem isActiveParticles;

	private void OnTriggerEnter(Collider collider)
	{
		RespawnInfo respawnInfo = collider.gameObject.GetComponent<RespawnInfo>();

		if (respawnInfo != null)
		{
			respawnInfo.RespawnPosition = spawnPoint.position;
			respawnInfo.RespawnEffect = respawnEffect;
			if (respawnInfo.activeParticleSystem != null)
			{
				respawnInfo.activeParticleSystem.Stop();
			}
			respawnInfo.activeParticleSystem = isActiveParticles;
			respawnInfo.activeParticleSystem.Play();
		}
	}

	/*
	private void SetSpawnActive(bool activeState)
	{
		if (activeState)
		{
			isActiveParticles.Play();
		}
		else
		{
			isActiveParticles.Stop();
		}
	}
	*/
}
