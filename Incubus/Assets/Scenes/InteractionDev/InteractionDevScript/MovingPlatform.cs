using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
	[SerializeField] private GameObject startPoint;
	[SerializeField] private GameObject endPoint;
	[SerializeField] private float durationSeconds = 2f;
	[SerializeField] private bool inverseDirection = false;


    private Rigidbody platformRB;
	private bool reverseDirection = false;
	private float timeSincePlatformStart;
	private float currentPosition = 0f;
	private List<Rigidbody> collidingBodies = new List<Rigidbody>();
	private Vector3 positionDelta = Vector3.zero;

	private void Start()
	{
		platformRB = GetComponent<Rigidbody>();

		if (platformRB == null)
		{
			Debug.LogError("Platform needs a Rigidbody");
		}

		reverseDirection = inverseDirection;
		timeSincePlatformStart = Time.time;
	}

	private void FixedUpdate()
	{
		UpdateCurrentPosition();
	}

	private void OnCollisionEnter(Collision collision)
	{
		collidingBodies.Add(collision.rigidbody);
	}

	private void OnCollisionExit(Collision collision)
	{
		collidingBodies.Remove(collision.rigidbody);
	}

	private void UpdateCurrentPosition()
	{
		UpdateDirectionAndTime();
		float relativeTime = Time.time - timeSincePlatformStart;

		if (reverseDirection)
		{
			currentPosition = 1 - (1 / durationSeconds * relativeTime);
		}
		else
		{
			currentPosition = 1 / durationSeconds * relativeTime;
		}

		Vector3 newPosition = Vector3.Lerp(startPoint.transform.position, endPoint.transform.position, currentPosition);
		positionDelta = newPosition - transform.position;
		platformRB.MovePosition(newPosition);

		foreach(Rigidbody rb in collidingBodies)
		{
			rb.MovePosition(rb.position + positionDelta);
		}
	}

	private void UpdateDirectionAndTime()
	{
		if (timeSincePlatformStart + durationSeconds < Time.time)
		{
			timeSincePlatformStart = Time.time;
			reverseDirection = !reverseDirection;
		}
	}
}
