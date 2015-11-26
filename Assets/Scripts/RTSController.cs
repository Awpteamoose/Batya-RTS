using UnityEngine;
using System.Collections;

public class RTSController : MonoBehaviour
{
	public enum Team
	{
		Player1,
		Player2,
		Neutral
	}

	public Team team;
	public RectTransform selectionBox;
	public static Rect selection = new Rect(0, 0, 0, 0);
	public static Vector3 destination = Vector3.zero;
	private Vector3 startClick = -Vector3.one;

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			startClick = Input.mousePosition;
		}
		else if (Input.GetMouseButtonUp(0))
		{
			startClick = -Vector3.one;
			selectionBox.sizeDelta = Vector2.zero;
			selectionBox.position = Vector2.zero;
		}

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
		}

		if (Input.GetMouseButtonUp(1))
		{
			//calculate new destination
			RaycastHit hit;
			int layerMask = 1 << 8; // only hit the Walkable layer
			Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(r, out hit, Mathf.Infinity, layerMask))
			{
				destination = hit.point;
			}
		}
	}

	public static bool Selected(Vector3 point)
	{
		var camPos = Camera.main.WorldToScreenPoint(point);
		camPos.y = Screen.height - camPos.y;
		return selection.Contains(camPos);
	}
}
