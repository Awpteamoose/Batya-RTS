using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerLerpPosition : NetworkBehaviour
{
	public float lerpRate = 5;

	[SyncVar] private Vector3 syncPosition;
	[SyncVar] private Quaternion syncRotation;

	private new Rigidbody rigidbody;

	void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
	}

	void Start()
	{
		if (isServer) return;
		Destroy(GetComponent<NavMeshAgent>());
	}

	public override int GetNetworkChannel()
	{
		return 1;
	}

	void Update()
	{
		if (isServer)
		{
			if (Vector3.Distance(syncPosition, transform.position) > 0.01f)
				syncPosition = transform.position;
			if (Quaternion.Angle(syncRotation, transform.rotation) > 2.5f)
				syncRotation = transform.rotation;
		}
		else
		{
			if (Vector3.Distance(transform.position, syncPosition) > 7.5f)
				transform.position = syncPosition;
			else
			{
				//transform.position = Vector3.Lerp(transform.position, syncPosition, Time.deltaTime * lerpRate);
				var extra = (syncPosition - transform.position);
				transform.position = Vector3.Lerp(transform.position, syncPosition + extra, Time.deltaTime * lerpRate);
			}
			transform.rotation = Quaternion.Lerp(transform.rotation, syncRotation, Time.deltaTime * lerpRate);
		}
	}
}
