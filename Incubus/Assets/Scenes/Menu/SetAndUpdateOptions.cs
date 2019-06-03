using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetAndUpdateOptions : MonoBehaviour
{
	public Slider cameraSpeedSlider;
	public Slider audioLevel;
	public Toggle invertCamera;
    public Button backButton;

	public void SetCameraInvertBool(bool value)
	{
		PlayerCamera.InvertCamera = value;
	}

	public void SetCameraSpeed(float value)
	{
		PlayerCamera.CameraMultipier = value;
	}

	public void SetAudioVolume(float value)
	{
		AudioListener.volume = value;
	}

	private void Start()
	{
		cameraSpeedSlider.value = PlayerCamera.CameraMultipier;
		invertCamera.isOn = PlayerCamera.InvertCamera;
		audioLevel.value = AudioListener.volume;
	}

    private void Update()
    {
        if (gameObject.activeSelf && Input.GetButtonDown("Cancel"))
        {
            backButton.onClick.Invoke();
        }
    }
}