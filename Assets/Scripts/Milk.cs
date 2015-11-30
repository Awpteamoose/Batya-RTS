using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Milk : NetworkBehaviour
{
	void OnCollisionEnter(Collision collision)
	{
		if (!isServer) return;
		if (collision.collider.tag == "Floor")
		{
			GetComponent<Rigidbody>().isKinematic = true;
			var collider = GetComponent<BoxCollider>();
			collider.isTrigger = true;
			collider.size = new Vector3(8, 1, 8);
			var newForward = transform.rotation * Vector3.forward;
			newForward.y = 0f;
			transform.rotation = Quaternion.LookRotation(newForward);
			this.After(10, () => Destroy(gameObject));
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (!isServer) return;
		var babushka = other.GetComponent<Unit>();
		if (babushka)
		{
			babushka.Fall();
			var rigidBabushka = babushka.GetComponent<Rigidbody>();
			rigidBabushka.AddForce(rigidBabushka.rotation * Vector3.forward * 8f, ForceMode.VelocityChange);
			var torque = rigidBabushka.rotation * (Random.value > 0.5f ? Vector3.up : Vector3.down);
			rigidBabushka.AddTorque(torque * 300f, ForceMode.VelocityChange);
		}
	}
}
