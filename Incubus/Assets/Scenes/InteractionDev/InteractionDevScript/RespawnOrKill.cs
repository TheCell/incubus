using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnOrKill : MonoBehaviour
{
	public static void Respawn(GameObject objectToRespawn, RespawnInfo respawnInfo)
	{
		objectToRespawn.transform.position = respawnInfo.RespawnPosition;
		Rigidbody rb = objectToRespawn.GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.velocity = Vector3.zero;
		}

		respawnInfo.RespawnEffect.PlayOnce();
        Achievement_Manager.Set_RESPAWN();
	}

	private void OnTriggerEnter(Collider collider)
	{
		GameObject collisionRootObject = collider.transform.root.gameObject;
		GameObject collisionObject = collider.gameObject;
		RespawnInfo respawnInfo = collider.GetComponent<RespawnInfo>();

		if (respawnInfo == null)
		{
			Debug.LogWarning("Object has no respawn information. KILL IT");
			KillObject(collisionRootObject);
			return;
		}

		if (!respawnInfo.respawnOnTrigger)
		{
			KillObject(collisionRootObject);
		}
		else
		{
			Respawn(collisionObject, respawnInfo);
		}
	}

	private void KillObject(GameObject gameObject)
	{
		Destroy(gameObject);
	}
}
