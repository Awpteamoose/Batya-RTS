﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RTSController : MonoBehaviour
{
	public RectTransform selectionBox;

	public List<Unit> ownedUnits;
	[HideInInspector] public List<Unit> selectedUnits;

	private Vector3 startClick = -Vector3.one;
	private Rect selection = new Rect(0, 0, 0, 0);

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			// Deselect units and start selection
			startClick = Input.mousePosition;
			foreach (var unit in selectedUnits)
				unit.Deselect();
			selectedUnits.Clear();
		}
		else if (Input.GetMouseButtonUp(0))
		{
			// End selection and select unit under the mouse (if any)
			startClick = -Vector3.one;
			selectionBox.sizeDelta = Vector2.zero;
			selectionBox.position = Vector2.zero;

			RaycastHit hit;
			const int layerMask = 1 << 9; // only hit the Units layer
			Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(r, out hit, Mathf.Infinity, layerMask))
			{
				var unit = hit.transform.GetComponent<Unit>();
				if (ownedUnits.Contains(unit) && !selectedUnits.Contains(unit))
				{
					unit.Select();
					selectedUnits.Add(unit);
				}
			}
		}

		// Draggin the selection and updating it
		if (Input.GetMouseButton(0))
		{
			selection = new Rect(
				startClick.x, //x
				Screen.height - startClick.y, //y
				Input.mousePosition.x - startClick.x, //width
				startClick.y - Input.mousePosition.y //height
			);

			if (selection.width < 0)
			{
				selection.x += selection.width;
				selection.width = -selection.width;
			}
			if (selection.height < 0)
			{
				selection.y += selection.height;
				selection.height = -selection.height;
			}

			selectionBox.position = new Vector2(selection.x, Screen.height - (selection.y + selection.height));
			selectionBox.sizeDelta = new Vector2(selection.width, selection.height);

			// The selecting the units bit
			// No point checking units we don't own
			foreach (var unit in ownedUnits)
			{
				//if (!unit.GetComponent<Renderer>().isVisible) continue;
				var camPos = Camera.main.WorldToScreenPoint(unit.transform.position);
				camPos.y = Screen.height - camPos.y;
				if (!selection.Contains(camPos))
				{
					if (unit.selected)
					{
						unit.Deselect();
						selectedUnits.Remove(unit);
					}
					continue;
				}
				else if (!selectedUnits.Contains(unit))
				{
					unit.Select();
					selectedUnits.Add(unit);
				}
			}
		}

		if (Input.GetMouseButtonUp(1))
		{
			//calculate new destination
			RaycastHit hit;
			const int layerMask = 1 << 8; // only hit the Walkable layer
			Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(r, out hit, Mathf.Infinity, layerMask))
			{
				this.DrawSphere(hit.point);

				foreach (var unit in selectedUnits)
					unit.CmdMove(hit.point);
			}
		}

		if (Input.GetKeyDown(KeyCode.S))
		{
			foreach (var unit in selectedUnits)
				unit.CmdStop();
		}

		if (Input.GetKeyDown(KeyCode.A))
		{
			foreach (var unit in selectedUnits)
				unit.CmdUseAbility(Input.mousePosition);
		}

		//var cameraPos = Camera.main.transform.position;
		//var step = 50 * Time.deltaTime;
		//if (Input.mousePosition.x < 1f)
		//	cameraPos.x -= step;
		//else if (Input.mousePosition.x > Screen.width - 1f)
		//	cameraPos.x += step;
		//if (Input.mousePosition.y < 1f)
		//	cameraPos.z -= step;
		//else if (Input.mousePosition.y > Screen.height - 1f)
		//	cameraPos.z += step;

		//Camera.main.transform.position = cameraPos;
	}

}
