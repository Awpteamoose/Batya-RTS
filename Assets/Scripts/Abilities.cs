using UnityEngine;
using System.Collections.Generic;

public class Abilities : MonoBehaviour
{
	public static Dictionary<string, Ability> list = new Dictionary<string, Ability>();
	public GameObject trolleyCharge;

	void Awake()
	{
		list["CatToss"] = new CatToss();
		list["TrolleyCharge"] = new TrolleyCharge(trolleyCharge);
		list["MilkToss"] = new MilkToss();
	}
}
