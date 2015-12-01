using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class PlayerNetworkSetup : NetworkBehaviour
{
	public static RTSController player1;
	public static RTSController player2;
	[SyncVar] public float team1score;
	[SyncVar] public float team2score;

	// Use this for initialization
	void Start () {
		var rtsController = GetComponent<RTSController>();

		if (isLocalPlayer)
		{
			// Setting up my own rts controller as a both server and a client
			rtsController.enabled = true;
			rtsController.selectionBox = GameObject.Find("SelectionBox").GetComponent<RectTransform>();
			rtsController.team1Score = GameObject.Find("Team1 Score").GetComponent<Text>();
			rtsController.team2Score = GameObject.Find("Team2 Score").GetComponent<Text>();

			var team = GameObject.FindGameObjectsWithTag(isServer ? "Team1" : "Team2");
			foreach (var unitGo in team)
			{
				var unit = unitGo.GetComponent<Unit>();
				rtsController.ownedUnits.Add(unit);
				unit.owner = rtsController;
			}

			if (isServer)
				player1 = rtsController;
		}
		else
		{
			if (isServer)
			{
				player2 = rtsController;
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
				player1 = rtsController;
			    var team1 = GameObject.FindGameObjectsWithTag("Team1");
				foreach (var unitGo in team1)
				{
					var unit = unitGo.GetComponent<Unit>();
					player1.ownedUnits.Add(unit);
					unit.owner = rtsController;
				}
			}
		}
	}

	void FixedUpdate()
	{
		if (!isServer || !player2) return;
		team1score = player1.score;
		team2score = player2.score;
	}
}
