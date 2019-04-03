using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingOnPlayer : MonoBehaviour
{
	[SerializeField] private float shotIntervalSeconds = 1f;
	[SerializeField] private GameObject shotPrefab;
	[SerializeField] private float bulletSpeed = 20f;

	private float lastShot;
	private List<GameObject> bulletPool = new List<GameObject>();
	private List<GameObject>.Enumerator bulletEnumerator;

	private StationaryEnemyTurning stationaryEnemyTurning;
	
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

		stationaryEnemyTurning = GetComponent<StationaryEnemyTurning>();
		if (stationaryEnemyTurning == null)
		{
			Debug.LogError("missing Script StationaryEnemyTurning");
		}
	}
	
	private void Update()
    {
		if (ShotReady())
		{
			//Shoot(playerPosition);
			Shoot();
		}
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

	/*
	private void Shoot(Vector3 targetPosition)
	{
		if (!bulletEnumerator.MoveNext())
		{
			bulletEnumerator = bulletPool.GetEnumerator();
			bulletEnumerator.MoveNext();
		}
		GameObject bullet = bulletEnumerator.Current;
		bullet.transform.position = transform.position + transform.forward;
		Vector3 targetDirection = targetPosition - transform.position;
		Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 180f, 180.0f);
		bullet.transform.rotation = Quaternion.LookRotation(newDirection);
		bullet.SetActive(true);
		Rigidbody bulletRB = bullet.GetComponentInChildren<Rigidbody>();
		bulletRB.gameObject.SetActive(true);
		bulletRB.velocity = bullet.transform.forward * bulletSpeed;
		//Debug.DrawRay(bullet.transform.position, bullet.transform.forward, Color.red, 1f);
	}
	*/

	private void Shoot()
	{
		if (!bulletEnumerator.MoveNext())
		{
			bulletEnumerator = bulletPool.GetEnumerator();
			bulletEnumerator.MoveNext();
		}

		GameObject bullet = bulletEnumerator.Current;
		bullet.SetActive(true);
		Rigidbody bulletRB = bullet.GetComponentInChildren<Rigidbody>();
		bullet.transform.position = transform.position + transform.forward;
		//bullet.transform.rotation = transform.rotation;
		bulletRB.position = transform.position + transform.forward;
		bulletRB.rotation = transform.rotation;
		bulletRB.gameObject.SetActive(true);
		//bulletRB.velocity = bullet.transform.forward * bulletSpeed;
		bulletRB.velocity = transform.forward * bulletSpeed;
	}
}
