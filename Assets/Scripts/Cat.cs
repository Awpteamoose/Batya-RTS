using UnityEngine;
using System.Collections;
using System.Threading;
using UnityEngine.Networking;

public class Cat : NetworkBehaviour
{
	[HideInInspector] public RTSController owner;
	private Coroutine destroyCoroutine;
	private bool used;

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
		clientRts.RpcGrantUnit(babushka.name);
		babushka.owner = clientRts;
	}

	void ClientToServer(Unit babushka, RTSController serverRts, RTSController clientRts)
	{
		// Take it from the client
		clientRts.ownedUnits.Remove(babushka);
		clientRts.RpcTakeUnit(babushka.name);

		// Give it to the server
		serverRts.ownedUnits.Add(babushka);
		babushka.owner = serverRts;
	}

	void TransferOwnership(Unit babushka, RTSController first, RTSController second)
	{
		if (babushka.owner == first)
		{
			if (first.enabled)
			{
				//Debug.Log("FIRST SERVER TO CLIENT");
				ServerToClient(babushka, first, second);
			}
			else
			{
				//Debug.Log("FIRST CLIENT TO SERVER");
				ClientToServer(babushka, second, first);
			}
		}
		else
		{
			if (second.enabled)
			{
				//Debug.Log("SECOND SERVER TO CLIENT");
				ServerToClient(babushka, second, first);
			}
			else
			{
				//Debug.Log("SECOND CLIENT TO SERVER");
				ClientToServer(babushka, first, second);
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (!isServer || !PlayerNetworkSetup.player2) return;
		var babushka = other.GetComponent<Unit>();
		if (babushka && babushka.owner != owner && !used)
		{
			var first = babushka.owner;
			var second = owner;
			TransferOwnership(babushka, first, second);
			babushka.After(5, () => TransferOwnership(babushka, first, second));
			//Debug.Log("BABUSHKA CAPTURED");
			this.Cancel(destroyCoroutine);
			Destroy(gameObject);
			used = true;
		}
	}
}
