using UnityEngine;
using System.Collections;

public abstract class Ability
{
	public abstract void Use(Unit self, Vector3 position);
}
