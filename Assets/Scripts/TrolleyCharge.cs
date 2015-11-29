using UnityEngine;
using System.Collections;
using UnityEngine.Networking.Match;

public class TrolleyCharge : Ability
{
	private GameObject trolleyCharge;

	public TrolleyCharge(GameObject trolleyCharge)
	{
		this.trolleyCharge = trolleyCharge;
	}

	public override void Use(Unit self, Vector3 position)
	{
		var parentTcp = self.GetComponentInParent<TrolleyChargeProjectile>();
		if (parentTcp)
		{
			self.transform.SetParent(parentTcp.oldParent);
			self.GetComponent<NavMeshAgent>().enabled = true;
			self.GetComponent<Collider>().enabled = true;
			GameObject.Destroy(parentTcp.gameObject);
			return;
		}
		var trolley = self.transform.Find("Babywka").Find("Trolley");
		var trolleyChargeInst = GameObject.Instantiate(trolleyCharge).transform;

		var projectile = trolleyChargeInst.GetComponent<TrolleyChargeProjectile>();
		projectile.oldParent = self.transform.parent;

		var selfPos = self.transform.position;
		selfPos.y = 0;
		position.y = 0;
		self.transform.rotation = Quaternion.LookRotation(position - selfPos);

		trolleyChargeInst.position = trolley.position;
		trolleyChargeInst.rotation = trolley.rotation;

		self.transform.SetParent(trolleyChargeInst);

		self.GetComponent<NavMeshAgent>().enabled = false;
		self.GetComponent<Collider>().enabled = false;
	}
}
