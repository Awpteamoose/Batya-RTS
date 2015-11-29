using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MilkToss : Ability
{
	private GameObject milk;

	public MilkToss(GameObject milk)
	{
		this.milk = milk;
	}

	public override void Use(Unit self, Vector3 position)
	{
		var milkInstance = GameObject.Instantiate(milk).transform;
		NetworkServer.Spawn(milkInstance.gameObject);
		milkInstance.position = self.transform.position + Vector3.up * 3f;

		var milkRigid = milkInstance.GetComponent<Rigidbody>();
		var desiredVelocity = Utils.calculateBestThrowSpeed(milkRigid.position, position, 2f);
		if (desiredVelocity.magnitude > 10f)
			desiredVelocity = desiredVelocity.normalized * 10f;
		milkRigid.velocity = desiredVelocity;
	}
}
