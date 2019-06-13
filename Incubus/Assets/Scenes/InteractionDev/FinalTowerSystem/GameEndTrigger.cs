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

	private void Start()
	{
		displayPlane.GetComponent<MeshRenderer>().enabled = false;
		videoPlayer = displayPlane.GetComponent<VideoPlayer>();
		videoPlayer.playOnAwake = false;
		videoPlayer.aspectRatio = VideoAspectRatio.FitInside;
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
			BlockPlayerMovement();
			videoPlayer.SetDirectAudioVolume(0, AudioListener.volume);
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
        displayPlane.GetComponent<MeshRenderer>().enabled = false;
		playerCamera.GetComponent<PostProcessLayer>().enabled = true;
		SceneManager.LoadScene(1, LoadSceneMode.Single);
        return;
	}
}
