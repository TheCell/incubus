using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInformation : MonoBehaviour
{
	[SerializeField] private string[] activeScenes;

	public string[] GetShouldBeActiveScenes()
	{
		return activeScenes;
	}

	private void OnTriggerEnter(Collider other)
	{
		AdditionalSceneLoader sceneLoader = other.GetComponent<AdditionalSceneLoader>();
		if (sceneLoader == null)
		{
			return;
		}

		sceneLoader.LoadScenes(this);
	}
}
