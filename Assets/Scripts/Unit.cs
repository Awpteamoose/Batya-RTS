using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class Unit : NetworkBehaviour
{
	public float stunDuration;
	public string abilityChoice;
	public float abilityCooldown;
	public bool switchOff = false;
	public float maxFoodAmount = 100f;
	
	[ReadOnly] public bool selected;
	[ReadOnly] public bool fallen;
	[ReadOnly] public float foodAmount;

	[HideInInspector] public RTSController owner;

	private Collider helpCollider;
	private Collider trolleyCollider;
	private NavMeshAgent agent;
	private new Rigidbody rigidbody;

	private float standAfter;
	private List<Unit> adjacentBabushkas = new List<Unit>();
	private Ability ability;
	private float _abilityCooldown;

	void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		rigidbody = GetComponent<Rigidbody>();
	}

	void Start()
	{
		ability = Abilities.list[abilityChoice];
		helpCollider = transform.FindChild("Help").GetComponent<Collider>();
		trolleyCollider = transform.FindChild("Babywka").FindChild("Trolley").GetComponent<Collider>();
	}

	void Update()
	{
		if (!isServer) return;
		if (!fallen)
		{
			var dotResult = Vector3.Dot(agent.desiredVelocity.normalized, (transform.rotation * Vector3.forward).normalized);
			agent.speed = dotResult > 0.55f ? 4f : 1f;
		}
		else if(rigidbody.velocity.sqrMagnitude < 1f)
		{
			standAfter -= Time.deltaTime;
			foreach (var babushka in adjacentBabushkas)
			{
				if (babushka.owner == owner && !babushka.fallen)
					standAfter -= Time.deltaTime;
			}
			if (standAfter <= 0)
				Stand();
		}
		_abilityCooldown -= Time.deltaTime;
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
		transform.FindChild("Particles").gameObject.SetActive(true);
	}

	public void Deselect()
	{
		selected = true;
		transform.FindChild("Particles").gameObject.SetActive(false);
	}

	public void Fall()
	{
		standAfter = stunDuration;
		rigidbody.isKinematic = false;
		agent.enabled = false;
		fallen = true;
		helpCollider.enabled = true;
		trolleyCollider.enabled = true;
	}

	public void Stand()
	{
		foodAmount = 0f;
		rigidbody.isKinematic = true;
		agent.enabled = true;
		fallen = false;
		helpCollider.enabled = false;
		trolleyCollider.enabled = false;
		adjacentBabushkas.Clear();
	}
	
	public void Move(Vector3 destination)
	{
		if (!agent.enabled) return;
		agent.Resume();
		agent.SetDestination(destination);
	}

	public void Stop()
	{
		agent.Stop();
	}

	public void UseAbility(Vector3 position)
	{
		if (!fallen && (_abilityCooldown <= 0 || switchOff))
		{
			ability.Use(this, position);
			_abilityCooldown = abilityCooldown;
		}
	}
}
