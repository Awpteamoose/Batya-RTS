using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerLerpPosition : NetworkBehaviour
{
	public float lerpRate = 5;

	private Vector3 syncPosition;
	private Quaternion syncRotation;

	private new Rigidbody rigidbody;

	void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
	}

	void Start()
	{
		if (isServer) return;
		Destroy(GetComponent<NavMeshAgent>());
		Destroy(GetComponent<Rigidbody>());
		Destroy(GetComponent<Collider>());
	}

	//public override float GetNetworkSendInterval()
	//{
	//	return 0.016f;
	//}

	public override int GetNetworkChannel()
	{
		return 1;
	}

	void FixedUpdate()
	{
		if (isServer)
			RpcTransmitPosition(rigidbody.position, rigidbody.rotation);
		else
			LerpPosition();
	}

	void LerpPosition()
	{
		transform.position = Vector3.Lerp(transform.position, syncPosition, Time.deltaTime * lerpRate);
		transform.rotation = Quaternion.Lerp(transform.rotation, syncRotation, Time.deltaTime * lerpRate);
		Debug.DrawLine(transform.position, syncPosition, Color.red);
	}

	[ClientRpc]
	void RpcTransmitPosition(Vector3 position, Quaternion rotation)
	{
		syncPosition = position;
		syncRotation = rotation;
	}
}
