using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshManipulation : MonoBehaviour
{
	[SerializeField] private LayerMask manipulatableLayers;
	[SerializeField] private PlayerCamera playerCamera;
	[SerializeField] private float manipulationSpeed = 0.8f;
	[SerializeField] private float displacementSpeed = 5f;
	private PlayerController playerController;

	enum ManipulationModes
	{
		Pyramid = 1,
		Mesh = 2
	}

	// Ray Logic
	private Vector3 rayStartPoint;
	private float maxRayDistance = 5f;
	private bool isManipulating;
	private GameObject manipulationBall;
	private RaycastHit rayCastHit;

	// inputs
	private float strecht, shrink;

	// Mesh infos
	private Mesh targetMesh;
	private MeshCollider meshCollider;
	private MeshFilter meshFilter;
	private Vector3[] meshVertices;
	private int[] meshTriangles;
	private Vector3 displacementNormal = Vector3.zero;

	// pyramidManipulation
	Vector3 targetPosition = Vector3.zero;
	private ManipulationModes manipulationMode = ManipulationModes.Pyramid;
	[SerializeField] private float brushSize = 0.8f;

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
		meshFilter = rayCastHit.collider.gameObject.GetComponent<MeshFilter>();

		if (meshCollider == null || meshCollider.sharedMesh == null)
		{
			Debug.LogError("no meshCollider or sharedMesh on collider");
			return;
		}

		if (meshFilter == null && manipulationMode == ManipulationModes.Mesh)
		{
			Debug.LogError("no meshFilter found");
			return;
		}

		targetPosition = rayCastHit.point;
		targetMesh = meshCollider.sharedMesh;
		meshVertices = targetMesh.vertices;
		meshTriangles = targetMesh.triangles;
		displacementNormal = Vector3.zero;
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

		int triangleVertexIndexP0 = rayCastHit.triangleIndex * 3 + 0;
		int triangleVertexIndexP1 = rayCastHit.triangleIndex * 3 + 1;
		int triangleVertexIndexP2 = rayCastHit.triangleIndex * 3 + 2;
		int vertexIndexP0 = meshTriangles[triangleVertexIndexP0];
		int vertexIndexP1 = meshTriangles[triangleVertexIndexP1];
		int vertexIndexP2 = meshTriangles[triangleVertexIndexP2];
		int targetTriangleIndex;
		int targetVertexIndex;

		Vector3 p0 = meshVertices[vertexIndexP0];
		Vector3 p1 = meshVertices[vertexIndexP1];
		Vector3 p2 = meshVertices[vertexIndexP2];
		Transform hitTransform = rayCastHit.collider.transform;
		// Transforms position from local space to worldspace
		p0 = hitTransform.TransformPoint(p0);
		p1 = hitTransform.TransformPoint(p1);
		p2 = hitTransform.TransformPoint(p2);

		float distanceToP0 = Vector3.Distance(targetPosition, p0);
		float distanceToP1 = Vector3.Distance(targetPosition, p1);
		float distanceToP2 = Vector3.Distance(targetPosition, p2);

		if (distanceToP0 < distanceToP1 && distanceToP0 < distanceToP2)
		{
			targetPosition = p0;
			targetTriangleIndex = triangleVertexIndexP0;
			targetVertexIndex = meshTriangles[targetTriangleIndex];
		}
		else if (distanceToP1 < distanceToP2)
		{
			targetPosition = p1;
			targetTriangleIndex = triangleVertexIndexP1;
			targetVertexIndex = meshTriangles[targetTriangleIndex];
		}
		else
		{
			targetPosition = p2;
			targetTriangleIndex = triangleVertexIndexP2;
			targetVertexIndex = meshTriangles[targetTriangleIndex];
		}

		float forceOnMesh = (shrink + strecht) / ((hitTransform.localScale.x + hitTransform.localScale.y + hitTransform.localScale.z) / 3);
		forceOnMesh = displacementSpeed * forceOnMesh;
		if (manipulationMode == ManipulationModes.Pyramid)
		{

			DisplaceVertex(targetTriangleIndex, forceOnMesh);
		}
		else if (manipulationMode == ManipulationModes.Mesh)
		{
			if (displacementNormal == Vector3.zero)
			{
				displacementNormal = targetMesh.normals[meshTriangles[targetTriangleIndex]];
			}
			DisplaceVertices(targetVertexIndex, forceOnMesh, brushSize, displacementNormal);
		}

		UpdateTargetPosition();
	}


	private void DisplaceVertex(int targetTriangleIndex, Vector3 normal, float force)
	{
		Vector3 targetVertexPoint = meshVertices[meshTriangles[targetTriangleIndex]];
		Vector3 vertexPoint = Vector3.zero;

		List<int> indices = new List<int>();

		// learned somewhere else, todo here: Only one vertex point needed
		for (int i = 0; i < meshTriangles.Length; i++)
		{
			vertexPoint = meshVertices[meshTriangles[i]];
			float sqrMagnitude = (vertexPoint - targetVertexPoint).sqrMagnitude;

			if (vertexPoint != targetVertexPoint)
			{
				continue;
			}

			indices.Add(i);
		}

		normal = normal.normalized;
		DisplaceVertexGroup(indices, normal, force);
	}

	/**
	 *  Saving points in List is important to extrude based on a combination of the vertices
	 */
	private void DisplaceVertex(int targetTriangleIndex, float force)
	{
		Vector3 targetVertexPoint = meshVertices[meshTriangles[targetTriangleIndex]];
		Vector3 vertexPoint = Vector3.zero;

		List<int> indices = new List<int>();
		Vector3 normal = Vector3.zero;

		for (int i = 0; i < meshTriangles.Length; i++)
		{
			vertexPoint = meshVertices[meshTriangles[i]];
			float sqrMagnitude = (vertexPoint - targetVertexPoint).sqrMagnitude;

			if (vertexPoint != targetVertexPoint)
			{
				continue;
			}

			indices.Add(i);
			normal += targetMesh.normals[meshTriangles[i]];
		}

		normal = normal.normalized;
		DisplaceVertexGroup(indices, normal, force);
	}

	private void DisplaceVertices(int middleVertexIndex, float force, float brushSizeRadius, Vector3 displaceNormal)
	{
		// search indices > 0.2f < brushSizeRadius
		// adjust force
		//DisplaceVertex(vertexIndex, force, 0.2f);

		Vector3 middleVertex = meshVertices[middleVertexIndex];

		for (int i = 0; i < meshVertices.Length; i++)
		{
			Vector3 tempVertex = meshVertices[i];
			float distance = Vector3.Distance(middleVertex, tempVertex);
			if (distance > brushSizeRadius)
			{
				continue;
			}

			int triangleIndex = GetTriangleIndexFromVertex(tempVertex);
			float relativeForce = Mathf.Lerp(force, force / 3, distance / brushSizeRadius);
			// normal += targetMesh.normals[meshTriangles[i]];
			DisplaceVertex(triangleIndex, displaceNormal, force);
		}
	}

	private int GetTriangleIndexFromVertex(Vector3 vertex)
	{
		int triangleIndex = -1;
		int counter = 0;

		while (triangleIndex < 0 || counter < meshTriangles.Length)
		{
			if (meshVertices[meshTriangles[counter]] == vertex)
			{
				triangleIndex = counter;
			}
			counter++;
		}

		return triangleIndex;
	}

	private void DisplaceVertexGroup(List<int> indices, Vector3 normal, float force)
	{
		if (indices.Count > 0)
		{
			//Debug.Log("normal: " + normal + " force: " + force);
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
		if (Input.GetButtonDown("SwitchManipulation"))
		{
			switch (manipulationMode)
			{
				case ManipulationModes.Pyramid:
					manipulationMode = ManipulationModes.Mesh;
					break;
				case ManipulationModes.Mesh:
					manipulationMode = ManipulationModes.Pyramid;
					break;
				default:
					manipulationMode = ManipulationModes.Pyramid;
					break;
			}
		}

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
	}
}
