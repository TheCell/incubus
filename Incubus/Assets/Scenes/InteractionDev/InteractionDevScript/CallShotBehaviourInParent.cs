using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallShotBehaviourInParent : MonoBehaviour
{
	private ShotBehaviour shotBehaviour;

	private void Start()
	{
		shotBehaviour = gameObject.GetComponentInParent<ShotBehaviour>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (shotBehaviour != null)
		{
			shotBehaviour.HandleTriggerEnterFromChild(other);
		}
	}
}
