using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTrigger : MonoBehaviour
{
	[SerializeField] private Towerlogic towerScript;
    [SerializeField] private GameObject particleContainer;
	[SerializeField] private GameObject[] miniatureTowerParts;
	[SerializeField] private Material replacementMaterial;
	private static int numberOfPiecesRemoved = 0;
    private int materialsUpdated = 0;
	private bool wasActivated;
    
	public static void ResetCounter()
	{
		numberOfPiecesRemoved = 0;
	}

    private void Update()
    {
        UpdateTowerMaterial();
    }

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
			RemoveMiniatureTowerPart();
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

	private void RemoveMiniatureTowerPart()
	{
		numberOfPiecesRemoved++;
    }

	private void UpdateTowerMaterial()
	{
        if (materialsUpdated == numberOfPiecesRemoved)
        {
            return;
        }

		for (int i = 0; i < numberOfPiecesRemoved; i++)
		{
			if (miniatureTowerParts.Length > i)
			{
				ReplaceMaterial(miniatureTowerParts[i]);
			}
		}

        materialsUpdated = numberOfPiecesRemoved;
    }

	private void ReplaceMaterial(GameObject gameObject)
	{
		MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
		if (renderer)
		{
			Material[] materials = renderer.materials;
			for (int i = 0; i < materials.Length; i++)
			{
				materials[i] = replacementMaterial;
			}
			renderer.materials = materials;
		}
	}
}
