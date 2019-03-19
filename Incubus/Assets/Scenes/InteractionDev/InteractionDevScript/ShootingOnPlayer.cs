using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingOnPlayer : MonoBehaviour
{
	[SerializeField] private float scanRadius = 15f;
	[SerializeField] private float shotIntervalSeconds = 1f;
	[SerializeField] private GameObject shotPrefab;
	[SerializeField] private float bulletSpeed = 20f;

	private float lastShot;
	private List<GameObject> bulletPool = new List<GameObject>();
	private List<GameObject>.Enumerator bulletEnumerator;

	// Start is called before the first frame update
	private void Start()
    {
		if (shotPrefab == null)
		{
			Debug.LogError("missing Shot Prefab");
		}

		// prefill Pool
		for (int i = 0; i < 20; i++)
		{
			GameObject shot = (GameObject)Instantiate(shotPrefab);
			shot.SetActive(false);
			bulletPool.Add(shot);
		}
		bulletEnumerator = bulletPool.GetEnumerator();

		lastShot = Time.time;
	}

	// Update is called once per frame
	private void Update()
    {
		ShootPlayer();
	}

	private void ShootPlayer()
	{
		Vector3 playerPosition = SearchPlayer();
		Shoot(playerPosition);
	}

	private Vector3 SearchPlayer()
	{
		Vector3 playerPosition = transform.forward;
		GameObject player = GameObject.FindWithTag("Player");
		if (player != null)
		{
			float distance = Vector3.Distance(transform.position, player.transform.position);
			if (distance < scanRadius)
			{
				playerPosition = player.transform.position;
			}
		}
		return playerPosition;
	}

	private bool ShotReady()
	{
		if (lastShot + shotIntervalSeconds < Time.time)
		{
			lastShot = Time.time;
			return true;
		}

		return false;
	}

	private void Shoot(Vector3 targetPosition)
	{
		if (ShotReady())
		{
			if (!bulletEnumerator.MoveNext())
			{
				bulletEnumerator = bulletPool.GetEnumerator();
				bulletEnumerator.MoveNext();
			}
			GameObject bullet = bulletEnumerator.Current;
			bullet.transform.position = transform.position;
			Vector3 targetDirection = targetPosition - transform.position;
			Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 180f, 180.0f);
			bullet.transform.rotation = Quaternion.LookRotation(newDirection);
			bullet.SetActive(true);
			bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;
			Debug.DrawRay(bullet.transform.position, bullet.transform.forward, Color.red, 1f);
		}
	}
}
