using UnityEngine;
using System.Collections;

public class TrolleyChargeProjectile : MonoBehaviour
{
	public float speed;

	[HideInInspector] public Transform oldParent;
	private Unit myBabushka;

	void Start()
	{
		myBabushka = GetComponentInChildren<Unit>();
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		var forward = (transform.rotation * Vector3.forward).normalized;
		GetComponent<Rigidbody>().MovePosition(transform.position + forward * speed * Time.fixedDeltaTime);
	}

	void OnCollisionEnter(Collision collision)
	{
		var babushka = collision.collider.GetComponent<Unit>();
		if (!babushka && collision.collider.name == "Trolley")
			babushka = collision.collider.transform.parent.parent.GetComponent<Unit>();
		if (babushka)
		{
			babushka.Fall();
			var babushkaRigid = babushka.GetComponent<Rigidbody>();
			var normal = collision.contacts.Length > 0 ? collision.contacts[0].normal : collision.impulse.normalized;
			var flyDirection = (-normal + Vector3.up).normalized;
			babushkaRigid.AddForce(flyDirection * 650f, ForceMode.Impulse);
			babushkaRigid.AddTorque(Random.insideUnitSphere.normalized * 100f, ForceMode.Impulse);
		}
		else if (!collision.collider.CompareTag("Floor"))
		{
			myBabushka.transform.SetParent(oldParent);
			myBabushka.GetComponent<Collider>().enabled = true;
			myBabushka.Fall();

			var myBabushkaRigid = myBabushka.GetComponent<Rigidbody>();
			var normal = collision.contacts.Length > 0 ? collision.contacts[0].normal : collision.impulse.normalized;
			var flyDirection = (normal + Vector3.up).normalized;
			myBabushkaRigid.AddForce(flyDirection * 650f, ForceMode.Impulse);
			myBabushkaRigid.AddTorque(Random.insideUnitSphere.normalized * 100f, ForceMode.Impulse);

			Destroy(gameObject);
		}
	}
}
