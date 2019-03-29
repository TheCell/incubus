using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnInfo : MonoBehaviour
{
	public bool respawnOnTrigger = true;
	private Vector3 respawnPosition = Vector3.zero;
	private List<Vector3> respawnHistory = new List<Vector3>();

	public Vector3 RespawnPosition
	{
		get { return respawnPosition; }
		set
		{
			respawnHistory.Add(respawnPosition);
			respawnPosition = value;
		}
	}

	private void Start()
    {
		RespawnPosition = transform.position;
    }

	private void OnCollisionEnter(Collision collision)
	{
		
	}
}
