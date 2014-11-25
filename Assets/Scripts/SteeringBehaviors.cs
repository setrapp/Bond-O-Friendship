using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SteeringBehaviors : MonoBehaviour {
	public SimpleMover mover;
	public Vector3 desiredVelocity;
	public Vector3 steeringForce;
	/*TODO actual movement should not be done here.*/

	void Start()
	{
		if (mover == null)
		{
			mover = GetComponent<SimpleMover>();
		}
	}


	public void Seek(Vector3 seekTarget, bool arrive = false)
	{
		desiredVelocity = seekTarget - transform.position;
		if (!arrive)
		{
			desiredVelocity = desiredVelocity.normalized * mover.maxSpeed;
		}

		steeringForce = desiredVelocity - mover.velocity;

		mover.Accelerate(steeringForce, !arrive, false);
	}

	public void Flee(Vector3 fleeTarget)
	{
		if (transform.position == fleeTarget)
		{
			Seek(fleeTarget + -Vector3.right);
		}
		else
		{
			Seek(transform.position + ((transform.position - fleeTarget) * 2));
		}
	}

	public void Pursue(GameObject pursuee, bool arrive = false)
	{
		Pursue(pursuee, Vector3.zero, false, arrive);
	}

	public void Pursue(GameObject pursuee, float distance, bool acceptWithinProximity = true, bool arrive = false)
	{
		// Only seek if still far away || getting close is not good enough.
		Vector3 fromPursuee = transform.position - pursuee.transform.position;
		if (!acceptWithinProximity || fromPursuee.sqrMagnitude > Mathf.Pow(distance, 2))
		{
			Seek(pursuee.transform.position + (fromPursuee.normalized * distance), arrive);
		}
	}

	public void Pursue(GameObject pursuee, Vector3 offset, bool acceptWithinProximity = true, bool arrive = false)
	{
		Vector3 worldOffset = pursuee.transform.TransformDirection(offset);
		bool needSeek = true;

		// Only seek if still far away || getting close is not good enough.
		if (acceptWithinProximity)
		{
			Vector3 toPursuee = pursuee.transform.position - transform.position;
			if (toPursuee.sqrMagnitude <= worldOffset.sqrMagnitude && Vector3.Dot(-toPursuee, worldOffset) > 0)
			{
				needSeek = false;
			}
		}
		
		if (needSeek)
		{
			Seek(pursuee.transform.position + worldOffset, arrive);
		}
	}

	public bool AvoidObstacles(float checkDistance, float checkRadius)
	{
		if (checkDistance <= 0 || checkRadius <= 0)
		{
			return false;
		}

		if (checkDistance < checkRadius * 2)
		{
			checkDistance = checkRadius * 2;
		}

		bool avoiding = false;
		// Set origins for center of capsules defining spheres.
		Vector3 startPoint = transform.position + (transform.forward * checkRadius);
		Vector3 endPoint = (transform.position + (transform.forward * checkDistance)) - (transform.forward * checkRadius);
		//Debug.Log(startPoint + " " + endPoint + " " + (endPoint - startPoint));

		if (Physics.CheckCapsule(startPoint, endPoint, 0))
		{
			RaycastHit[] potentialHits = Physics.CapsuleCastAll(startPoint, endPoint, checkRadius, transform.forward, checkDistance);
			List<RaycastHit> obstacleHits = new List<RaycastHit>();
			for (int i = 0; i < potentialHits.Length; i++)
			{
				bool hitSelf = (potentialHits[i].collider.gameObject == gameObject);
				bool ignoreHit = Physics.GetIgnoreLayerCollision(gameObject.layer, potentialHits[i].collider.gameObject.layer);
				if (!(hitSelf || ignoreHit))
				{
					obstacleHits.Add(potentialHits[i]);
					//Debug.Log(potentialHits[i].collider.gameObject.name + " " + (potentialHits[i].point - transform.position).magnitude);
				}
			}

			for (int i = 0; i < obstacleHits.Count; i++)
			{
				Vector3 toObstacle = (obstacleHits[i].collider.gameObject.transform.position - transform.position);
				//Vector3 projToObstacle = Helper.ProjectVector(transform.right, -toObstacle);
				Vector3 steeringAdd = transform.right;// *Mathf.Max(1 - (toObstacle.magnitude / checkDistance), 0);
				if (Vector3.Dot(steeringAdd, toObstacle) > 0)
				{
					steeringAdd *= -1;
				}
				desiredVelocity += steeringAdd;
				//Debug.Log(obstacleHits[i].collider.gameObject.name + " steering: " + steeringAdd);
				avoiding = true;
			}
			//Debug.Log("-----");
		}
		steeringForce = desiredVelocity - mover.velocity;
		mover.Accelerate(steeringForce, false, true);
		return avoiding;
	}
}
