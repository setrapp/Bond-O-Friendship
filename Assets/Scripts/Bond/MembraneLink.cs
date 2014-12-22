﻿using UnityEngine;
using System.Collections;

public class MembraneLink : BondLink {
	public Membrane membrane;
	public BondAttachable bondAttachable;

	void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.tag == "Character")
		{
			BondAttachable partner = collision.collider.GetComponent<BondAttachable>();
			if (partner != null && !membrane.IsBondMade(partner))
			{
				bondAttachable.AttemptBond(partner, transform.position, true);
			}
		}
	}

	void AttachFluff(Fluff fluff)
	{
		if (fluff != null && fluff.moving && (fluff.attachee == null || fluff.attachee.gameObject == gameObject))
		{
			if (!membrane.IsBondMade(fluff.creator))
			{
				bondAttachable.AttemptBond(fluff.creator, transform.position, true);
			}
			fluff.PopFluff();
		}
	}
}
