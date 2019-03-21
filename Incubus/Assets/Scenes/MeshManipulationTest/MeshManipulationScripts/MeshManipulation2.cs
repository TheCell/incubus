using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshManipulation2 : MonoBehaviour
{
	[SerializeField] private LayerMask manipulatableLayers;
	[SerializeField] private PlayerCamera playerCamera;
	[SerializeField] private float displacementSpeed = 5f;
	[SerializeField] private float meshBrushSize = 5f;
	private PlayerController playerController;

	enum ManipulationModes
	{
		Pyramid = 1,
		Mesh = 2
	}

	// inputs
	private float strecht, shrink, force;
	private ManipulationModes manipulationMode = ManipulationModes.Pyramid;
	private bool isManipulating;

	// Ray Logic
	private float maxRayDistance = 5f;
	private RaycastHit rayCastHit;

	// Mesh infos
	private Mesh targetMesh;
	private MeshCollider meshCollider;
	private MeshFilter meshFilter;
	private Vector3[] meshVertices;
	private Vector3[] meshNormals;
	private int[] meshTriangles;
	private Vector3 displacementNormal = Vector3.zero;

	// Vertexpoints used to display points and indices for manipulating
	int targetVertexIndex = -1;
	List<int> meshVertexIndices = new List<int>();
	
	// only for displaying
	Vector3 targetVertexPosition = Vector3.zero;
	List<Vector3> vertexPositionsAroundTargetVertexPosition = new List<Vector3>();

	// spheres
	private GameObject targetmanipulationBall;
	private GameObject[] areaManipulationBalls = new GameObject[10];
	[SerializeField] private Material manipulationMainSphereMaterial;
	[SerializeField] private Material manipulationSphereMaterial;

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
		ResetMeshInfos();
		InitializeSpheres();
	}

	private void Update()
	{
		GetInput();
		RayCastWhileNotManipulating();
		SetMeshInfosIfFound();
		UpdateIndicesAndPositions();
		CheckAndStartManipulation();
		DisplaySpheres();
	}

	private void FixedUpdate()
	{
		ManipulateVertex();
	}

	private void RayCastWhileNotManipulating()
	{
		if (!isManipulating)
		{
			Vector3 rayStartPoint = transform.position + playerCamera.LookAtPlayerOffset;
			Ray ray = new Ray(rayStartPoint, playerCamera.transform.forward);
			Physics.Raycast(ray, out rayCastHit, maxRayDistance, manipulatableLayers);
			if (rayCastHit.collider == null)
			{
				// player does not aim at the ground, try backup ray but can fail aswell
				Vector3 directionToGround = playerCamera.GetFloorLevelDot.transform.position - (transform.position + playerCamera.LookAtPlayerOffset);
				ray = new Ray(rayStartPoint, directionToGround);
				Physics.Raycast(ray, out rayCastHit, maxRayDistance + 2f, manipulatableLayers);
			}
		}
	}

	private void ResetMeshInfos()
	{
		targetMesh = null;
		meshCollider = null;
		meshFilter = null;
		meshVertices = null;
		meshNormals = null;
		meshTriangles = null;
	}

	private void SetMeshInfosIfFound()
	{
		if (rayCastHit.collider == null)
		{
			ResetMeshInfos();
			return;
		}

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

		targetMesh = meshCollider.sharedMesh;
		meshVertices = targetMesh.vertices;
		meshTriangles = targetMesh.triangles;
		meshNormals = targetMesh.normals;
	}

	private void UpdateMeshData()
	{
		targetMesh.vertices = meshVertices;
		targetMesh.RecalculateBounds();
		//targetMesh.RecalculateNormals();
		meshCollider.gameObject.SetActive(false);
		meshCollider.gameObject.SetActive(true);
	}

	private void UpdateIndicesAndPositions()
	{
		if (rayCastHit.collider == null)
		{
			if (isManipulating)
			{
				// update only positions from indices
				UpdatePositionsFromIndices();
			}
			else
			{
				// reset indices and positions, we are not manipulating and do not have a surface
				targetVertexIndex = -1;
				meshVertexIndices = new List<int>();
				targetVertexPosition = Vector3.zero;
				vertexPositionsAroundTargetVertexPosition = new List<Vector3>();
			}
		}
		else
		{
			if (isManipulating)
			{
				// update only positions from indices
				UpdatePositionsFromIndices();
			}
			else
			{
				// calculate and update indices and positions
				UpdateIndices();
				UpdatePositionsFromIndices();
			}
		}
	}

	private void UpdateIndices()
	{
		// this will only be called when raycast hit something and we are not already manipulating
		UpdateTargetIndex();
		UpdateIndicesAroundTargetIndex();
	}

	private void UpdateIndicesAroundTargetIndex()
	{
		meshVertexIndices = new List<int>();
		Transform hitTransform = rayCastHit.collider.transform;
		Vector3 targetVertexWorldPosition = hitTransform.TransformPoint(meshVertices[targetVertexIndex]);

		for (int i = 0; i < meshVertices.Length; i++)
		{
			Vector3 tempVertex = meshVertices[i];
			tempVertex = hitTransform.TransformPoint(tempVertex);

			float distance = Vector3.Distance(targetVertexWorldPosition, tempVertex);
			if (distance == 0f || distance > meshBrushSize)
			{
				continue;
			}

			meshVertexIndices.Add(i);
		}
	}

	private void UpdateTargetIndex()
	{
		Vector3 raycastHitPosition = rayCastHit.point;
		int triangleVertexIndexP0 = rayCastHit.triangleIndex * 3 + 0;
		int triangleVertexIndexP1 = rayCastHit.triangleIndex * 3 + 1;
		int triangleVertexIndexP2 = rayCastHit.triangleIndex * 3 + 2;
		int vertexIndexP0 = meshTriangles[triangleVertexIndexP0];
		int vertexIndexP1 = meshTriangles[triangleVertexIndexP1];
		int vertexIndexP2 = meshTriangles[triangleVertexIndexP2];
		int targetTriangleIndex;
		int tempVertexIndex;

		Vector3 p0 = meshVertices[vertexIndexP0];
		Vector3 p1 = meshVertices[vertexIndexP1];
		Vector3 p2 = meshVertices[vertexIndexP2];
		Transform hitTransform = rayCastHit.collider.transform;
		// Transforms position from local space to worldspace
		p0 = hitTransform.TransformPoint(p0);
		p1 = hitTransform.TransformPoint(p1);
		p2 = hitTransform.TransformPoint(p2);

		// find closest vertex index to raycast hit
		float distanceToP0 = Vector3.Distance(raycastHitPosition, p0);
		float distanceToP1 = Vector3.Distance(raycastHitPosition, p1);
		float distanceToP2 = Vector3.Distance(raycastHitPosition, p2);

		if (distanceToP0 < distanceToP1 && distanceToP0 < distanceToP2)
		{
			targetTriangleIndex = triangleVertexIndexP0;
			tempVertexIndex = meshTriangles[targetTriangleIndex];
		}
		else if (distanceToP1 < distanceToP2)
		{
			targetTriangleIndex = triangleVertexIndexP1;
			tempVertexIndex = meshTriangles[targetTriangleIndex];
		}
		else
		{
			targetTriangleIndex = triangleVertexIndexP2;
			tempVertexIndex = meshTriangles[targetTriangleIndex];
		}

		targetVertexIndex = tempVertexIndex;
	}

	private void ManipulateVertex()
	{
		if (!isManipulating)
		{
			return;
		}
		
		if (manipulationMode == ManipulationModes.Pyramid)
		{
			DisplaceTargetVertex();
		}
		else if (manipulationMode == ManipulationModes.Mesh)
		{
			DisplaceTargetVertex();
			DisplaceAroundTargetVertex();
		}
	}

	private void DisplaceTargetVertex()
	{
		Vector3 localIndexPosition = meshVertices[targetVertexIndex];

		Transform hitTransform = rayCastHit.collider.transform;
		float forceOnMesh = (force) / ((hitTransform.localScale.x + hitTransform.localScale.y + hitTransform.localScale.z) / 3);
		Vector3 deltaPosition = displacementNormal * forceOnMesh * Time.fixedDeltaTime;

		for (int i = 0; i < meshVertices.Length; i++)
		{
			Vector3 tempPosition = meshVertices[i];
			if (Vector3.Distance(localIndexPosition, tempPosition) == 0)
			{
				meshVertices[i] = meshVertices[i] + deltaPosition;
			}
		}
		UpdateMeshData();
	}

	private void DisplaceAroundTargetVertex()
	{
		Transform hitTransform = rayCastHit.collider.transform;
		float forceOnMesh = (force) / ((hitTransform.localScale.x + hitTransform.localScale.y + hitTransform.localScale.z) / 3);
		Vector3 deltaPosition = displacementNormal * forceOnMesh * Time.fixedDeltaTime;

		foreach (int index in meshVertexIndices)
		{
			meshVertices[index] = meshVertices[index] + deltaPosition;
		}
		UpdateMeshData();
	}
	
	private void UpdatePositionsFromIndices()
	{
		Transform hitTransform = rayCastHit.collider.transform;
		// targetVertex
		if (targetVertexIndex > -1)
		{
			Vector3 localPosition = meshVertices[targetVertexIndex];
			targetVertexPosition = hitTransform.TransformPoint(localPosition);
		}
		else
		{
			targetVertexPosition = Vector3.zero;
		}

		// other Vertices around targetVertex
		vertexPositionsAroundTargetVertexPosition = new List<Vector3>();
		foreach (int vertexIndex in meshVertexIndices)
		{
			Vector3 localPosition = meshVertices[vertexIndex];
			Vector3 vertexWorldPosition = hitTransform.TransformPoint(localPosition);
			// multiple indices for different triangles hold the same position
			if (!vertexPositionsAroundTargetVertexPosition.Contains(vertexWorldPosition))
			{
				vertexPositionsAroundTargetVertexPosition.Add(vertexWorldPosition);
			}
		}
	}

	private void DisplaySpheres()
	{
		if (targetVertexPosition != Vector3.zero)
		{
			targetmanipulationBall.transform.position = targetVertexPosition;
			targetmanipulationBall.SetActive(true);
		}
		else
		{
			targetmanipulationBall.SetActive(false);
		}

		int count = 0;
		// display as many balls as there are entrys, hide the rest
		if (manipulationMode == ManipulationModes.Mesh)
		{
			// display one at each world position
			foreach (Vector3 listEntry in vertexPositionsAroundTargetVertexPosition)
			{
				if (count < areaManipulationBalls.Length)
				{
					areaManipulationBalls[count].transform.position = listEntry;
					areaManipulationBalls[count].SetActive(true);
				}

				count++;
			}
		}

		for (; count < areaManipulationBalls.Length; count++)
		{
			areaManipulationBalls[count].SetActive(false);
		}
	}

	private void InitializeSpheres()
	{
		// setup main ball
		targetmanipulationBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		Destroy(targetmanipulationBall.GetComponent<Collider>());
		targetmanipulationBall.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
		targetmanipulationBall.GetComponent<Renderer>().material = manipulationMainSphereMaterial;
		targetmanipulationBall.SetActive(false);

		// setup area balls
		GameObject areaBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		Destroy(areaBall.GetComponent<Collider>());
		areaBall.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
		areaBall.GetComponent<Renderer>().material = manipulationSphereMaterial;
		areaBall.SetActive(false);
		//areaManipulationBalls

		for (int i = 0; i < 10; i++)
		{
			areaManipulationBalls[i] = Instantiate(areaBall);
		}
	}

	private void CheckAndStartManipulation()
	{
		if (!isManipulating && (Mathf.Abs(shrink) + Mathf.Abs(strecht) <= 0.3f) && rayCastHit.collider != null)
		{
			isManipulating = true;
			displacementNormal = meshNormals[targetVertexIndex];
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
		if (Input.GetButton("StretchMesh"))
		{
			strecht = 1f;
		}
		if ((Input.GetButton("ShrinkMesh")))
		{
			shrink = -1f;
		}

		force = (shrink + strecht) * displacementSpeed;

		if (isManipulating && (Mathf.Abs(shrink) + Mathf.Abs(strecht) <= 0.3f))
		{
			isManipulating = false;
		}
	}
}
