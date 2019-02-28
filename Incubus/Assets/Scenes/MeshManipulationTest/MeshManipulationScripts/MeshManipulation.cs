using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshManipulation : MonoBehaviour
{
	[System.Serializable]
	public class ManipulationObject : System.Object
	{
		public Mesh mesh;
		public MeshFilter filter;
		public int triangleIndex;
		public int[] suroundingVertices;
		public Vector3[] vertices;
		public Vector3[] normals;
	}

	[SerializeField] private LayerMask manipulatableLayers;
	[SerializeField] private PlayerCamera playerCamera;
	private PlayerController playerController;

	// Ray Logic
	private Vector3 rayStartPoint;
	private float maxRayDistance = 5f;
	private bool isManipulating;
	private GameObject manipulationBall;
	private RaycastHit rayCastHit;

	// inputs
	private float lockToPyramid, lockToArea, cameraVertical, cameraHorizontal;

	// pyramidManipulation
	public ManipulationObject manipulationObject;
	private GameObject objectForManipulation;


	private void Start()
	{
		if (GetComponent<PlayerController>() != null)
		{
			playerController = GetComponent<PlayerController>();
		}
		else
		{
			Debug.LogError("Missing PlayerController script");
		}

		rayCastHit = new RaycastHit();
		manipulationObject = new ManipulationObject();

		// setup debug ball
		manipulationBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		Destroy(manipulationBall.GetComponent<Collider>());
		manipulationBall.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
		manipulationBall.SetActive(false);
	}

	private void Update()
	{
		GetInput();
		RayCast();
		ManipulateVertex();
	}

	private void FixedUpdate()
	{
	}

	private void RayCast()
	{
		if (isManipulating && lockToPyramid <= 0.3f)
		{
			isManipulating = !isManipulating;
			ResetObjectToManipulate();
		}

		if (!isManipulating && lockToPyramid > 0.3f)
		{
			rayStartPoint = transform.position + playerCamera.LookAtPlayerOffset;
			Ray ray = new Ray(rayStartPoint, playerCamera.transform.forward);
			Physics.Raycast(ray, out rayCastHit, maxRayDistance, manipulatableLayers);
			Debug.DrawRay(rayStartPoint, playerCamera.transform.forward);
			Debug.Log(rayCastHit.collider);

			if (rayCastHit.collider != null)
			{
				isManipulating = true;

				SetObjectToManipulate();

				// show debug sphere
				manipulationBall.SetActive(true);
				manipulationBall.transform.position = rayCastHit.point;
			}
		}
	}

	private void SetObjectToManipulate()
	{
		objectForManipulation = rayCastHit.collider.gameObject;

		manipulationObject.triangleIndex = rayCastHit.triangleIndex;
		manipulationObject.filter = objectForManipulation.GetComponent<MeshFilter>();
		MeshCollider meshCollider = rayCastHit.collider as MeshCollider;
		if (meshCollider == null || meshCollider.sharedMesh == null)
		{
			Debug.LogError("no meshCollider or sharedMesh on collider");
			return;
		}
		manipulationObject.mesh = meshCollider.sharedMesh;
		manipulationObject.vertices = manipulationObject.mesh.vertices;
		manipulationObject.normals = manipulationObject.mesh.normals;
		/*
		manipulationObject.suroundingVertices = new int[3];
		int firstVertexIndex = manipulationObject.triangleIndex * 3;
		manipulationObject.suroundingVertices[0] = firstVertexIndex;
		manipulationObject.suroundingVertices[1] = firstVertexIndex + 1;
		manipulationObject.suroundingVertices[2] = firstVertexIndex + 2;
		*/
	}

	private void ResetObjectToManipulate()
	{
		objectForManipulation = null;
		manipulationObject = new ManipulationObject();
	}

	private void ManipulateVertex()
	{
		if (!isManipulating)
		{
			objectForManipulation = null;
			return;
		}

		/*
		int vertexIndex = manipulationObject.suroundingVertices[0];
		Vector3 vertexNormal = manipulationObject.normals[manipulationObject.triangleIndex];
		manipulationObject.vertices[vertexIndex] = manipulationObject.vertices[vertexIndex] * (1.0f + cameraVertical);

		manipulationObject.mesh.RecalculateNormals();
		*/
	}

	private void GetInput()
	{
		lockToArea = Input.GetAxis("LockToArea");
		lockToPyramid = Input.GetAxis("LockToPyramid");
		cameraVertical = Input.GetAxis("CameraVertical");
		cameraHorizontal = Input.GetAxis("CameraHorizontal");
	}
}
