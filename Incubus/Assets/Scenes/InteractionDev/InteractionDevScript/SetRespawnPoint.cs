using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRespawnPoint : MonoBehaviour
{
	[SerializeField] private Transform spawnPoint;
	[SerializeField] private RespawnEffect respawnEffect;

	private void OnTriggerEnter(Collider collider)
	{
		RespawnInfo respawnInfo = collider.gameObject.GetComponent<RespawnInfo>();

		if (respawnInfo != null)
		{
			respawnInfo.RespawnPosition = spawnPoint.position;
			respawnInfo.RespawnEffect = respawnEffect;
		}
	}
}
