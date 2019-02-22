﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[System.Serializable]
	public class MoveSettings : System.Object
	{
		public float forwardVelocity = 12;
		public float sidewardVelocity = 12;
		public float rotationVelocity = 100;
		public float jumpVelocity = 25;
		public float distToGrounded = 0.78f;
		public LayerMask ground;
	}

	[System.Serializable]
	public class PhysicsSettings : System.Object
	{
		public float downAcceleration = 0.75f;
	}

	[System.Serializable]
	public class InputSettings : System.Object
	{
		public float inputThreshold = 0.1f;
		public string FORWARD_AXIS = "Vertical";
		public string TURN_AXIS = "Horizontal";
		public string JUMP_AXIS = "Jump";
	}


	[SerializeField] private MoveSettings moveSettings = new MoveSettings();
	[SerializeField] private PhysicsSettings physicsSettings = new PhysicsSettings();
	[SerializeField] private InputSettings inputSettings = new InputSettings();

	private Vector3 velocity = Vector3.zero;
	private Quaternion targetRotation;
	private Rigidbody rigidb;
	private float forwardInput, sidewardInput, jumpInput;

	public Quaternion TargetRotation
	{
		get { return targetRotation; }
	}

	private bool IsGrounded()
	{
		return Physics.Raycast(transform.position, Vector3.down, moveSettings.distToGrounded, moveSettings.ground);
	}

	private void Start()
	{
		targetRotation = transform.rotation;
		if (GetComponent<Rigidbody>())
		{
			rigidb = GetComponent<Rigidbody>();
		}
		else
		{
			Debug.LogError("The Character needs a rigidbody");
		}

		forwardInput = sidewardInput = jumpInput = 0;
	}

	private void GetInput()
	{
		forwardInput = Input.GetAxis(inputSettings.FORWARD_AXIS); // interpolated
		sidewardInput = Input.GetAxis(inputSettings.TURN_AXIS); // interpolated
		jumpInput = Input.GetAxisRaw(inputSettings.JUMP_AXIS); // non-interpolated
	}

	private void Update()
	{
		GetInput();
	}

	private void FixedUpdate()
	{
		Run();
		Jump();

		rigidb.velocity = transform.TransformDirection(velocity);
	}

	private void Run()
	{
		if (Mathf.Abs(forwardInput) > inputSettings.inputThreshold)
		{
			// move
			velocity.z = moveSettings.forwardVelocity * forwardInput;
		}
		else
		{
			// zero velocity
			velocity.z = 0;
		}

		if (Mathf.Abs(sidewardInput) > inputSettings.inputThreshold)
		{
			// move
			velocity.x = moveSettings.sidewardVelocity * sidewardInput;
		}
		else
		{
			// zero velocity
			velocity.x = 0;
		}
	}

	private void Jump()
	{
		if (IsGrounded())
		{
			if (jumpInput > 0)
			{
				velocity.y = moveSettings.jumpVelocity;
			}
			else if (jumpInput == 0)
			{
				velocity.y = 0;
			}
		}
		else
		{
			// decrease velocity.y
			velocity.y -= physicsSettings.downAcceleration;
		}
	}
}
