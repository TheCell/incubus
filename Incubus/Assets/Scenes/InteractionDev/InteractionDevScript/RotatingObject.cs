using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObject : MonoBehaviour
{
	[SerializeField] private Vector3 rotation = Vector3.zero;
	[SerializeField] private float speed = 2f;
	private GameObject gameObj;
	//private Quaternion definedOrientation;

	//private List<Rigidbody> collidingBodies = new List<Rigidbody>();
	//private Vector3 positionDelta = Vector3.zero;
	//private Vector3 deltaEulerSinceLastCheck = Vector3.zero;
	private Quaternion startRotation;
	private Rigidbody collidingBody;
	//private ContactPoint collisionContact;

	private void Start()
	{
		gameObj = gameObject;
		//definedOrientation = transform.rotation;
		//Debug.Log(gameObject.name + " " + definedOrientation.eulerAngles);
	}

	/*
	private class ContactInfos
	{
		private Vector3 startRotation = Vector3.zero;
		private Collision collisionInfo = new Collision();

		ContactInfos(Vector3 rotationAtCollision, Collision collision)
		{
			startRotation = rotationAtCollision;
			collisionInfo = collision;
		}
	}
	*/

	/*
	public Vector3 DeltaEulerSinceLastCheck
	{
		get 
			{
				Vector3 delta = deltaEulerSinceLastCheck;
				deltaEulerSinceLastCheck = Vector3.zero;
				return delta;
			}

		set => deltaEulerSinceLastCheck = value;
	}
	*/

	private void FixedUpdate()
	{
		UpdateCurrentPosition();
		UpdateContacts();
	}
	/*
	private void OnCollisionStay(Collision collision)
	{
		ContactPoint[] contacts = new ContactPoint[collision.contactCount];
		collision.GetContacts(contacts);
		Debug.Log(collision.contactCount);
	}
	*/
	
	private void OnTriggerEnter(Collider other)
	{
		//Debug.Log("enter");
		//Debug.Log(collision.GetContact(0).point);
		//Vector3 throwaway = DeltaEulerSinceLastCheck; // empty the value
		Debug.Log(transform.up);
		collidingBody = other.GetComponent<Rigidbody>();
		startRotation = gameObj.transform.rotation;
		//collisionContact = collision.GetContact(0);

		/*
		collision.GetContact(0);
		ContactPoint[] contacts = new ContactPoint[collision.contactCount];
		collision.GetContacts(contacts);

		foreach (ContactPoint contact in contacts)
		{
		}
		
		//Debug.Log("added object");
		collision.rigidbody.velocity = Vector3.zero;
		collidingBodies.Add(collision.rigidbody);
		*/
	}

	private void OnTriggerExit(Collider other)
	{
		//Debug.Log("collision exit");
		//Debug.Log(collision.GetContact(0).point);

		collidingBody = null;
		//collisionContact = new ContactPoint();

		/*
		//Debug.Log("removed object");
		GameObject collidingObject = collision.gameObject;
		PlayerController playerController = collidingObject.GetComponent<PlayerController>();
		if (playerController != null)
		{
			playerController.SetImpact(positionDelta);
		}
		collidingBodies.Remove(collision.rigidbody);
		*/
	}

	private void UpdateCurrentPosition()
	{
		Quaternion rotationDelta = Quaternion.Euler(rotation * speed * Time.deltaTime);
		gameObj.transform.rotation = gameObj.transform.rotation * rotationDelta;
		//DeltaEulerSinceLastCheck += rotationDelta.eulerAngles;
	}

	private void UpdateContacts()
	{
		if (collidingBody == null)
		{
			return;
		}

		//Vector3 angles = DeltaEulerSinceLastCheck;
		//Vector3 translation = GetVectorFromAngles(collidingBody.position, transform.position, angles);
		Quaternion rotationDelta = RotationDelta();
		//Debug.Log(rotationDelta.eulerAngles.y);
		Vector3 currentVector = collidingBody.position - gameObj.transform.position;
		Vector3 translation = rotationDelta * currentVector;
		//Debug.Log(transform.position + " " + collidingBody.position + " " + angles + " " + translation);
		collidingBody.MovePosition(gameObj.transform.position + translation);
		/*
		Vector3 pivotPoint = transform.position;
		Vector3 collidingPivot = collidingBody.position;
		float distanceToCenter = Vector3.Distance(pivotPoint, collidingPivot);
		Vector3 rotation = DeltaEulerSinceLastCheck;
		Vector3 moveVector = Mathf.PI * distanceToCenter * (rotation / 180f);
		Debug.Log(distanceToCenter + " rotation " + moveVector);
		collidingBody.MovePosition(collidingBody.position + moveVector);
		*/
		//Vector3 bodyContact = collisionContact.
	}

	/*
	private Vector3 GetVectorFromAngles(Vector3 startPosition, Vector3 pivotPoint, Quaternion deltaRotation)
	{
		Vector3 translationVector = Vector3.zero;
		Quaternion.RotateTowards(Quaternion.)
		return translationVector;
	}
	*/

	private Quaternion RotationDelta()
	{
		//Debug.Log(startRotation.eulerAngles + " " + gameObj.transform.rotation.eulerAngles);
		//Quaternion deltaRotation = Quaternion.RotateTowards(startRotation, transform.rotation, 0f);
		Quaternion deltaRotation = Quaternion.Inverse(startRotation) * gameObj.transform.localRotation;
		startRotation = gameObj.transform.localRotation;
		//Debug.Log(startRotation.eulerAngles.y + " " + transform.rotation.y);
		//Debug.Log(deltaRotation.eulerAngles.y);
		//Debug.Log(transform.rotation.eulerAngles.y);
		//Debug.Log(deltaRotation.eulerAngles.y);
		return deltaRotation;
	}
}
