using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [System.Serializable]
    public class InputSettings : System.Object
    {
        public float inputThreshold = 0.1f;
        public string VERTICAL_AXIS = "CameraVertical";
        public string HORIZONTAL_AXIS = "CameraHorizontal";
		/*
        public string LOCK_TARGET_PYRAMID = "LockToPyramid";
        public string LOCK_TARGET_AREA = "LockToArea";
		*/
    }

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 lookAtPlayerOffset = new Vector3(0f, 1.2f, 0f);
	[SerializeField] private bool exponentialCameraTurn = true;
	[SerializeField] private int exponentialMultiplier = 3;

    private PlayerController playerController;
    private float cameraHorizontal, cameraVertical;
    // private float lockPyramid, lockArea;
    private InputSettings inputSettings;
    private Vector3 cameraPlayerOffset;
	private GameObject floorLevelDot;

	public Vector3 LookAtPlayerOffset
	{
		get { return lookAtPlayerOffset; }
	}

	public GameObject GetFloorLevelDot
	{
		get { return floorLevelDot; }
	}
	
	private void Start()
    {
        SetCameraTarget(target);

        inputSettings = new InputSettings();
        cameraPlayerOffset = transform.position - playerController.transform.position;

		floorLevelDot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		Destroy(floorLevelDot.GetComponent<Collider>());
		floorLevelDot.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
		floorLevelDot.GetComponent<MeshRenderer>().enabled = false;
    }

    private void Update()
    {
        GetInput();
    }

    private void LateUpdate()
    {
        RotateCamera();
        UpdateCameraPosition();
		UpdateFloorLevelDot();
	}

    private void RotateCamera()
    {
        Quaternion cameraRotationXDelta = Quaternion.AngleAxis(CameraSpeed(cameraHorizontal), Vector3.up);
        Quaternion cameraRotationYDelta = Quaternion.AngleAxis(CameraSpeed(cameraVertical), transform.right);

        // this does not work how I wish it would after rotating around a bit
        cameraPlayerOffset = cameraRotationYDelta * cameraRotationXDelta * cameraPlayerOffset;

    }

    private void UpdateCameraPosition()
    {
        // prevent camera from clipping through floor
        Vector3 newPos = playerController.transform.position + cameraPlayerOffset;
        newPos.y = newPos.y < playerController.transform.position.y ? newPos.y = playerController.transform.position.y : newPos.y;
        transform.position = newPos;
        transform.LookAt(target.transform.position + lookAtPlayerOffset);
    }

    private void GetInput()
    {
        cameraHorizontal = Input.GetAxis(inputSettings.HORIZONTAL_AXIS); // interpolated
        cameraVertical = Input.GetAxis(inputSettings.VERTICAL_AXIS); // interpolated

		/*
        lockPyramid = Input.GetAxis(inputSettings.LOCK_TARGET_PYRAMID); // interpolated
        lockArea = Input.GetAxis(inputSettings.LOCK_TARGET_AREA); // interpolated
		*/
    }

    private void SetCameraTarget(Transform targetTransform)
    {
        target = targetTransform;

        if (target != null)
        {
            if (target.GetComponent<PlayerController>())
            {
                playerController = target.GetComponent<PlayerController>();
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

	private void UpdateFloorLevelDot()
	{
		floorLevelDot.transform.position = target.transform.position - new Vector3(0.5f, 0.5f, 0.5f);
		floorLevelDot.transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
		floorLevelDot.transform.position = floorLevelDot.transform.position + floorLevelDot.transform.forward * 3f;
	}

	private float CameraSpeed(float direction)
	{
		float cameraSpeed = 1;

		if (exponentialCameraTurn)
		{
			cameraSpeed = cameraSpeed * Mathf.LerpUnclamped(0, Mathf.Pow(2, 2), direction);
		}
		else
		{
			cameraSpeed = cameraSpeed * direction;
		}

		return cameraSpeed;
	}
}