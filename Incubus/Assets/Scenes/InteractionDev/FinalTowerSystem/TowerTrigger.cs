using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTrigger : MonoBehaviour
{
	[SerializeField] private Towerlogic towerScript;
    [SerializeField] private GameObject particleContainer;

	private bool wasActivated;

	private void OnTriggerEnter(Collider other)
	{
		if (wasActivated)
		{
			return;
		}

		if (other.tag == "Player")
		{
			wasActivated = true;
            PlayParticles();
            towerScript.RemovePart();
		}
	}

    private void PlayParticles()
    {
        if (particleContainer != null)
        {
            var particleSystems = particleContainer.GetComponentsInChildren<ParticleSystem>();
            foreach ( ParticleSystem particleSystem in particleSystems)
            {
                particleSystem.Play();
            };
        }
    }
}
