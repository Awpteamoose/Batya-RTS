using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CatToss : Ability
{
	private GameObject cat;

	public CatToss(GameObject cat)
	{
		this.cat = cat;
	}

	public override void Use(Unit self, Vector3 position)
	{
		var catInstance = GameObject.Instantiate(cat).transform;
		NetworkServer.Spawn(catInstance.gameObject);
		catInstance.position = self.transform.position + Vector3.up * 3f;

		catInstance.GetComponent<Cat>().owner = self.owner;

		var catRigid = catInstance.GetComponent<Rigidbody>();
		var desiredVelocity = Utils.calculateBestThrowSpeed(catRigid.position, position, 0.5f);
		if (desiredVelocity.magnitude > 40f)
			desiredVelocity = desiredVelocity.normalized * 40f;
		catRigid.velocity = desiredVelocity;
	}

}
