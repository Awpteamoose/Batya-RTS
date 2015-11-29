using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerNetworkSetup : NetworkBehaviour
{
	// Use this for initialization
	void Start () {
		var rtsController = GetComponent<RTSController>();

		if (isLocalPlayer)
		{
			// Setting up my own rts controller as a both server and a client
			rtsController.enabled = true;
			rtsController.selectionBox = GameObject.Find("SelectionBox").GetComponent<RectTransform>();

			var team = GameObject.FindGameObjectsWithTag(isServer ? "Team1" : "Team2");
			foreach (var unitGo in team)
			{
				var unit = unitGo.GetComponent<Unit>();
				rtsController.ownedUnits.Add(unit);
				unit.owner = rtsController;
			}
		}
		else
		{
			if (isServer)
			{
				//Setting up other rts controller as a server
			    var team2 = GameObject.FindGameObjectsWithTag("Team2");
				foreach (var unitGo in team2)
				{
					var unit = unitGo.GetComponent<Unit>();
					rtsController.ownedUnits.Add(unit);
					unit.owner = rtsController;
				}
			}
			else
			{
				// Setting up other rts controller as a client
			}
		}
	}
}
