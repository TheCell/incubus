using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Renaissance Coders Tutorial helped here
public class CameraController : MonoBehaviour
{
	[SerializeField] private Transform target;
	[SerializeField] private float lookSmooth = 0.09f;
	[SerializeField] private Vector3 offsetFromTarget = new Vector3(0, 2, -8);
	[SerializeField] private float xTilt = 10;

	private Vector3 destination = Vector3.zero;
	private PlayerCharacterController playerCharacterController;
	private float rotateVelocity = 0;

	private void Start()
    {
		SetCameraTarget(target);
    }

	private void SetCameraTarget(Transform targetTransform)
	{
		target = targetTransform;

		if (target != null)
		{
			if (target.GetComponent<PlayerCharacterController>())
			{
				playerCharacterController = target.GetComponent<PlayerCharacterController>();
			}
			else
			{
				Debug.LogError("The camera's target needs a player character controller.");
			}
		}
		else
		{
			Debug.LogError("Your camera needs a target");
		}
	}

	private void LateUpdate()
	{
		// moving
		MoveToTarget();
		// rotate
		LookAtTarget();
	}

	private void MoveToTarget()
	{
		destination = playerCharacterController.TargetRotation * offsetFromTarget;
		destination += target.position;
		transform.position = destination;
	}

	private void LookAtTarget()
	{
		float eulerYAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, target.eulerAngles.y, ref rotateVelocity, lookSmooth);
		//float eulerXAngle = transform.eulerAngles.x;
		float eulerXAngle = xTilt;

		transform.rotation = Quaternion.Euler(eulerXAngle, eulerYAngle, 0.0f);
	}
}
