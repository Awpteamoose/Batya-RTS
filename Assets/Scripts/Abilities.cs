using UnityEngine;
using System.Collections.Generic;

public class Abilities : MonoBehaviour
{
	public static Dictionary<string, Ability> list = new Dictionary<string, Ability>();
	public GameObject trolleyCharge;
	public GameObject milk;
	public GameObject cat;

	void Awake()
	{
		list["CatToss"] = new CatToss(cat);
		list["TrolleyCharge"] = new TrolleyCharge(trolleyCharge);
		list["MilkToss"] = new MilkToss(milk);
	}
}
