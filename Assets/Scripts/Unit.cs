using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Unit : NetworkBehaviour
{
	public bool selected;
	[HideInInspector] public RTSController owner;

	private NavMeshAgent agent;
	private new Renderer renderer;
	private new Rigidbody rigidbody;

	void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		renderer = GetComponent<Renderer>();
	}

	public void Select()
	{
		selected = true;
		renderer.material.color = Color.red;
		renderer.material.SetColor("_SilhouetteColor", new Color(1f, 0f, 1f, 0.25f));
	}

	public void Deselect()
	{
		selected = true;
		renderer.material.color = Color.green;
		renderer.material.SetColor("_SilhouetteColor", new Color(0f, 1f, 1f, 0.25f));
	}

	public void Fall()
	{
		rigidbody.isKinematic = false;
		agent.enabled = false;
	}

	public void Stand()
	{
		rigidbody.isKinematic = true;
		agent.enabled = false;
	}
	
	[Command]
	public void CmdMove(Vector3 destination)
	{
		agent.Resume();
		agent.SetDestination(destination);
	}

	[Command]
	public void CmdStop()
	{
		agent.Stop();
	}
}
