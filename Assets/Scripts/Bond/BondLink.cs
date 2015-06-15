﻿using UnityEngine;
using System.Collections;

public class BondLink : MonoBehaviour {
	public Bond bond;
	public BoxCollider toPreviousCollider = null;
	public BoxCollider toNextCollider = null;
	public Rigidbody body = null;
	public BondLink linkNext = null;
	public BondLink linkPrevious = null;
	public ConfigurableJoint jointToNeighbor = null;
	public ConfigurableJoint jointToAttachment = null;
	public int orderLevel = 0;
	public bool broken = false;

	void OnDestroy()
	{
		broken = true;
	}
}

public class BondLinkContainer
{
	public BondLink link = null;
}
