using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshManipulation : MonoBehaviour
{
	[SerializeField] private LayerMask manipulatableLayers;
	[SerializeField] private PlayerCamera playerCamera;
	[SerializeField] private float manipulationSpeed = 0.8f;
	private PlayerController playerController;

	// Ray Logic
	private Vector3 rayStartPoint;
	private float maxRayDistance = 5f;
	private bool isManipulating;
	private GameObject manipulationBall;
	private RaycastHit rayCastHit;

	// inputs
	private float lockToPyramid, lockToArea, cameraVertical, cameraHorizontal, strecht, shrink;

	// Mesh infos
	Mesh targetMesh;
	MeshCollider meshCollider;
	Vector3[] meshVertices;
	int[] meshTriangles;

	// pyramidManipulation
	Vector3 targetPosition = Vector3.zero;


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
		// manipulationObject = new ManipulationObject();
		ResetObjectToManipulate();

		// setup debug ball
		manipulationBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		Destroy(manipulationBall.GetComponent<Collider>());
		manipulationBall.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
		manipulationBall.SetActive(false);
	}

	private void Update()
	{
		GetInput();
		CheckAndResetObjectToManipulate();
		RayCast();
		//ManipulateVertex();
	}

	private void FixedUpdate()
	{
		ManipulateVertex();
	}

	private void RayCast()
	{
		if (isManipulating && (Mathf.Abs(shrink) + Mathf.Abs(strecht) <= 0.3f))
		{
			isManipulating = !isManipulating;
		}

		if (!isManipulating && (Mathf.Abs(shrink) + Mathf.Abs(strecht) > 0.3f))
		{
			rayStartPoint = transform.position + playerCamera.LookAtPlayerOffset;
			Ray ray = new Ray(rayStartPoint, playerCamera.transform.forward);
			Physics.Raycast(ray, out rayCastHit, maxRayDistance, manipulatableLayers);
			if (rayCastHit.collider == null)
			{
				// player does not aim at the ground, try backup ray
				Vector3 directionToGround = playerCamera.GetFloorLevelDot.transform.position - (transform.position + playerCamera.LookAtPlayerOffset);
				ray = new Ray(rayStartPoint, directionToGround);
				Physics.Raycast(ray, out rayCastHit, maxRayDistance + 2f, manipulatableLayers);
			}

			if (rayCastHit.collider != null)
			{
				isManipulating = true;

				SetObjectToManipulate();

				// show debug sphere
				UpdateTargetPosition();
				manipulationBall.SetActive(true);
			}
		}
	}

	private void SetObjectToManipulate()
	{
		meshCollider = rayCastHit.collider as MeshCollider;
		if (meshCollider == null || meshCollider.sharedMesh == null)
		{
			Debug.LogError("no meshCollider or sharedMesh on collider");
			return;
		}

		targetPosition = rayCastHit.point;
		targetMesh = meshCollider.sharedMesh;
		meshVertices = targetMesh.vertices;
		meshTriangles = targetMesh.triangles;
	}

	private void ResetObjectToManipulate()
	{
		targetMesh = new Mesh();
	}

	private void CheckAndResetObjectToManipulate()
	{
		if (!isManipulating)
		{
			ResetObjectToManipulate();
		}
	}

	private void ManipulateVertex()
	{
		if (!isManipulating)
		{
			return;
		}

		int vertexIndexP0 = rayCastHit.triangleIndex * 3 + 0;
		int vertexIndexP1 = rayCastHit.triangleIndex * 3 + 1;
		int vertexIndexP2 = rayCastHit.triangleIndex * 3 + 2;
		int targetVertexIndex;

		Vector3 p0 = meshVertices[meshTriangles[vertexIndexP0]];
		Vector3 p1 = meshVertices[meshTriangles[vertexIndexP1]];
		Vector3 p2 = meshVertices[meshTriangles[vertexIndexP2]];
		Transform hitTransform = rayCastHit.collider.transform;
		p0 = hitTransform.TransformPoint(p0);
		p1 = hitTransform.TransformPoint(p1);
		p2 = hitTransform.TransformPoint(p2);

		float distanceToP0 = Vector3.Distance(targetPosition, p0);
		float distanceToP1 = Vector3.Distance(targetPosition, p1);
		float distanceToP2 = Vector3.Distance(targetPosition, p2);
		
		if (distanceToP0 < distanceToP1 && distanceToP0 < distanceToP2)
		{
			targetPosition = p0;
			targetVertexIndex = vertexIndexP0;
		}
		else if (distanceToP1 < distanceToP2)
		{
			targetPosition = p1;
			targetVertexIndex = vertexIndexP1;
		}
		else
		{
			targetPosition = p2;
			targetVertexIndex = vertexIndexP2;
		}

		DisplaceVertex(targetVertexIndex, (shrink + strecht));
		UpdateTargetPosition();

		Debug.DrawLine(p0, p1);
		Debug.DrawLine(p1, p2);
		Debug.DrawLine(p2, p0);
	}

	private void DisplaceVertex(int vertexIndex, float force)
	{
		Vector3 targetVertexPoint = meshVertices[meshTriangles[vertexIndex]];
		Vector3 vertexPoint = Vector3.zero;
		float sqrRadius = 0.2f * 0.2f;

		List<int> indices = new List<int>();
		Vector3 normal = Vector3.zero;

		for (int i = 0; i < meshTriangles.Length; i++)
		{
			vertexPoint = meshVertices[meshTriangles[i]];
			float sqrMagnitude = (vertexPoint - targetVertexPoint).sqrMagnitude;
			
			if (sqrMagnitude > sqrRadius)
			{
				// not the vertexpoints we are looking for
				continue;
			}

			indices.Add(i);
			normal += targetMesh.normals[meshTriangles[i]];
		}

		normal = normal.normalized;
		if (indices.Count > 0)
		{
			Vector3 newPosition = meshVertices[meshTriangles[indices[0]]] + ((normal * force) * Time.fixedDeltaTime);

			indices.ForEach(delegate (int index)
			{
				meshVertices[meshTriangles[index]] = newPosition;
			});
		}

		UpdateMeshData();
	}

	private void UpdateMeshData()
	{
		targetMesh.vertices = meshVertices;
		targetMesh.RecalculateBounds();
		targetMesh.RecalculateNormals();
		meshCollider.gameObject.SetActive(false);
		meshCollider.gameObject.SetActive(true);
	}

	private void UpdateTargetPosition()
	{
		if (isManipulating)
		{
			manipulationBall.transform.position = targetPosition;
		}
		else
		{
			manipulationBall.SetActive(false);
		}
	}

	private void GetInput()
	{
		lockToArea = Input.GetAxis("LockToArea");
		lockToPyramid = Input.GetAxis("LockToPyramid");
		strecht = Input.GetAxisRaw("StretchMeshTrigger");
		shrink = -1f * Input.GetAxisRaw("ShrinkMeshTrigger");

		// x and B button
		if (Input.GetButton("StretchMesh"))
		{
			strecht = 1f;
		}
		if ((Input.GetButton("ShrinkMesh")))
		{
			shrink = -1f;
		}

		shrink = manipulationSpeed * shrink;
		strecht = manipulationSpeed * strecht;
		cameraVertical = Input.GetAxis("CameraVertical");
		cameraHorizontal = Input.GetAxis("CameraHorizontal");
	}
}
