﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameOnClick : MonoBehaviour
{
    public void StartGameByIndex(int sceneIndex)
	{
		SceneManager.LoadScene(sceneIndex);
	}
}