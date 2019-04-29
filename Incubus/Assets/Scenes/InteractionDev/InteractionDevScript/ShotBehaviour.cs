using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotBehaviour : MonoBehaviour
{
	[SerializeField] private float pushForce = 50f;
	private AudioSource audioSource;
	private Rigidbody bulletRigidbody;
	private ParticleSystem particleSystem;

	private void Start()
	{
		audioSource = GetComponentInChildren<AudioSource>();
		bulletRigidbody = GetComponentInChildren<Rigidbody>();
	}

	private void OnEnable()
	{
		particleSystem = GetComponentInChildren<ParticleSystem>();

		if (particleSystem != null)
		{
			particleSystem.Play();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 11)
		{
			// ignore triggers layer
			return;
		}

		if (other.gameObject.layer == 8) // player layer
		{
			if (audioSource != null)
			{
				//audioSource.Play();
				audioSource.transform.position = bulletRigidbody.position;
				audioSource.PlayOneShot(audioSource.clip);
			}

			PlayerController playerController = other.GetComponent<PlayerController>();
			if (playerController != null)
			{
				playerController.SetImpact(bulletRigidbody.velocity * pushForce);
			}
			//Rigidbody otherRB = other.gameObject.GetComponent<Rigidbody>();
			//otherRB.velocity = bulletRigidbody.velocity * pushForce;
			bulletRigidbody.gameObject.SetActive(false);
			//gameObject.SetActive(false);
		}
		else
		{
			gameObject.SetActive(false);
		}
	}

	public void HandleTriggerEnterFromChild(Collider other)
	{
		if (other.gameObject.layer == 11)
		{
			// ignore triggers layer
			return;
		}

		if (other.gameObject.layer == 8) // player layer
		{
			//audioSource.Play();
			audioSource.transform.position = bulletRigidbody.position;
			audioSource.PlayOneShot(audioSource.clip);
			PlayerController playerController = other.GetComponent<PlayerController>();
			if (playerController != null)
			{
				playerController.SetImpact(bulletRigidbody.velocity * pushForce);
			}
			//Rigidbody otherRB = other.gameObject.GetComponent<Rigidbody>();
			//otherRB.velocity = bulletRigidbody.velocity * pushForce;
			bulletRigidbody.gameObject.SetActive(false);
			//gameObject.SetActive(false);
		}
		else
		{
			gameObject.SetActive(false);
		}
	}
}
