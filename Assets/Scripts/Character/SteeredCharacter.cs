using UnityEngine;
using System.Collections;

public class SteeredCharacter : MonoBehaviour {
	public bool drawLines = false;
	public GameObject targetObject;
	public Vector3 targetPoint;
	public float targetDistance;
	public float slowDistance;
	public float avoidDistance;
	public float avoidRadius;
	public bool seeking;
	public bool arrive;
	public bool pointPursuit;
	public bool precisePursuit;
	public SteeringBehaviors steering;
	
	void Start()
	{
		if (steering == null)
		{
			steering = GetComponent<SteeringBehaviors>();
		}
	}

	void Update()
	{
		// Obstacle Avoidance
		bool avoiding = false;
		if (steering.mover.velocity.sqrMagnitude > 0)
		{
			avoiding = steering.AvoidObstacles(avoidDistance, avoidRadius);
		}

		// Seeking
		if (!avoiding)
		{
			if (steering != null)
			{
				if (seeking)
				{
					if (targetObject != null)
					{
						if (pointPursuit)
						{
							steering.Pursue(targetObject, targetPoint, !precisePursuit, arrive);
						}
						else
						{
							steering.Pursue(targetObject, targetDistance, !precisePursuit, arrive);
						}
					}
					else
					{
						steering.Seek(targetPoint, arrive);
					}
				}
				else
				{
					steering.Flee(targetPoint);
				}
			}
		}

		transform.LookAt(transform.position + steering.mover.velocity, transform.up);
	}

	void OnDrawGizmos()
	{
		if (drawLines && steering != null && steering.mover != null)
		{
			// Velocity
			Gizmos.color = Color.white;
			Gizmos.DrawLine(transform.position, transform.position + steering.mover.velocity);

			// Desired velocity
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, transform.position + steering.desiredVelocity);

			// Steering force
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(transform.position, transform.position + steering.steeringForce);

			// Obstacle Checking.
			Vector3 avoidStart = transform.position + (transform.forward * avoidRadius);
			Vector3 avoidEnd = (transform.position + (transform.forward * avoidDistance)) -(transform.forward * avoidRadius);
			Vector3 avoidOut = transform.right * avoidRadius;
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(avoidStart, avoidRadius);
			Gizmos.DrawWireSphere(avoidEnd, avoidRadius);
			Gizmos.DrawLine(avoidStart + avoidOut, avoidEnd + avoidOut);
			Gizmos.DrawLine(avoidStart - avoidOut, avoidEnd - avoidOut);
		}
	}
}
