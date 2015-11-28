using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerLerpPosition : NetworkBehaviour
{
	[SyncVar]
	private Vector3 syncPosition;

	[SyncVar]
	private Quaternion syncRotation;

	public float lerpRate = 5;

	public override float GetNetworkSendInterval()
	{
		return 0.016f;
	}

	void FixedUpdate ()
	{
		TransmitPosition();
	}

	void Update()
	{
		LerpPosition();
	}

	void LerpPosition()
	{
		if (!isServer)
		{
			transform.position = Vector3.Lerp(transform.position, syncPosition, Time.deltaTime * lerpRate);
			transform.rotation = Quaternion.Lerp(transform.rotation, syncRotation, Time.deltaTime * lerpRate);
			Debug.DrawLine(transform.position, syncPosition, Color.red);
		}
	}

	//[Command]
	void CmdProvideTransform(Vector3 pos, Quaternion rot)
	{
		syncPosition = pos;
		syncRotation = rot;
	}

	[ClientCallback]
	void TransmitPosition()
	{
		if (isServer)
		{
			CmdProvideTransform(transform.position, transform.rotation);
		}
	}
}
