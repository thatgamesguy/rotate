using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
	private static readonly string levelTimePreText = "LevelTime_";
	private static readonly int noScoreRecorded = -1;

	public void StoreToken (LeaderboardToken token)
	{
		if (token.isInvalid) {
			return;
		}

		print ("New Record, level " + token.levelIndex 
			+ " completed in: " + token.timeInSecs.ToString ());
		PlayerPrefs.SetFloat (token.key, token.timeInSecs);
	}

	public LeaderboardToken GetToken(int levelIndex)
	{
		float current = GetRecordedTime (levelIndex);

		if (current == noScoreRecorded) {
			return LeaderboardToken.Invalid (levelIndex);
		}

		return LeaderboardToken.WithData (levelIndex, current);
	}

	private float GetRecordedTime (int levelIndex)
	{
		return PlayerPrefs.GetFloat (levelTimePreText + levelIndex, noScoreRecorded);
	}

	public class LeaderboardToken
	{
		public int levelIndex { get;  private set; }
		public float timeInSecs { get; private set; }
		public bool hasBeenUpdated { get; private set; }
		public string key { get { return levelTimePreText + levelIndex; } }

		public bool isInvalid { get; private set; }

		private LeaderboardToken (int levelIndex, float timeInSecs)
		{
			this.levelIndex = levelIndex;
			this.timeInSecs = timeInSecs;
			hasBeenUpdated = false;
			isInvalid = false;
		}

		private LeaderboardToken(int levelIndex)
		{
			this.levelIndex = levelIndex;
			hasBeenUpdated = false;
			this.isInvalid = true;
		}

		public static LeaderboardToken WithData (int levelIndex, float timeInSecs)
		{
			return new LeaderboardToken (levelIndex, timeInSecs);
		}

		public static LeaderboardToken Invalid (int levelIndex)
		{
			return new LeaderboardToken (levelIndex);
		}

		public void UpdateTime(float timeInSecs)
		{
			if (isInvalid || this.timeInSecs > timeInSecs) {
				this.timeInSecs = timeInSecs;
				hasBeenUpdated = true;
				isInvalid = false;
			}
		}
	}

}

