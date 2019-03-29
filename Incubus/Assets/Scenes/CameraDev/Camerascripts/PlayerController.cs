using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[System.Serializable]
	public class MoveSettings : System.Object
	{
		// public float forwardVelocity = 12;
		// public float sidewardVelocity = 12;
		public float movementVelocity = 12;
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
	public GameObject visualContainer;

	// movement direction based on camera view
	[SerializeField] private Transform cameraTransform;
	private Vector3 movementForwardDirection;
	private Vector3 movementPlaneOrthogonal = Vector3.up;

	private Vector3 velocity = Vector3.zero;
	private Quaternion targetRotation;
	private Rigidbody rigidb;
	private SphereCollider sphereCollider;
	private float forwardInput, sidewardInput, jumpInput;
	private Vector3 impactToAdd = Vector3.zero;
	private float coyoteTime = 0.1f;
	private float lastGroundedTime;
	private RaycastHit groundHit;
	private Vector3 groundAngle;

	public Quaternion TargetRotation
	{
		get { return targetRotation; }
	}

	public void SetImpact(Vector3 impact)
	{
		impactToAdd += impact;
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

		if (GetComponent<SphereCollider>())
		{
			sphereCollider = GetComponent<SphereCollider>();
		}
		else
		{
			Debug.LogError("The Character needs a Collider");
		}

		if (cameraTransform != null)
		{
			movementForwardDirection = Vector3.ProjectOnPlane(cameraTransform.forward, movementPlaneOrthogonal);
		}
		else
		{
			Debug.LogError("player Controller has no camera Reference");
		}

		forwardInput = sidewardInput = jumpInput = 0;
		lastGroundedTime = Time.timeSinceLevelLoad;
	}

	private void Update()
	{
		GetInput();
		UpdateMovementPlane();
		UpdateMovementForward();
	}

	private void FixedUpdate()
	{
		Jump();
		Run();
		ApplyImpact();

		rigidb.velocity = transform.TransformDirection(velocity);
	}

	private bool IsGrounded()
	{
		bool isGrounded = IsGroundedWithoutCoyoteTime();
		if (isGrounded)
		{
			lastGroundedTime = Time.timeSinceLevelLoad;
		}
		else
		{
			if (lastGroundedTime + coyoteTime > Time.timeSinceLevelLoad)
			{
				isGrounded = true;
			}
		}
		return isGrounded;
	}

	private bool IsGroundedWithoutCoyoteTime()
	{
		Vector3 downwardOffset = new Vector3(0f, -0.20f, 0f);
		return Physics.CheckSphere(visualContainer.transform.position + downwardOffset, sphereCollider.radius - 0.15f, moveSettings.ground);
	}

	/*
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Vector3 downwardOffset = new Vector3(0f, -0.20f, 0f);
		Gizmos.DrawSphere(visualContainer.transform.position + downwardOffset, sphereCollider.radius - 0.15f);
	}
	*/
	
	private void GetInput()
	{
		forwardInput = Input.GetAxis(inputSettings.FORWARD_AXIS); // interpolated
		sidewardInput = Input.GetAxis(inputSettings.TURN_AXIS); // interpolated
		jumpInput = Input.GetAxisRaw(inputSettings.JUMP_AXIS); // non-interpolated
	}

	private void UpdateMovementForward()
	{
		movementForwardDirection = Vector3.ProjectOnPlane(cameraTransform.forward, movementPlaneOrthogonal).normalized;
	}

	private void Run()
	{
		// allow for strafing, more speed if moving forward and sidewards
		float downAcceleration = velocity.y;
		Vector3 combinedDirection = Vector3.zero;

		if (Mathf.Abs(forwardInput) > inputSettings.inputThreshold)
		{
			combinedDirection = forwardInput * movementForwardDirection;
			visualContainer.transform.Rotate(cameraTransform.right, forwardInput * 10f, Space.World);
		}

		if (Mathf.Abs(sidewardInput) > inputSettings.inputThreshold)
		{
			Vector3 movementSidewardDirection = Quaternion.AngleAxis(90, movementPlaneOrthogonal) * movementForwardDirection;
			combinedDirection = combinedDirection + sidewardInput * movementSidewardDirection;
			visualContainer.transform.Rotate(cameraTransform.forward, sidewardInput * -10f, Space.World);
		}

		Vector3 newVelocity = combinedDirection * moveSettings.movementVelocity;
		newVelocity.y = newVelocity.y + downAcceleration;
		velocity = newVelocity;
		Debug.DrawRay(transform.position, movementForwardDirection, Color.red);
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

	private void ApplyImpact()
	{
		velocity += impactToAdd;
		impactToAdd = Vector3.zero;
	}

	private void UpdateMovementPlane()
	{
		if (!IsGroundedWithoutCoyoteTime()) // don't use IsGrounded, this needs to be instant
		{
			movementPlaneOrthogonal = Vector3.up;
		}
		else
		{
			UpdateMovementPlaneFromAngle();
		}
	}

	private void UpdateMovementPlaneFromAngle()
	{
		UpdateGroundHit();

		if (groundHit.collider != null)
		{
			Vector3 groundNormal = groundHit.normal;
			groundAngle = Vector3.Cross(transform.right, groundNormal);
			movementPlaneOrthogonal = groundNormal;
		}
		else
		{
			Debug.Log("collider is null");
			movementPlaneOrthogonal = Vector3.up;
		}
	}

	private void UpdateGroundHit()
	{
		// I don't know why I have to inverse the layerMask
		bool test = Physics.Raycast(transform.position, Vector3.down * 2f, out groundHit, moveSettings.ground);
	}
}
