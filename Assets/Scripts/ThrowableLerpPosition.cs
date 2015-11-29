using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ThrowableLerpPosition : NetworkBehaviour
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
		Destroy(GetComponent<Rigidbody>());
		Destroy(GetComponent<Collider>());
	}

	public override int GetNetworkChannel()
	{
		return 1;
	}

	void FixedUpdate()
	{
		if (isServer)
		{
			if (rigidbody.isKinematic)
			{
				syncPosition = transform.position;
				syncRotation = transform.rotation;
			}
			else
			{
				syncPosition = rigidbody.position;
				syncRotation = rigidbody.rotation;
			}
		}
		else
		{
			transform.position = Vector3.Lerp(transform.position, syncPosition, Time.deltaTime * lerpRate);
			transform.rotation = Quaternion.Lerp(transform.rotation, syncRotation, Time.deltaTime * lerpRate);
		}
	}
}
