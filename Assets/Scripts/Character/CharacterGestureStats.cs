using UnityEngine;
using System.Collections;

public class CharacterGestureStats : MonoBehaviour {

	public BondAttachable bondAttachable;
	public float bondAttraction = 0;
	public float maxAttraction = 10;
	public float attractionChangeRate = 1;
	public float bondPullFactor = 1;
	public float bondPushFactor = 1;
	public float pushAwayDistanceMax = 1;

	void Start()
	{
		if (bondAttachable == null)
		{
			bondAttachable = GetComponent<BondAttachable>();
		}
	}
	
	public void PullBondIn(BondAttachable partner = null)
	{
		//bondAttraction = (bondAttachable.bondOverrideStats.stats.attachSpring1 + bondAttachable.bondOverrideStats.stats.attachSpring2) / 2;
		if (bondAttraction < maxAttraction)
		{
			bondAttraction += attractionChangeRate;
			if (bondAttraction > maxAttraction)
			{
				bondAttraction = maxAttraction;
			}
		}

		if (bondAttraction > 0)
		{
			UpdateBondToPartner(partner, bondAttraction * bondPullFactor);
		}
		else
		{
			UpdateBondToPartner(partner, -bondAttraction * bondPushFactor);
		}
	}

	public void PushBondOut(BondAttachable partner = null)
	{
		//bondAttraction = (bondAttachable.bondOverrideStats.stats.attachSpring1 + bondAttachable.bondOverrideStats.stats.attachSpring2) / 2;
		if (bondAttraction > -maxAttraction)
		{
			bondAttraction -= attractionChangeRate;
			if (bondAttraction < -maxAttraction)
			{
				bondAttraction = -maxAttraction;
			}
		}

		if (bondAttraction > 0)
		{
			UpdateBondToPartner(partner, bondAttraction * bondPullFactor);
		}
		else
		{
			UpdateBondToPartner(partner, -bondAttraction * bondPushFactor);
		}
	}

	private void UpdateBondToPartner(BondAttachable partner, float springForce)
	{
		bondAttachable.bondOverrideStats.stats.attachSpring1 = bondAttachable.bondOverrideStats.stats.attachSpring2 = springForce;

		if (bondAttraction < 0)
		{
			bondAttachable.bondOverrideStats.stats.pullApartMaxFactor = pushAwayDistanceMax * Mathf.Abs(bondAttraction / maxAttraction);
		}
		else
		{
			bondAttachable.bondOverrideStats.stats.pullApartMaxFactor = 0;
		}

		if (partner != null)
		{
			for (int i = 0; i < bondAttachable.bonds.Count; i++)
			{
				if (bondAttachable.bonds[i].OtherPartner(bondAttachable) == partner)
				{
					bondAttachable.bonds[i].stats.attachSpring1 = bondAttachable.bonds[i].stats.attachSpring2 = springForce;
					if (bondAttraction < 0)
					{
						bondAttachable.bonds[i].stats.pullApartMaxFactor = pushAwayDistanceMax * Mathf.Abs(bondAttraction / maxAttraction);
					}
					else
					{
						bondAttachable.bonds[i].stats.pullApartMaxFactor = 0;
					}
				}
			}
		}
	}
}
