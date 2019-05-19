using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetAndUpdateOptions : MonoBehaviour
{
	public Slider cameraSpeedSlider;
	public Toggle invertCamera;

	public void SetCameraInvertBool(bool value)
	{
		PlayerCamera.InvertCamera = value;
	}

	public void SetCameraSpeed(float value)
	{
		PlayerCamera.CameraMultipier = value;
	}

	void Start()
	{
		cameraSpeedSlider.value = PlayerCamera.CameraMultipier;
		invertCamera.isOn = PlayerCamera.InvertCamera;
	}
}