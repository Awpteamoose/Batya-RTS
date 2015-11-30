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
		var milkInstance = GameObject.Instantiate(milk, self.transform.position + Vector3.up * 3f, Quaternion.identity) as GameObject;

		var milkRigid = milkInstance.GetComponent<Rigidbody>();
		var desiredVelocity = Utils.calculateBestThrowSpeed(milkRigid.position, position, 2f);
		if (desiredVelocity.magnitude > 10f)
			desiredVelocity = desiredVelocity.normalized * 10f;
		milkRigid.velocity = desiredVelocity;
		milkRigid.AddTorque(Random.insideUnitSphere.normalized * 5f, ForceMode.VelocityChange);
		NetworkServer.Spawn(milkInstance.gameObject);
	}
}
