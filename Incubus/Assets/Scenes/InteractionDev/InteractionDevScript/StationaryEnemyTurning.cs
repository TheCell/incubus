using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryEnemyTurning : MonoBehaviour
{
	[SerializeField] private float maxRotation = 100f;
	[SerializeField] private float scanRadius = 15f;
	private Quaternion targetRotation;
	private Rigidbody rigidBody;
	private Vector3 playerPosition;

	private void Start()
    {
		rigidBody = GetComponent<Rigidbody>();
		if (rigidBody == null)
		{
			Debug.LogError("missing Rigidbody");
		}

		targetRotation = transform.rotation;
    }

	private void Update()
	{
		UpdatePlayerPosition();
	}

	private void FixedUpdate()
    {
		TurnStepwiseTowards();
	}

	private void TurnStepwiseTowards()
	{
		//Vector3 targetPlaneDirection = Vector3.ProjectOnPlane(targetDirection, transform.up);
		/*
		float angle = Vector3.SignedAngle(transform.forward, targetDirection, transform.up);
		Vector3 newForwardDirection = transform.forward;
		if (Mathf.Abs(angle) < maxRotationAngle)
		{
			newForwardDirection.x = targetDirection.x;
		}

		transform.forward = newForwardDirection;
		*/

		/*
		targetRotation = Quaternion.FromToRotation(transform.position + transform.forward, playerPosition);
		//Debug.Log("current Rot: " + transform.rotation + " target rot: " + targetRotation);
		Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.fixedDeltaTime * maxRotation);
		rigidBody.MoveRotation(rotation);
		*/
		if (Vector3.Dot(transform.up, Vector3.up) > 0.4f)
		{
			Vector3 targetDirection = playerPosition - transform.position;
			Vector3 stationaryPlaneDirection = Vector3.ProjectOnPlane(targetDirection, transform.up);
			float step = Time.fixedDeltaTime * maxRotation;
			//Debug.Log(transform.forward + "   " + stationaryPlaneDirection);
			Vector3 newDirection = Vector3.RotateTowards(transform.forward, stationaryPlaneDirection, step, 0.0f);
			transform.rotation = Quaternion.LookRotation(newDirection);
		}
	}

	private void UpdatePlayerPosition()
	{
		playerPosition = SearchPlayer();
	}

	private Vector3 SearchPlayer()
	{
		Vector3 playerPosition = transform.position + transform.forward;
		GameObject player = GameObject.FindWithTag("Player");
		if (player != null)
		{
			float distance = Vector3.Distance(transform.position, player.transform.position);
			if (distance < scanRadius)
			{
				playerPosition = player.transform.position;
			}
		}

		return playerPosition;
	}
}
