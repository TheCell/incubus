using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTrigger : MonoBehaviour
{
	[SerializeField] private Towerlogic towerScript;
    [SerializeField] private GameObject particleContainer;
	[SerializeField] private GameObject[] miniatureTowerParts;
	[SerializeField] private Material replacementMaterial;

	private List<GameObject> miniatureTower = new List<GameObject>();
	private List<GameObject>.Enumerator numerator;
	private bool wasActivated;

	private void Start()
	{
		for (int i = 0; i < miniatureTowerParts.Length; i++)
		{
			miniatureTower.Add(miniatureTowerParts[i]);
		}

		 numerator = miniatureTower.GetEnumerator();
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
		if (numerator.MoveNext())
		{
			GameObject towerPart = numerator.Current;
			ReplaceMaterial(towerPart);
		}
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
