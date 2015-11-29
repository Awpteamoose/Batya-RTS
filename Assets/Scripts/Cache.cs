﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Cache : NetworkBehaviour
{
	private new Collider collider;
	public float maxFoodAmount = 200f;
	[ReadOnly] public float foodAmount;

	void Awake()
	{
		collider = GetComponent<Collider>();
	}

	void OnEnable()
	{
		collider.enabled = true;
		foodAmount = maxFoodAmount;
		RpcEnableVisual();
	}

	void OnDisable()
	{
		collider.enabled = false;
		CacheManager.disabledCaches.Add(this);
		CacheManager.totalCachesActive -= 1;
		RpcDisableVisual();
	}
	
	void OnTriggerStay(Collider other)
	{
		var babushka = other.GetComponent<Unit>();
		if (babushka && babushka.foodAmount < babushka.maxFoodAmount)
		{
			foodAmount -= Time.deltaTime * 20f;
			babushka.foodAmount += Time.deltaTime * 20f;
			if (foodAmount <= 0)
				enabled = false;
		}
	}

	void Update()
	{
		foodAmount -= Time.deltaTime * 3f;
		if (foodAmount <= 0)
			enabled = false;
	}

	[ClientRpc]
	void RpcEnableVisual()
	{
		GetComponentInChildren<Renderer>().material.color = Color.yellow;
	}

	[ClientRpc]
	void RpcDisableVisual()
	{
		GetComponentInChildren<Renderer>().material.color = Color.white;
	}
}
