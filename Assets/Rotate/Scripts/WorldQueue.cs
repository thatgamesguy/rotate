using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldQueue : MonoBehaviour
{
    public World[] worlds;

    public World currentWorld { get { return worlds[currentWorldIndex]; } }

    private static readonly string worldIndexKey = "WorldIndex";

    private Leaderboard leaderboard;
    private int currentWorldIndex;

    void Awake()
    {
        leaderboard = FindObjectOfType<Leaderboard>();
    }

    void Start()
    {
        foreach (var world in worlds)
        {
            world.gameObject.SetActive(false);
        }

        currentWorldIndex = PlayerPrefs.GetInt(worldIndexKey, 0);

        if(currentWorldIndex > worlds.Length - 1)
        {
            currentWorldIndex = 0;
        }

        worlds[currentWorldIndex].GetComponentInChildren<Goal>().onWorldComplete += IncrementWorld;
        worlds[currentWorldIndex].gameObject.SetActive(true);
        worlds[currentWorldIndex].StartWorld();
    }

    private void IncrementWorld()
    {
        worlds[currentWorldIndex].GetComponentInChildren<Goal>().onWorldComplete -= IncrementWorld;
        worlds[currentWorldIndex].gameObject.SetActive(false);

        if (RecordCurrentWorldTime())
        {
            RecordRemainingTime();
        }

        currentWorldIndex = (currentWorldIndex + 1) % worlds.Length;

        if (currentWorldIndex == 0)
        {
            Debug.Log("All levels complete!");
            return;
        }

        PlayerPrefs.SetInt(worldIndexKey, currentWorldIndex);

        worlds[currentWorldIndex].GetComponentInChildren<Goal>().onWorldComplete += IncrementWorld;
        worlds[currentWorldIndex].gameObject.SetActive(true);
        worlds[currentWorldIndex].StartWorld();
    }

    private bool RecordCurrentWorldTime()
    {
        float roundTimeSecs = worlds[currentWorldIndex].GetCompleteTime();

        var currentRecord = leaderboard.GetToken(currentWorldIndex);

        currentRecord.UpdateTime(roundTimeSecs);

        if (currentRecord.hasBeenUpdated)
        {
            leaderboard.StoreToken(currentRecord);
            return true;
        }

        return false;
    }

    private void RecordRemainingTime()
    {
        float remainingTime = worlds[currentWorldIndex].GetTimeRemainder();

        print("Store remaining time of " + remainingTime);
    }

}
