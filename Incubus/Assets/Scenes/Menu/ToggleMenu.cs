using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMenu : MonoBehaviour
{
	[SerializeField] private GameObject mainMenu;
	[SerializeField] private PlayerController playerController;
	[SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private MeshManipulation2 meshManipulation;

	private void Update()
    {
        if (Input.GetButtonDown("Startmenu"))
		{
			Toggle();
		}

		if (mainMenu.activeSelf)
		{
			Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
		{
			Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
		}
	}

	public void Toggle()
	{
		mainMenu.SetActive(!mainMenu.activeSelf);
		playerController.SetInputActive(!mainMenu.activeSelf);
		playerCamera.SetInputActive(!mainMenu.activeSelf);
        meshManipulation.SetInputActive(!mainMenu.activeSelf);
	}
}
