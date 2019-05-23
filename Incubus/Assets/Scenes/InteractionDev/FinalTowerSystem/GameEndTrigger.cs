using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using System;
using UnityEngine.SceneManagement;

public class GameEndTrigger : MonoBehaviour
{
	[SerializeField] private VideoClip closingVideo;
	[SerializeField] private PlayerController playerController;
	[SerializeField] private PlayerCamera playerCamera;

	private VideoPlayer videoPlayer;

	private void Start()
	{
		GameObject cameraObj = GameObject.Find("Main Camera");
		//Camera camera = cameraObj.GetComponent<Camera>();
		videoPlayer = cameraObj.AddComponent<UnityEngine.Video.VideoPlayer>();
		//videoPlayer = new VideoPlayer();
		//videoPlayer.targetCamera = camera;
		//videoPlayer.source = VideoSource.VideoClip;
		videoPlayer.playOnAwake = false;
		videoPlayer.isLooping = false;
		videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.CameraNearPlane;
		videoPlayer.aspectRatio = VideoAspectRatio.FitInside;
		videoPlayer.clip = closingVideo;
		videoPlayer.loopPointReached += Quit;
	}

	public void prepareVideo()
	{
		videoPlayer.Prepare();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<PlayerController>() != null)
		{
			//Quit();
			BlockPlayerMovement();
			PlayClosingVideo();
		}
	}

	private void BlockPlayerMovement()
	{
		playerController.SetInputActive(false);
		playerCamera.SetInputActive(false);
	}

	private void PlayClosingVideo()
	{
		videoPlayer.Play();
	}

	private void Quit(VideoPlayer vp)
	{
		SceneManager.LoadScene(0);
		return;

#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}
