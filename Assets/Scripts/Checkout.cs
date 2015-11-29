using UnityEngine;
using System.Collections;

public class Checkout : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
	{
		var babushka = other.GetComponent<Unit>();
		if (babushka && babushka.foodAmount > 0.1f)
		{
			Debug.Log("CACHED IN");
			babushka.owner.score += babushka.foodAmount;
			babushka.foodAmount = 0f;
		}
	}
}
