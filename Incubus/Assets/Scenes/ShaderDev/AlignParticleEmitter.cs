using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignParticleEmitter : MonoBehaviour
{
	[SerializeField] private GameObject particleContainer;
	private PlayerController playerController;
	private Rigidbody rb;
	private ParticleSystem[] particleEffects;
	

	// Start is called before the first frame update
	private void Start()
    {
		rb = gameObject.GetComponent<Rigidbody>();
		particleEffects = gameObject.GetComponentsInChildren<ParticleSystem>();
		playerController = gameObject.GetComponent<PlayerController>();
		if (playerController == null)
		{
			Debug.LogError("no playercontroller found");
		}
	}

	// Update is called once per frame
	private void Update()
    {
		RotateContainer();
		EnableDisableOnSprint();
	}

	private void RotateContainer()
	{
		Vector3 velocity = rb.velocity;
		if (velocity != Vector3.zero)
		{
			particleContainer.transform.rotation = Quaternion.LookRotation(velocity, Vector3.up);
		}
	}

	private void EnableDisableOnSprint()
	{
		if (playerController.IsSprinting() && playerController.IsGrounded())
		{
			for (int i = 0; i < particleEffects.Length; i++)
			{
				particleEffects[i].Play();
			}
		}
		else
		{
			for (int i = 0; i < particleEffects.Length; i++)
			{
				particleEffects[i].Stop();
			}
		}
	}
}
