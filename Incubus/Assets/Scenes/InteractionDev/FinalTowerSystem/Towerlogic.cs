using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Towerlogic : MonoBehaviour
{
	[SerializeField] private List<GameObject> towerParts;

	private void Start()
	{
		towerParts.Reverse();
	}

	public void RemovePart()
	{
		IEnumerator<GameObject> partEnumerator = towerParts.GetEnumerator();
		if (partEnumerator.MoveNext())
		{
			GameObject element = partEnumerator.Current;
			towerParts.Remove(element);
			Destroy(element);
		}
	}
}
