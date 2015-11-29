using System;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;

public static class Utils
{
	public static IEnumerator After(float time, Action callback)
	{
	   yield return new WaitForSeconds(time);
	   callback();
	}

	public static IEnumerator DoFor(float time, Action process, Action after = null)
	{
		while (time > 0)
		{
			yield return new WaitForEndOfFrame();
			process();
			time -= Time.deltaTime;
		}
		if (after != null) after();
	}

	public static Coroutine After(this MonoBehaviour caller, float time, Action callback)
	{
		return caller.StartCoroutine(After(time, callback));
	}

	public static Coroutine DoFor(this MonoBehaviour caller, float time, Action process, Action after = null)
	{
		return caller.StartCoroutine(DoFor(time, process, after));
	}

	public static void Cancel(this MonoBehaviour caller, Coroutine co)
	{
		caller.StopCoroutine(co);
	}

	public static GameObject DebugSphere(Vector3 position, float radius, Color color)
	{
		var s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		s.transform.position = position;
		s.transform.localScale = new Vector3(radius, radius, radius);
		s.GetComponent<Renderer>().material.color = color;
		Object.Destroy(s.GetComponent<Collider>());
		return s;
	}

	public static void DrawSphere(this MonoBehaviour caller, Vector3 position, float time = 1, float radius = 1f, Color color = new Color())
	{
		if (color.r <= 0.1 && color.g <= 0.1 && color.b <= 0.1 && color.a <= 0.1) // new color
			color = Color.green;

		var s = DebugSphere(position, radius, color);
		caller.After(time, () =>
		{
			Object.Destroy(s);
		});
	}
}
