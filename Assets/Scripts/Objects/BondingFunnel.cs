﻿using UnityEngine;
using System.Collections;

public class BondingFunnel : MonoBehaviour {

	public Rigidbody pushee;
	public GameObject pusheeCap1;
	public GameObject pusheeCap2;
	public GameObject bearingPusher;
	public GameObject scaleBearing1;
	public GameObject scaleBearing2;
	public Rigidbody noBacktrackBody1;
	public Rigidbody noBacktrackBody2;
	public float scaleModification = 0;
	public float destroyBearingDistance = 3.5f;
	public float destroyBackupDistance = 24;
	private float pusheeLocalY = 0;
	private bool funnelingOut = false;
	private float capXZRatio = 1;

	void Start()
	{
		pusheeLocalY = pushee.transform.localPosition.y;
		noBacktrackBody1.centerOfMass = Vector3.zero;
		noBacktrackBody2.centerOfMass = Vector3.zero;
	}

	void Update()
	{
		if (scaleBearing1 != null && scaleBearing2 != null)
		{
			Vector3 pusheeScale = pushee.transform.localScale;
			pusheeScale.y = Mathf.Abs(scaleBearing2.transform.localPosition.y - scaleBearing1.transform.localPosition.y) + scaleModification;

			if (!funnelingOut && pusheeScale.y <= destroyBearingDistance)
			{
				funnelingOut = true;
			}
			else if (funnelingOut && pusheeScale.y >= destroyBackupDistance)
			{
				Destroy(scaleBearing1);
				scaleBearing1 = null;
				Destroy(scaleBearing2);
				scaleBearing2 = null;
				Destroy(bearingPusher);
				bearingPusher = null;

				SpringJoint[] nonCollisionSprings = GetComponentsInChildren<SpringJoint>();
				for (int i = 0; i < nonCollisionSprings.Length; i++)
				{
					if (nonCollisionSprings[i].connectedBody == pushee)
					{
						Destroy(nonCollisionSprings[i]);
					}
				}
			}

			pushee.transform.localScale = pusheeScale;

			Vector3 capScale = pusheeCap1.transform.localScale;
			capScale.z = (capScale.x * pusheeScale.x) / pusheeScale.y;
			pusheeCap1.transform.localScale = capScale;
			pusheeCap2.transform.localScale = capScale;
		}

		Vector3 pusheePos = pushee.transform.localPosition;
		pusheePos.y = pusheeLocalY;
	}

	public void StopBackTracking()
	{
		noBacktrackBody1.isKinematic = false;
		noBacktrackBody2.isKinematic = false;
	}
}
