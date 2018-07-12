using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTimer : MonoBehaviour
{
    public Text text;
    public float timerStart = 10f;

    public Action onTimerFinished;

    private string emptyTime;
    private float currentTime = 0f;
    private bool shouldUpdate = false;
    private TimeSpan timeSpan;

    // Use this for initialization
    void Start()
    {
        timeSpan = TimeSpan.FromSeconds(timerStart);
        UpdateTimeUI();
        emptyTime = text.text;
    }

	public void StartTimer()
    {
        shouldUpdate = true;
    }

    public void ResetTimer()
    {
        currentTime = timerStart;
        text.text = emptyTime;
    }

    public void StopTimer()
    {
        shouldUpdate = false;
    }

    public double GetTimeInSeconds()
    {
        return timerStart - timeSpan.TotalSeconds;
    }

    public float GetTimeRemainder()
    {
        return timerStart - (float)GetTimeInSeconds();
    }

    void Update()
    {
        if (!shouldUpdate)
        {
            return;
        }

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            if (onTimerFinished != null)
            {
                onTimerFinished();
            }

            print("Time Up");

            currentTime = 0f;
            shouldUpdate = false;
        }

        timeSpan = TimeSpan.FromSeconds(currentTime);

        UpdateTimeUI();

    }

    private void UpdateTimeUI()
    {
        text.text = string.Format("{0:00}:{1:00}", timeSpan.Seconds, timeSpan.Milliseconds / 10);
    }
}
