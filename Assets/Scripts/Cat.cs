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
		var babushka = other.GetComponent<Unit>();
		if (babushka && babushka.owner != owner)
		{
			var first = babushka.owner;
			var second = owner;
			TransferOwnership(babushka, first, second);
			babushka.After(5, () => TransferOwnership(babushka, first, second));
			//Debug.Log("BABUSHKA CAPTURED");
			this.Cancel(destroyCoroutine);
			Destroy(gameObject);
		}
	}
}
