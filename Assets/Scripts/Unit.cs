using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class Unit : NetworkBehaviour
{
	public float stunDuration;
	public string abilityChoice;

	[ReadOnly] public bool selected;
	[ReadOnly] public bool fallen;

	[HideInInspector] public RTSController owner;

	private Collider helpCollider;
	private NavMeshAgent agent;
	private new Renderer renderer;
	private new Rigidbody rigidbody;

	private float standAfter;
	private List<Unit> adjacentBabushkas = new List<Unit>();
	private Ability ability;

	void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		renderer = GetComponent<Renderer>();
		rigidbody = GetComponent<Rigidbody>();
	}

	void Start()
	{
		ability = Abilities.list[abilityChoice];
		helpCollider = transform.FindChild("Help").GetComponent<Collider>();
	}

	void Update()
	{
		if (!isServer) return;
		if (!fallen)
		{
			var dotResult = Vector3.Dot(agent.desiredVelocity.normalized, (transform.rotation * Vector3.forward).normalized);
			agent.speed = dotResult > 0.55f ? 4f : 1f;
		}
		else
		{
			standAfter -= Time.deltaTime;
			foreach (var babushka in adjacentBabushkas)
			{
				if (babushka.owner == owner && !babushka.fallen)
					standAfter -= Time.deltaTime;
			}
			if (standAfter <= 0)
				CmdStand();
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (!isServer) return;
		var babushka = other.GetComponent<Unit>();
		if (babushka)
		{
			adjacentBabushkas.Add(babushka);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (!isServer) return;
		var babushka = other.GetComponent<Unit>();
		if (babushka && babushka.owner == owner)
		{
			adjacentBabushkas.Remove(babushka);
		}
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

	[Command]
	public void CmdFall(Vector3 direction)
	{
		standAfter = stunDuration;
		rigidbody.isKinematic = false;
		agent.enabled = false;
		rigidbody.AddForce(direction.normalized * 10, ForceMode.Impulse);
		fallen = true;
		helpCollider.enabled = true;
	}

	[Command]
	public void CmdStand()
	{
		rigidbody.isKinematic = true;
		agent.enabled = true;
		fallen = false;
		helpCollider.enabled = false;
		adjacentBabushkas.Clear();
	}
	
	[Command]
	public void CmdMove(Vector3 destination)
	{
		if (!agent.enabled) return;
		agent.Resume();
		agent.SetDestination(destination);
		//this.After(1, () => CmdFall(Vector3.left));
	}

	[Command]
	public void CmdStop()
	{
		agent.Stop();
	}

	[Command]
	public void CmdUseAbility(Vector3 position)
	{
		if (!fallen)
			ability.Use(position);
	}
}
