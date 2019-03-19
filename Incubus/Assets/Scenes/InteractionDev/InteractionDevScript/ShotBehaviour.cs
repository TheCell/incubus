using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotBehaviour : MonoBehaviour
{
	[SerializeField] private float pushForce = 50f;
	private AudioSource audioSource;
	private Rigidbody bulletRigidbody;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		bulletRigidbody = GetComponent<Rigidbody>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 8)
		{
			//audioSource.Play();
			audioSource.PlayOneShot(audioSource.clip);
			PlayerController playerController = other.GetComponent<PlayerController>();
			if (playerController != null)
			{
				playerController.SetImpact(bulletRigidbody.velocity * pushForce);
			}
			//Rigidbody otherRB = other.gameObject.GetComponent<Rigidbody>();
			//otherRB.velocity = bulletRigidbody.velocity * pushForce;
			//gameObject.SetActive(false);
		}
	}

	private void OnDisable()
	{
		if (audioSource != null)
		{
		}
	}
}
