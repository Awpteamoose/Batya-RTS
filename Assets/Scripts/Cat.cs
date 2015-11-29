using UnityEngine;
using System.Collections;
using System.Threading;

public class Cat : MonoBehaviour
{
	[HideInInspector] public RTSController owner;
	private Coroutine destroyCoroutine;

	void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.tag == "Floor")
		{
			GetComponent<Rigidbody>().isKinematic = true;
			var collider = GetComponent<BoxCollider>();
			collider.isTrigger = true;
			collider.size = new Vector3(10, 1, 10);
			var newForward = transform.rotation * Vector3.forward;
			newForward.y = 0f;
			transform.rotation = Quaternion.LookRotation(newForward);
			destroyCoroutine = this.After(10, () => Destroy(gameObject));
		}
	}

	void ServerToClient(Unit babushka, RTSController serverRts, RTSController clientRts)
	{
		// Take it from teh server
		serverRts.ownedUnits.Remove(babushka);
		serverRts.selectedUnits.Remove(babushka);
		babushka.Deselect();

		// Give it to the client
		clientRts.ownedUnits.Add(babushka);
		owner.RpcGrantUnit(babushka.name);
	}

	void ClientToServer(Unit babushka, RTSController serverRts, RTSController clientRts)
	{
		// Take it from the client
		clientRts.ownedUnits.Remove(babushka);
		clientRts.RpcTakeUnit(babushka.name);

		// Give it to the server
		serverRts.ownedUnits.Add(babushka);
	}

	void OnTriggerEnter(Collider other)
	{
		var babushka = other.GetComponent<Unit>();
		if (babushka && babushka.owner != owner)
		{
			if (babushka.owner.enabled)
			{
				Debug.Log("BABUSHKA OWNER IS SERVER");
				var server = babushka.owner;
				var client = owner;
				ServerToClient(babushka, server, client);
				server.After(1, () => Debug.Log("WOOOOT WOOT"));
				server.After(10, () => ClientToServer(babushka, server, client));
			}
			else
			{
				Debug.Log("BABUSHKA OWNER IS CLIENT");
				var client = babushka.owner;
				var server = owner;
				ClientToServer(babushka, server, client);
				server.After(1, () => Debug.Log("WOOOOT WORKS"));
				server.After(10, () => ServerToClient(babushka, server, client));
			}
			Debug.Log("BABUSHKA CAPTURED");
			this.Cancel(destroyCoroutine);
			Destroy(gameObject);
		}
	}
}
