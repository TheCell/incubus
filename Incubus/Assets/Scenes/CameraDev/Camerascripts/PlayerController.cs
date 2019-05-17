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
		public bool isActive = true;
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

	private bool justJumped = false;

	//private Vector3 translationDeltaSinceLastCheck = Vector3.zero;
	private Vector3 positionAtLastCheck = Vector3.zero;
	
	private enum Surface
	{
		Normal = 0,
		Slippery = 1
	}
	private Surface surfaceCondition;

	public Quaternion TargetRotation
	{
		get { return targetRotation; }
	}

	public Vector3 TranslationDeltaSinceLastCheck
	{
		get
		{
			/*
			Vector3 delta = translationDeltaSinceLastCheck;
			translationDeltaSinceLastCheck = Vector3.zero;
			*/
			Vector3 currentPosition = transform.position;
			Vector3 positionDelta = currentPosition - positionAtLastCheck;
			positionAtLastCheck = transform.position;
			return positionDelta;
		}
	}

	public void SetImpact(Vector3 impact)
	{
		impactToAdd += impact;
	}

	public void SetInputActive(bool inputState)
	{
		inputSettings.isActive = inputState;
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

		surfaceCondition = Surface.Normal;
		positionAtLastCheck = transform.position;
	}

	private void Update()
	{
		GetInput();
	}

	private void FixedUpdate()
	{
		UpdateMovementPlane();
		UpdateMovementForward();

		UpdateYAcceleration();
		Jump();
		Run();

		//rigidb.velocity = transform.TransformDirection(velocity);
		ApplyImpact();
		rigidb.velocity = velocity;
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
		if (inputSettings.isActive)
		{
			forwardInput = Input.GetAxis(inputSettings.FORWARD_AXIS); // interpolated
			sidewardInput = Input.GetAxis(inputSettings.TURN_AXIS); // interpolated
			jumpInput = Input.GetAxisRaw(inputSettings.JUMP_AXIS); // non-interpolated
		}
		else
		{
			forwardInput = 0f;
			sidewardInput = 0f;
			jumpInput = 0f;
		}
	}

	private void UpdateMovementForward()
	{
		movementForwardDirection = Vector3.ProjectOnPlane(cameraTransform.forward, movementPlaneOrthogonal).normalized;
	}

	private void Run()
	{
		// allow for strafing, more speed if moving forward and sidewards
		float currentYVelocity = velocity.y;
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
		newVelocity.y = newVelocity.y + currentYVelocity;

		velocity = newVelocity;
		Debug.DrawRay(transform.position, velocity, Color.blue);
		Debug.DrawRay(transform.position, movementForwardDirection, Color.red);
	}

	private void Jump()
	{
		if (IsGrounded() && jumpInput > 0 && !justJumped)
		{
			velocity.y = moveSettings.jumpVelocity;
			justJumped = true;
		}
	}

	private void UpdateYAcceleration()
	{
		if (IsGrounded())
		{
			justJumped = false;
			velocity.y = 0f;
		}
		else if (IsGroundedWithoutCoyoteTime())
		{
			velocity.y = 0f;
		}
		else
		{
			justJumped = false;
			velocity.y -= physicsSettings.downAcceleration;
			if (velocity.y < -300f)
			{
				velocity.y = -300f;
			}
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

		Debug.DrawRay(transform.position, movementPlaneOrthogonal, Color.black);
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
		bool test = Physics.Raycast(transform.position, Vector3.down * 2f, out groundHit, moveSettings.ground);
	}

	private Surface GetSurfaceType(string surfaceName)
	{
		Surface surfaceType = Surface.Normal;
		switch (surfaceName)
		{
			case "Normal":
				surfaceType = Surface.Normal;
				break;
			case "Slippery":
				surfaceType = Surface.Slippery;
				break;
			default:
				surfaceType = Surface.Normal;
				break;
		}
		return surfaceType;
	}

	private bool IsSlippery()
	{
		bool isSlippery = false;
		if (groundHit.collider != null)
		{
			isSlippery = GetSurfaceType(groundHit.transform.tag) == Surface.Slippery;
		}
		return isSlippery;
	}
}
