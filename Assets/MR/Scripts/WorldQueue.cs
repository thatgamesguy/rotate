using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldQueue : MonoBehaviour 
{
	public World[] worlds;

	private static readonly string worldIndexKey = "WorldIndex";

	private Leaderboard leaderboard;
	private int currentWorld;

	void Awake()
	{
		leaderboard = FindObjectOfType<Leaderboard> ();
	}

	void Start () 
	{
		foreach (var world in worlds) {
			world.gameObject.SetActive (false);
		}
		
		currentWorld = PlayerPrefs.GetInt (worldIndexKey, 0);

		worlds [currentWorld].GetComponentInChildren<Goal> ().onWorldComplete += IncrementWorld;
		worlds [currentWorld].gameObject.SetActive (true);
		worlds [currentWorld].StartWorld ();
	}

	private void IncrementWorld()
	{
		worlds [currentWorld].GetComponentInChildren<Goal> ().onWorldComplete -= IncrementWorld;
		worlds [currentWorld].gameObject.SetActive (false);

		if (RecordCurrentWorldTime ()) {
			RecordRemainingTime ();
		}

		currentWorld = (currentWorld + 1) % worlds.Length;

		if (currentWorld == 0) {
			Debug.Log ("All levels complete!");
			return;
		}

		worlds [currentWorld].GetComponentInChildren<Goal> ().onWorldComplete += IncrementWorld;
		worlds [currentWorld].gameObject.SetActive (true);
		worlds [currentWorld].StartWorld ();
	}

	private bool RecordCurrentWorldTime()
	{
		float roundTimeSecs = worlds [currentWorld].GetCompleteTime ();

		var currentRecord = leaderboard.GetToken (currentWorld);

		currentRecord.UpdateTime (roundTimeSecs);

		if (currentRecord.hasBeenUpdated) {
			leaderboard.StoreToken (currentRecord);
			return true;
		}

		return false;
	}

	private void RecordRemainingTime()
	{
		float remainingTime = worlds [currentWorld].GetTimeRemainder ();

		print ("Store remaining time of " + remainingTime);
	}
		
}
