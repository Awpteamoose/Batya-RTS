using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Cache : NetworkBehaviour
{
	private new Collider collider;
	public float maxFoodAmount = 200f;
	[ReadOnly] public float foodAmount;
	private string style;

	void Awake()
	{
		collider = GetComponent<Collider>();
	}

	void OnEnable()
	{
		collider.enabled = true;
		foodAmount = maxFoodAmount;
		var styles = transform.Find("Styles");
		style = styles.GetChild(Random.Range(0, styles.childCount)).name;
		RpcEnableVisual(style);
	}

	void OnDisable()
	{
		collider.enabled = false;
		CacheManager.disabledCaches.Add(this);
		CacheManager.totalCachesActive -= 1;
		RpcDisableVisual(style);
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
	void RpcEnableVisual(string style)
	{
		transform.Find("Styles").Find(style).gameObject.SetActive(true);
		transform.Find("Particles").gameObject.SetActive(true);
	}

	[ClientRpc]
	void RpcDisableVisual(string style)
	{
		transform.Find("Styles").Find(style).gameObject.SetActive(false);
		transform.Find("Particles").gameObject.SetActive(false);
	}
}
