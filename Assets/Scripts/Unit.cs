using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
	public bool selected;
	public RTSController controller;

	void Start()
	{
		controller.ownedUnits.Add(this);		
	}

	public void Select()
	{
		selected = true;
		GetComponent<Renderer>().material.color = Color.red;
		GetComponent<Renderer>().material.SetColor("_SilhouetteColor", Color.magenta);
	}

	public void Deselect()
	{
		selected = true;
		GetComponent<Renderer>().material.color = Color.green;
		GetComponent<Renderer>().material.SetColor("_SilhouetteColor", Color.cyan);
	}

	public void Move(Vector3 destination)
	{
		GetComponent<NavMeshAgent>().SetDestination(destination);
	}
}
