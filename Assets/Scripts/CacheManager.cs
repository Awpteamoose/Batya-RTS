using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class CacheManager : NetworkBehaviour
{
	[ReadOnly] public static int totalCachesActive;
	public List<Cache> caches;
	public static List<Cache> disabledCaches = new List<Cache>(); 
	public float activationInterval = 15f;
	[SerializeField] private float timer = 5f;

	// Use this for initialization
	void Start ()
	{
		if (!isServer)
			Destroy(gameObject);
		foreach (var cache in caches)
		{
			disabledCaches.Add(cache);	
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (totalCachesActive < 2)
		{
			timer -= Time.deltaTime;
			if (timer <= 0 && disabledCaches.Count > 0)
			{
				var newCache = disabledCaches[Random.Range(0, disabledCaches.Count)];
				disabledCaches.Remove(newCache);
				newCache.enabled = true;
				timer = activationInterval;
				totalCachesActive += 1;
			}
		}
	}
}
