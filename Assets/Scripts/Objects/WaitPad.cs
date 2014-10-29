﻿using UnityEngine;
using System.Collections;

public class WaitPad : MonoBehaviour {

	public bool pOonPad = false;
	public bool pTonPad = false;
	private Color mycolor;
	private float red;
	private float turnTime;

	// Use this for initialization
	void Start () {
		red = 0.1f;
		turnTime = 0.3f;
	}
	
	// Update is called once per frame
	void Update () {
		mycolor = new Color(red,0.3f,0.5f);
		renderer.material.color = mycolor;

	if(pOonPad == true && pTonPad == true)
		{
			//renderer.material.color = Color.magenta;
			if(red < 1.0f)
			red += Time.deltaTime*turnTime;
		}
		if(pOonPad == false || pTonPad == false)
		{
			if(red > 0.1f)
			red -= Time.deltaTime;
		}

	}
	void OnTriggerEnter(Collider collide)
	{
		if(collide.gameObject.name == "Player 1")
		{
			pOonPad = true;
			//print("1");
		}
		if(collide.gameObject.name == "Player 2")
		{
			pTonPad = true;
			//print ("2");
		}
	}
	void OnTriggerExit(Collider collide)
	{
		if(collide.gameObject.name == "Player 1")
		{
			pOonPad = false;
		}
		if(collide.gameObject.name == "Player 2")
		{
			pTonPad = false;
		}
	}
}