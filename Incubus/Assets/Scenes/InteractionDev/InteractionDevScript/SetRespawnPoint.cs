using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRespawnPoint : MonoBehaviour
{
	[SerializeField] private Transform spawnPoint;

	private void OnTriggerEnter(Collider collider)
	{
		RespawnInfo respawnInfo = collider.gameObject.GetComponent<RespawnInfo>();

		if (respawnInfo != null)
		{
			respawnInfo.RespawnPosition = spawnPoint.position;
		}
	}
}
