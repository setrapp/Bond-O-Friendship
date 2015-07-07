﻿using UnityEngine;
using System.Collections;

public class SeasonReaction : MonoBehaviour {

	public string managerSearchTag = "Island";
	public SeasonManager manager = null;
	protected SeasonManager.ActiveSeason season = SeasonManager.ActiveSeason.DRY;
	protected bool seasonChanged = false;


	protected virtual void Start()
	{
		// Find the season manager that controls this object by checking the transform parents.
		Transform parent = transform.parent;
		while(manager == null && parent != null)
		{
			if (parent.gameObject.tag == managerSearchTag)
			{
				manager = parent.GetComponent<SeasonManager>();
			}
			if (manager == null)
			{
				parent = parent.transform.parent;
			}
		}

		if (manager != null)
		{
			season = manager.activeSeason;
		}
		else
		{
			enabled = false;
		}
		seasonChanged = false;
	}

	protected virtual void Update()
	{
		seasonChanged = false;
		if (manager != null && season != manager.activeSeason)
		{
			season = manager.activeSeason;
			seasonChanged = true;
		}
	}
}