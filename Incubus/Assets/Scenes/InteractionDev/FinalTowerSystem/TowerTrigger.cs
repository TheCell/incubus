using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTrigger : MonoBehaviour
{
	[SerializeField] private Towerlogic towerScript;
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
			towerScript.RemovePart();
		}
	}
}
