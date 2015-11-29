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
	private Collider trolleyCollider;
	private NavMeshAgent agent;
	private new Renderer renderer;
	private new Rigidbody rigidbody;

	private float standAfter;
	private List<Unit> adjacentBabushkas = new List<Unit>();
	private Ability ability;
	private bool canStand;

	void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		renderer = GetComponent<Renderer>();
		rigidbody = GetComponent<Rigidbody>();
		Units.list[name] = this;
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
		else
		{
			standAfter -= Time.deltaTime;
			foreach (var babushka in adjacentBabushkas)
			{
				if (babushka.owner == owner && !babushka.fallen)
					standAfter -= Time.deltaTime;
			}
			if (standAfter <= 0 && canStand)
				Stand();
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

	void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.tag == "Floor")
		{
			canStand = true;
		}
	}

	void OnCollisionExit(Collision collision)
	{
		if (collision.transform.tag == "Floor")
		{
			canStand = false;
		}
	}

	public void Select()
	{
		selected = true;
		renderer.enabled = true;
		renderer.material.color = Color.red;
		renderer.material.SetColor("_SilhouetteColor", new Color(1f, 0f, 1f, 0.25f));
	}

	public void Deselect()
	{
		selected = true;
		renderer.enabled = false;
		renderer.material.color = Color.green;
		renderer.material.SetColor("_SilhouetteColor", new Color(0f, 1f, 1f, 0.25f));
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
		this.DrawSphere(position);
		if (!fallen)
			ability.Use(this, position);
	}
}
