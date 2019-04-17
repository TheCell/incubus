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
	private GameObject indicator;
	
	private void Start()
    {
		indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		Destroy(indicator.GetComponent<SphereCollider>());
		indicator.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

		if (shotPrefab == null)
		{
			Debug.LogError("missing Shot Prefab");
		}

		// prefill Pool
		for (int i = 0; i < 5; i++)
		{
			GameObject shot = (GameObject)Instantiate(shotPrefab);
			shot.SetActive(false);
			bulletPool.Add(shot);
		}
		bulletEnumerator = bulletPool.GetEnumerator();
		lastShot = Time.time;
	}
	
	private void Update()
    {
		indicator.transform.position = transform.position + transform.forward;
		if (ShotReady())
		{
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

	private void Shoot()
	{
		if (!bulletEnumerator.MoveNext())
		{
			bulletEnumerator = bulletPool.GetEnumerator();
			bulletEnumerator.MoveNext();
		}

		GameObject bullet = bulletEnumerator.Current;
		bullet.transform.position = transform.position + transform.forward;
		bullet.SetActive(true);
		Rigidbody bulletRB = bullet.GetComponentInChildren<Rigidbody>();
		//bullet.transform.rotation = transform.rotation;
		//bulletRB.position = transform.position + transform.forward;
		//bulletRB.gameObject.transform.position = Vector3.zero;
		//bulletRB.gameObject.transform.rotation = transform.rotation;
		bulletRB.position = Vector3.zero;
		bulletRB.rotation = transform.rotation;
		bulletRB.gameObject.SetActive(true);
		//bulletRB.velocity = bullet.transform.forward * bulletSpeed;
		bulletRB.velocity = transform.forward * bulletSpeed;
	}
}
