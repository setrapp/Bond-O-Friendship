﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SimpleMover))]
public class MovePulse : MonoBehaviour {
	[HideInInspector]
	private SimpleMover mover;
	public PulseShot creator;
	public PartnerLink volleyTarget;
	public int volleys;
	public float capacity;
	public Vector3 target;
	public float rotationSpeed = 50.0f;
	//public GameObject pulseCreator;
	public PulseShot volleyPartner;
	public TrailRenderer trail;
	public bool moving = false;
	public float baseAngle = -1;
	public Vector3 baseDirection;
	public Animation swayAnimation;
	private bool disableColliders;
	public Vector3 oldBulbPos;
	public GameObject bulb;
	[HideInInspector]
	public CapsuleCollider hull;
	[HideInInspector]
	public Rigidbody body;
	public FluffStick attachee;
	public float attacheePullRate = 1;
	public bool attacheePossessive = false;
	public GameObject ignoreCollider;
	

	void Awake()
	{
		//pulseCreator = GameObject.Find("Globals");
		oldBulbPos = bulb.transform.position;
		if (hull == null)
		{
			hull = GetComponent<CapsuleCollider>();
		}
		if (body == null)
		{
			body = GetComponent<Rigidbody>();
		}
		mover = GetComponent<SimpleMover>();
	}

	// Update is called once per frame
	void Update() 
	{
		if (disableColliders)
		{
			Collider[] colliders = GetComponentsInChildren<Collider>();
			for (int i = 0; i < colliders.Length; i++)
			{
				colliders[i].enabled = false;
			}
			disableColliders = false;
		}

		if (attacheePossessive && attachee == null)
		{
			attacheePossessive = false;
		}

		bool moverMoving = (mover.velocity.sqrMagnitude > mover.cutSpeedThreshold);

		if (moving != moverMoving)
		{
			if (!moverMoving)
			{
				RaycastHit attachInfo;
				if (Physics.Raycast(transform.position, Vector3.forward, out attachInfo, Mathf.Infinity))
				{
					Attach(attachInfo.collider.gameObject, transform.position, -Vector3.forward);
				}
				trail.gameObject.SetActive(false);
			}
			else
			{
				transform.parent = null;
				// If fluff is pointing more in the z direction than the other directions, rotate into the correct plane.
				if(Mathf.Pow(transform.up.z, 2) > new Vector2(transform.up.x, transform.up.y).sqrMagnitude)
				{
					transform.up = -mover.velocity;
				}
				ToggleSwayAnimation(false);
				trail.gameObject.SetActive(true);
				baseAngle = -1;
			}

			moving = moverMoving;
		}

		if (moving)
		{
			if (ignoreCollider == null && hull.isTrigger)
			{
				hull.isTrigger = false;
			}

			transform.Rotate(0.0f, 0.0f, rotationSpeed * Time.deltaTime);
		}
	}

	public void Pass(Vector3 passForce, GameObject ignoreColliderTemporary = null)
	{
		attachee = null;
		attacheePossessive = false;

		// If something attachable is already in reach, attach without moving.
		RaycastHit attemptPassHit;
		float blockingTestDistance = Mathf.Max(hull.height, hull.radius);
		bool blocked = TestForBlocking(passForce, blockingTestDistance, out attemptPassHit);
		if (blocked)
		{
			moving = true;
			Attach(attemptPassHit.collider.gameObject, attemptPassHit.point, attemptPassHit.normal, true);
			return;
		}

		// Allow fluff to act on physical objects.
		if (body != null)
		{
			body.isKinematic = false;

		}
		ignoreCollider = ignoreColliderTemporary;
		mover.Accelerate(passForce);
	}

	public void Pull(GameObject puller, float pullMagnitude)
	{
		// If something is blocking the path to the puller, do not move.
		RaycastHit attemptPullHit;
		Vector3 toPuller = puller.transform.position - transform.position;
		float toPullerDist = toPuller.magnitude;
		toPuller /= toPullerDist;
		bool blocked = TestForBlocking(toPuller, toPullerDist, out attemptPullHit, puller);
		if (blocked)
		{
			return;
		}

		Vector3 pullForce = toPuller * pullMagnitude;
		if (attachee != null && attachee.pullableBody != null)
		{
			attachee.AddPullForce(pullForce, transform.position);
		}
		else 
		{
			if (body != null)
			{
				body.isKinematic = false;
			}
			mover.Accelerate(pullForce * Time.deltaTime, false);
		}
	}

	public void Attach(GameObject attacheeObject, Vector3 position, Vector3 standDirection, bool sway = true)
	{
		// If already attached to a possessive attachee, do not attempt to attach.
		if (attachee != null && attacheePossessive)
		{
			return;
		}

		// If the attachee has a special way of attaching fluffs, use its method instead;
		PartnerLink fluffContainer = attacheeObject.GetComponent<PartnerLink>();
		if (fluffContainer != null)
		{
			fluffContainer.AttachFluff(this);
			return;
		}
		
		// Position and orient.
		transform.position = position;
		transform.up = standDirection;

		// Stop moving, and if desired, start swaying.
		mover.Stop();
		ToggleSwayAnimation(sway);
		
		// Halt physical interactions.
		if (body != null)
		{
			body.isKinematic = true;
		}
		hull.isTrigger = true;

		// Actaully attach to target.
		if (attacheeObject != null)
		{
			transform.parent = attacheeObject.transform;
			attachee = attacheeObject.GetComponent<FluffStick>();
		}
		moving = false;
		creator = null;
	}

	public void ToggleSwayAnimation(bool playSway)
	{
		if (swayAnimation != null)
		{
			swayAnimation.enabled = playSway;
			swayAnimation["Fluff_Sway"].time = 0;
		}
	}

	public bool TestForBlocking(Vector3 moveDirection, float testDistance, out RaycastHit blocker, GameObject whoWantsToKnow = null)
	{
		int fluffLayer = (int)Mathf.Pow(2, gameObject.layer);
		RaycastHit[] hits = Physics.RaycastAll((transform.position + bulb.transform.position) / 2, moveDirection, testDistance, ~fluffLayer);
		bool blocked = false;
		blocker = new RaycastHit();
		for (int j = 0; j < hits.Length && !blocked; j++)
		{
			bool hitIgnoredCollider = hits[j].collider.gameObject == ignoreCollider;
			bool hitTester = hits[j].collider.gameObject == whoWantsToKnow;
			bool layerIgnorable = Physics.GetIgnoreLayerCollision(gameObject.layer, hits[j].collider.gameObject.layer);
			blocked = !(hitIgnoredCollider || hitTester || layerIgnorable);
			if (blocked)
			{
				blocker = hits[j];
			}
		}
		return blocked;
	}

	void OnCollisionEnter(Collision collision)
	{
		// Attempt to attach to collided object.
		bool sameLayer = (collision.collider.gameObject.layer == gameObject.layer);
		bool alreadyAttachee = (attachee != null && collision.collider.gameObject == attachee.gameObject);
		bool shouldIgnore = collision.collider.gameObject == ignoreCollider;
		if (!(attacheePossessive || sameLayer || alreadyAttachee || shouldIgnore))
		{
			//PartnerLink fluffContainer = collision.collider.gameObject.GetComponent<PartnerLink>();
			//if (collision.collider.gameObject != ignoreCollider && !collision.collider.isTrigger && !attacheePossessive && fluffContainer != null)
			//{
			//	fluffContainer.AttachFluff(this);
			//}
			//else
			{
				Attach(collision.collider.gameObject, collision.contacts[0].point, collision.contacts[0].normal);
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		PartnerLink fluffContainer = other.GetComponent<PartnerLink>();
		if (fluffContainer != null && (attachee == null || attachee.gameObject != other.gameObject) && ignoreCollider != other.gameObject)
		{
			fluffContainer.AttachFluff(this);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (ignoreCollider == other.gameObject)
		{
			ignoreCollider = null;
		}
	}

	void OnDestroy()
	{
		if (attachee != null)
		{
			FluffSpawn attacheeFluffContainer = attachee.GetComponent<FluffSpawn>();
			if (attacheeFluffContainer != null)
			{
				attacheeFluffContainer.fluffs.Remove(this);
			}
		}
	}
}
