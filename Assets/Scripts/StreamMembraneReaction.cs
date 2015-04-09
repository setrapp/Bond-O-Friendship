using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StreamMembraneReaction : StreamReaction {

	public MembraneWall membraneWall;
	private Membrane createdMembrane;

	void Awake()
	{
		membraneWall.createOnStart = false;
	}

	public override void React(float actionRate)
	{
		if (reactionProgress < 1)
		{
			base.React(actionRate);

			if (reactionProgress >= 1)
			{
				if (createdMembrane == null)
				{
					createdMembrane = membraneWall.CreateWall();
				}
			}
		}
	}
}
