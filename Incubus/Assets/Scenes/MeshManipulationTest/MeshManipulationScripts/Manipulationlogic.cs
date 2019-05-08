using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// saves the start meshdata
// measures time after manipulation
// returns the mash to start
public class Manipulationlogic : MonoBehaviour
{
	private float lastManipulationTime;
	private float timeBeforeLerpBack = 3f;
	private float lerpTime = 2.5f;

	// Mesh infos
	private Mesh originalMesh;
	// private MeshCollider originalMeshCollider;
	private MeshFilter originalFilter;
	private Vector3[] originalVertices;
	private Vector3[] originalNormals;
	private int[] originalTriangles;

	// Start is called before the first frame update
	void Start()
    {
		// originalMeshCollider = gameObject.GetComponent<MeshCollider>();

		/*
		if (originalMeshCollider == null || originalMeshCollider.sharedMesh == null)
		{
			Debug.LogError("no meshCollider or sharedMesh on collider");
			return;
		}

		if (originalMeshFilter == null)
		{
			Debug.LogError("no meshFilter found");
			return;
		}
		*/

		originalFilter = GetComponent<MeshFilter>();
		Mesh mesh = originalFilter.sharedMesh;
		originalMesh = new Mesh();
		originalMesh.name = "original";
		originalVertices = mesh.vertices;
		originalTriangles = mesh.triangles;
		originalNormals = mesh.normals;
	}

    // Update is called once per frame
    void Update()
    {
		CheckTimeAndManipulateBack();
	}

	public void Manipulating()
	{
		lastManipulationTime = Time.timeSinceLevelLoad;
	}

	private void CheckTimeAndManipulateBack()
	{
		if (lastManipulationTime + timeBeforeLerpBack < Time.timeSinceLevelLoad)
		{
			ManipulateBack();
		}
	}

	private void ManipulateBack()
	{
		MeshFilter currentFilter = GetComponent<MeshFilter>();
		Mesh currentMesh = currentFilter.sharedMesh;
		
		float deltatimeAfterManipulation = Time.timeSinceLevelLoad - (lastManipulationTime + timeBeforeLerpBack);
		deltatimeAfterManipulation = Mathf.Clamp(deltatimeAfterManipulation, 0, lerpTime);
		float currentProgress = 1 / lerpTime * deltatimeAfterManipulation;
		//currentMesh.vertices = originalVertices;
		currentMesh.triangles = originalTriangles;
		currentMesh.normals = originalNormals;
		Vector3[] tempVertices = currentMesh.vertices;

		for (int i = 0; i < currentMesh.vertices.Length; i++)
		{
			tempVertices[i] = Vector3.Lerp(currentMesh.vertices[i], originalVertices[i], currentProgress);
		}
		currentMesh.vertices = tempVertices;

		/*
		for (int i = 0; i < currentMesh.vertices.Length; i++)
		{
			currentMesh.vertices[i] = Vector3.Lerp(currentMesh.vertices[i], originalVertices[i], currentProgress);
		}
		for (int i = 0; i < currentMesh.normals.Length; i++)
		{
			currentMesh.normals[i] = Vector3.Lerp(currentMesh.normals[i], originalNormals[i], currentProgress);
		}
		*/

		currentMesh.RecalculateBounds();
		MeshCollider currentCollider = GetComponent<MeshCollider>();
		if (currentCollider != null)
		{
			currentCollider.gameObject.SetActive(false);
			currentCollider.gameObject.SetActive(true);
		}
	}
}
