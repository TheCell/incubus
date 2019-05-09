using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnInfo : MonoBehaviour
{
	public bool respawnOnTrigger = true;
	private Vector3 respawnPosition = Vector3.zero;
	private List<Vector3> respawnHistory = new List<Vector3>();
	private RespawnEffect respawnEffect;

	public Vector3 RespawnPosition
	{
		get { return respawnPosition; }
		set
		{
			respawnHistory.Add(respawnPosition);
			respawnPosition = value;
		}
	}

	public RespawnEffect RespawnEffect
	{
		get
		{
			if (respawnEffect == null)
			{
				respawnEffect = new RespawnEffect();
			}
			return respawnEffect;
		}
		set { respawnEffect = value; }
	}

	private void Start()
    {
		RespawnPosition = transform.position;
    }

	private void Update()
	{
		if (Input.GetButton("ResetToCheckpoint"))
		{
			ManualRespawn();
		}
	}

	private void ManualRespawn()
	{
		transform.position = RespawnPosition;
		Rigidbody rb = GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.velocity = Vector3.zero;
		}

		RespawnEffect.PlayOnce();
	}
}
