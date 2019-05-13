using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform2 : MonoBehaviour
{
	[SerializeField] private Transform startPoint;
	[SerializeField] private Transform endPoint;
	[SerializeField] private float durationSeconds = 2f;
	[SerializeField] private bool waitForPlayer = true;
	[SerializeField] private AnimationCurve elevatorSpeed;

	private bool reverseDirection = false;
	private float timeSincePlatformStart;
	private float currentLerp = 0f;
	private List<Rigidbody> collidingBodies = new List<Rigidbody>();
	private Vector3 positionDelta = Vector3.zero;
	private bool finishVisited = false;
	private bool platformRunning = false;
	private bool somethingOnPlatform = false;

	private void Start()
    {
		CheckAndStartPlatform();
	}

	private void Update()
	{
		if (collidingBodies.Count > 0)
		{
			somethingOnPlatform = true;
		}
		else
		{
			somethingOnPlatform = false;
		}
		CheckAndStartPlatform();
		//UpdateCurrentPosition();
	}

	private void FixedUpdate()
	{
		UpdateCurrentPosition();
	}

	private void OnCollisionEnter(Collision collision)
	{
		//Debug.Log("added object");
		collision.rigidbody.velocity = Vector3.zero;
		collidingBodies.Add(collision.rigidbody);
	}

	private void OnCollisionExit(Collision collision)
	{
		//Debug.Log("removed object");
		GameObject collidingObject = collision.gameObject;
		PlayerController playerController = collidingObject.GetComponent<PlayerController>();
		if (playerController != null)
		{
			playerController.SetImpact(positionDelta);
		}
		collidingBodies.Remove(collision.rigidbody);
	}

	private void CheckAndStartPlatform()
	{
		if (platformRunning)
		{
			return;
		}

		if (waitForPlayer)
		{
			if (somethingOnPlatform)
			{
				platformRunning = true;
				timeSincePlatformStart = Time.timeSinceLevelLoad;
			}
			else
			{
				// just update starttime
				timeSincePlatformStart = Time.timeSinceLevelLoad;
			}
		}
		else
		{
			platformRunning = true;
			timeSincePlatformStart = Time.timeSinceLevelLoad;
		}
	}

	private void UpdateCurrentPosition()
	{
		UpdateDirectionAndTime();
		currentLerp = GetCurrentLerpvalue();

		Vector3 newPosition = Vector3.LerpUnclamped(startPoint.transform.position, endPoint.transform.position, elevatorSpeed.Evaluate(currentLerp));
		positionDelta = newPosition - transform.position;
		transform.position = transform.position + positionDelta;

		foreach (Rigidbody rb in collidingBodies)
		{
			//rb.transform.position = rb.transform.position + positionDelta;
			rb.MovePosition(rb.transform.position + positionDelta);
		}
	}

	private void UpdateDirectionAndTime()
	{
		if (!platformRunning)
		{
			timeSincePlatformStart = Time.timeSinceLevelLoad;
		}
		else if (timeSincePlatformStart + durationSeconds < Time.timeSinceLevelLoad)
		{
			timeSincePlatformStart = Time.timeSinceLevelLoad;
			reverseDirection = !reverseDirection;
			finishVisited = !finishVisited;
			if (!finishVisited)
			{
				platformRunning = false;
			}
		}
	}

	private float GetRelativeTime()
	{
		return Time.timeSinceLevelLoad - timeSincePlatformStart;
	}

	private float GetCurrentLerpvalue()
	{
		float relativeTime = GetRelativeTime();
		float positionZeroToOne;

		if (reverseDirection)
		{
			positionZeroToOne = 1 - (1 / durationSeconds * relativeTime);
		}
		else
		{
			positionZeroToOne = 1 / durationSeconds * relativeTime;
		}

		return positionZeroToOne;
	}
}
