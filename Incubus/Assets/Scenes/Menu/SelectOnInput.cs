using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectOnInput : MonoBehaviour
{
	public EventSystem eventSystem;
	public GameObject selectedGameObject;

	private bool buttonSelected;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetAxisRaw("Vertical") != 0f && buttonSelected == false)
		{
			eventSystem.SetSelectedGameObject(selectedGameObject);
			buttonSelected = true;
		}
    }

	private void OnDisable()
	{
		buttonSelected = false;
	}
}
