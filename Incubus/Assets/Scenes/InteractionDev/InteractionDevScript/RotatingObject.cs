using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObject : MonoBehaviour
{
	[SerializeField] private Vector3 rotation = Vector3.zero;
	[SerializeField] private float speed = 2f;
	private GameObject gameObj;
	private Quaternion startRotation;
	private Rigidbody collidingBody;

	private void Start()
	{
		gameObj = gameObject;
	}
	
	private void FixedUpdate()
	{
		UpdateCurrentPosition();
		UpdateContacts();
	}

	private void OnTriggerEnter(Collider other)
	{
		collidingBody = other.GetComponent<Rigidbody>();
		startRotation = gameObj.transform.rotation;
	}

	private void OnTriggerExit(Collider other)
	{
		collidingBody = null;
	}

	private void UpdateCurrentPosition()
	{
		Quaternion rotationDelta = Quaternion.Euler(rotation * speed * Time.deltaTime);
		gameObj.transform.rotation = gameObj.transform.rotation * rotationDelta;
	}

	private void UpdateContacts()
	{
		if (collidingBody == null)
		{
			return;
		}
		
		Quaternion rotationDelta = RotationDelta();
		Vector3 currentVector = collidingBody.position - gameObj.transform.position;
		Vector3 translation = rotationDelta * currentVector;
		collidingBody.MovePosition(gameObj.transform.position + translation);
	}

	private Quaternion RotationDelta()
	{
		Quaternion deltaRotation = gameObj.transform.localRotation * Quaternion.Inverse(startRotation);
		startRotation = gameObj.transform.localRotation;
		return deltaRotation;
	}
}
