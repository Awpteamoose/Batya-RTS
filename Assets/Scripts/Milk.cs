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
			GetComponent<BoxCollider>().enabled = false;
			GetComponent<SphereCollider>().enabled = true;
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
		if (babushka && !babushka.fallen)
		{
			babushka.Fall();
			var rigidBabushka = babushka.GetComponent<Rigidbody>();
			var myPosProj = transform.position;
			myPosProj.y = 0;
			var rigidBabushkaPosProj = rigidBabushka.transform.position;
			rigidBabushkaPosProj.y = 0;
			var slipDir = ((myPosProj - rigidBabushkaPosProj).normalized + Vector3.up).normalized;
			rigidBabushka.AddForce(slipDir * 10f, ForceMode.VelocityChange);
			var torque = rigidBabushka.rotation * Random.insideUnitSphere.normalized;
			rigidBabushka.AddTorque(torque * 300f, ForceMode.VelocityChange);
		}
	}
}
