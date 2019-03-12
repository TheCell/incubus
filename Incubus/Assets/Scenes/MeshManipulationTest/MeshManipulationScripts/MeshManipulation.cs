using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshManipulation : MonoBehaviour
{
	[SerializeField] private LayerMask manipulatableLayers;
	[SerializeField] private PlayerCamera playerCamera;
	[SerializeField] private float manipulationSpeed = 0.8f;
	[SerializeField] private float radiusOfEffect = 0.3f;
	private PlayerController playerController;
	
	enum ManipulationModes
	{
		Pyramid = 1,
		Mesh = 2
	}

	public enum CurveType
	{
		Curve1, Curve2
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

	// pyramidManipulation
	Vector3 targetPosition = Vector3.zero;
	private ManipulationModes manipulationMode = ManipulationModes.Pyramid;

	// area Manipulation

	private CurveType curveType;
	Curve curve;

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

		CurveType1();
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

		if (manipulationMode == ManipulationModes.Pyramid)
		{
			DisplaceVertex(targetVertexIndex, (shrink + strecht));
		}
		else if (manipulationMode == ManipulationModes.Mesh)
		{
			/*
			Vector3 targetVertex = meshVertices[targetVertexIndex];
			Vector3 relativePoint = meshFilter.transform.InverseTransformPoint(targetVertex);
			DisplaceVertices(relativePoint, (shrink + strecht), radiusOfEffect);
			*/
			DisplaceVertices(targetVertexIndex, (shrink + strecht), 10f);

		}

		UpdateTargetPosition();

		/*
		Debug.DrawLine(p0, p1);
		Debug.DrawLine(p1, p2);
		Debug.DrawLine(p2, p0);
		*/
	}

	/**
	 *  Saving points in List is important to extrude based on a combination of the vertices
	 */
	private void DisplaceVertex(int vertexIndex, float force)
	{
		Vector3 targetVertexPoint = meshVertices[meshTriangles[vertexIndex]];
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

		UpdateMeshData();
	}

	private void DisplaceVertices(int middleVertexIndex, float force, float brushSizeRadius)
	{
		// search indices > 0.2f < brushSizeRadius
		// adjust force
		//DisplaceVertex(vertexIndex, force, 0.2f);

		for (int i = 0; i < meshVertices.Length; i++)
		{
			
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

	/*
	private void DisplaceVertices(Vector3 pos, float force, float radius)
	{
		Vector3 vertex = Vector3.zero;
		float sqrRadius = radius * radius;

		for (int i = 0; i < meshTriangles.Length; i++)
		{
			Vector3 normal = Vector3.zero;
			float sqrMagnitude = (meshVertices[meshTriangles[i]] - pos).sqrMagnitude;
			if (sqrMagnitude > sqrRadius)
			{
				continue;
			}

			vertex = meshVertices[meshTriangles[i]];
			normal += targetMesh.normals[meshTriangles[i]];
			float distance = Mathf.Sqrt(sqrMagnitude);

			float increment = curve.GetPoint(distance).y * force;
			Vector3 newPosition = meshVertices[meshTriangles[i]] + ((normal * increment) * Time.fixedDeltaTime);
			meshVertices[meshTriangles[i]] = newPosition;
			//Vector3 translate = (vertex * increment) * Time.deltaTime;
			//Quaternion rotation = Quaternion.Euler(translate);
			//Matrix4x4 m = Matrix4x4.TRS(translate, rotation, Vector3.one);
			//meshVertices[meshTriangles[i]] = m.MultiplyPoint3x4(meshVertices[meshTriangles[i]]);
		}

		UpdateMeshData();
	}
	*/

	private void DisplaceVertexGroup(List<int> indices, Vector3 normal, float force)
	{
		if (indices.Count > 0)
		{
			Vector3 newPosition = meshVertices[meshTriangles[indices[0]]] + ((normal * force) * Time.fixedDeltaTime);

			indices.ForEach(delegate (int index)
			{
				meshVertices[meshTriangles[index]] = newPosition;
			});
		}
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

	void CurveType1()
	{
		Vector3[] curvepoints = new Vector3[3];
		curvepoints[0] = new Vector3(0, 1, 0);
		curvepoints[1] = new Vector3(0.5f, 0.5f, 0);
		curvepoints[2] = new Vector3(1, 0, 0);
		curve = new Curve(curvepoints[0], curvepoints[1], curvepoints[2], false);
	}

	void CurveType2()
	{
		Vector3[] curvepoints = new Vector3[3];
		curvepoints[0] = new Vector3(0, 0, 0);
		curvepoints[1] = new Vector3(0.5f, 1, 0);
		curvepoints[2] = new Vector3(1, 0, 0);
		curve = new Curve(curvepoints[0], curvepoints[1], curvepoints[2], false);
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
