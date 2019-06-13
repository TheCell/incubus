using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Towerlogic : MonoBehaviour
{
	[SerializeField] private List<GameObject> towerParts;
	[SerializeField] private GameObject topPart;
	private ParticleSystem particles;
	private GameEndTrigger gameEndTrigger;

	private void Start()
	{
		TowerTrigger.ResetCounter();
		towerParts.Reverse();
		gameEndTrigger = GetComponentInChildren<GameEndTrigger>();
		particles = topPart.GetComponentInChildren<ParticleSystem>();
		if (particles == null)
		{
			Debug.LogError("particlesystem not found");
		}
	}

	public void RemovePart()
	{
		IEnumerator<GameObject> partEnumerator = towerParts.GetEnumerator();
		if (partEnumerator.MoveNext())
		{
			GameObject element = partEnumerator.Current;
			Bounds towerPartBounds = GetMaxBoundsOfChilds(element);
			towerParts.Remove(element);
			Destroy(element);
			Vector3 topPos = topPart.transform.position;
			topPos.y = topPos.y - towerPartBounds.size.y;
			topPart.transform.position = topPos;
			PlayParticles();
		}

		if (towerParts.Count <= 0)
		{
			PrepareVideoWhenAllPartsRemoved();
		}
	}

	private void PrepareVideoWhenAllPartsRemoved()
	{
		if (gameEndTrigger != null)
		{
			gameEndTrigger.prepareVideo();
		}
	}

	private void PlayParticles()
	{
		particles.Play();
	}
	/*
	private void OnDrawGizmos()
	{
		// Draw each child's bounds as a green box.
		Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
		foreach (var child in GetComponentsInChildren<Collider>())
		{
			Gizmos.DrawCube(child.bounds.center, child.bounds.size);
		}

		// Draw total bounds of all the children as a white box.
		Gizmos.color = new Color(1f, 1f, 1f, 0.1f);
		var maxBounds = GetMaxBoundsOfChilds(gameObject);
		Gizmos.DrawCube(maxBounds.center, maxBounds.size);
		Debug.Log("Total height is " + maxBounds.size.y);
	}
	*/

	/*
	private void OnDrawGizmos()
	{
		// Draw each child's bounds as a green box.
		Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
		foreach (var towerPart in towerParts)
		{
			var child = towerPart.GetComponent<Collider>();
			Gizmos.DrawCube(child.bounds.center, child.bounds.size);
		}

		// Draw total bounds of all the children as a white box.
		Gizmos.color = new Color(1f, 1f, 1f, 0.1f);
		var maxBounds = GetMaxBoundsOfChilds(gameObject);
		Gizmos.DrawCube(maxBounds.center, maxBounds.size);
		Debug.Log("Total height is " + maxBounds.size.y);
	}
	*/

	private Bounds GetMaxBoundsOfChilds(GameObject parent)
	{
		// thanks to IsaiahKelly https://answers.unity.com/questions/1404860/how-to-measure-the-height-of-3d-objects.html
		Bounds total = new Bounds(parent.transform.position, Vector3.zero);
		foreach (Collider childCollider in parent.GetComponentsInChildren<Collider>())
		{
			total.Encapsulate(childCollider.bounds);
		}

		return total;
	}
}
