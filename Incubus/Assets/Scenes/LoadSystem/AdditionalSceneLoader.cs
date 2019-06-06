using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditionalSceneLoader : MonoBehaviour
{
	private List<string> currentScenes = new List<string>();
	private List<string> scenesToLoad = new List<string>();
	private List<string> scenesToUnload = new List<string>();
	private Scene rootScene;
	private string worldDecoration = "WorldDecoration";

	private void Start()
	{
		rootScene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(worldDecoration, LoadSceneMode.Additive);
	}

	public void LoadScenes(SceneInformation sceneInformation)
	{
		UpdateScenesToLoad(sceneInformation);
		LoadScenes();

		UpdateScenesToUnload(sceneInformation);
		UnloadScenes();
	}

	private void LoadScene(string sceneName)
	{
		if (!sceneName.Equals(rootScene.name))
		{
			if (sceneName.Equals("StartLevel 2"))
			{
				SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
			}
			else
			{
				SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
			}
		}
	}

	private void LoadScenes()
	{
		scenesToLoad.ForEach(item =>
		{
			LoadScene(item);
		});
	}

	private void UnloadScenes()
	{
		scenesToUnload.ForEach(item =>
		{
			UnloadScene(item);
		});
	}

	private void UnloadScene(string sceneName)
	{
		if (!sceneName.Equals(rootScene.name) && !sceneName.Equals(worldDecoration))
		{
			SceneManager.UnloadSceneAsync(sceneName);
		}
	}

	private void UpdateScenesToLoad(SceneInformation sceneInfo)
	{
		string[] shouldBeScenes = sceneInfo.GetShouldBeActiveScenes();
		List<string> newScenes = new List<string>();
		
		for (int i = 0; i < shouldBeScenes.Length; i++)
		{
			string sceneName = shouldBeScenes[i];
			if (!currentScenes.Contains(sceneName))
			{
				newScenes.Add(sceneName);
			}
		}

		scenesToLoad = newScenes;
		AddToCurrentScenes(newScenes);
	}

	private void AddToCurrentScenes(List<string> newScenes)
	{
		newScenes.ForEach(item =>
		{
			if (!currentScenes.Contains(item))
			{
				currentScenes.Add(item);
			}
		});
	}
	
	private void UpdateScenesToUnload(SceneInformation sceneInfo)
	{
		List<string> relevantScenes = new List<string>(sceneInfo.GetShouldBeActiveScenes());
		List<string> newScenesToUnload = new List<string>();

		currentScenes.ForEach(item =>
		{
			if (!relevantScenes.Contains(item))
			{
				newScenesToUnload.Add(item);
			}
		});
		
		scenesToUnload = newScenesToUnload;

		scenesToUnload.ForEach(item =>
		{
			currentScenes.Remove(item);
		});
	}
}
