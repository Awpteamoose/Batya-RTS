using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
	public bool selected;
	public RTSController.Team team;

	void Update()
	{
		if (GetComponent<Renderer>().isVisible && Input.GetMouseButton(0))
		{
			selected = RTSController.Selected(transform.position);
		}

		if (selected)
		{
			GetComponent<Renderer>().material.color = Color.red;
			GetComponent<Renderer>().material.SetColor("_SilhouetteColor", Color.magenta);
			if (Input.GetMouseButtonUp(1))
			{
				// right mouse released - move
				GetComponent<NavMeshAgent>().SetDestination(RTSController.destination);
			}
		}
		else
		{
			GetComponent<Renderer>().material.color = Color.green;
			GetComponent<Renderer>().material.SetColor("_SilhouetteColor", Color.cyan);
		}

	}

	void OnMouseUp()
	{
		selected = true;
	}
}
