using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class GameEndTrigger : MonoBehaviour
{
	//[SerializeField] private VideoClip closingVideo;
	[SerializeField] private PlayerController playerController;
	[SerializeField] private PlayerCamera playerCamera;
	[SerializeField] private GameObject displayPlane;
	private VideoPlayer videoPlayer;

	//private VideoPlayer videoPlayer;

	private void Start()
	{
		displayPlane.GetComponent<MeshRenderer>().enabled = false;
		videoPlayer = displayPlane.GetComponent<VideoPlayer>();
		//GameObject cameraObj = GameObject.Find("Main Camera");
		//Camera camera = cameraObj.GetComponent<Camera>();
		//videoPlayer = cameraObj.AddComponent<UnityEngine.Video.VideoPlayer>();
		//videoPlayer = new VideoPlayer();
		//videoPlayer.targetCamera = camera;
		//videoPlayer.source = VideoSource.VideoClip;
		videoPlayer.playOnAwake = false;
		//videoPlayer.isLooping = false;
		//videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.CameraNearPlane;
		videoPlayer.aspectRatio = VideoAspectRatio.FitInside;
		//videoPlayer.clip = closingVideo;
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
        DisablePostProcessing();
		displayPlane.GetComponent<MeshRenderer>().enabled = true;
		videoPlayer.Play();
	}

	private void DisablePostProcessing()
	{
		playerCamera.GetComponent<PostProcessLayer>().enabled = false;
	}

	private void Quit(VideoPlayer vp)
	{
        videoPlayer.Stop();
        //displayPlane.GetComponent<MeshRenderer>().enabled = false;
        SceneManager.LoadScene(1, LoadSceneMode.Single);
		return;

#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}
