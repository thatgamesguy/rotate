using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    private static LevelTimer timer;

    private Player player;
    private Transform entry;
    private RotateWorld rotation;
    private Teleport[] teleports;
    private ShrinkGrowOverTime[] shrinkingTiles;

    void Awake()
    {
        if (timer == null)
        {
            timer = FindObjectOfType<LevelTimer>();
        }

        player = GetComponentInChildren<Player>();

        foreach (Transform t in transform)
        {
            if (t.gameObject.CompareTag("Entry"))
            {
                entry = t;
                break;
            }
        }

        rotation = GetComponent<RotateWorld>();

        teleports = GetComponentsInChildren<Teleport>();

        shrinkingTiles = GetComponentsInChildren<ShrinkGrowOverTime>();
    }

    void OnEnable()
    {
        player.onPlayerDeath += StartWorld;
        timer.onTimerFinished += OnTimeUp;
    }

    void OnDisable()
    {
        player.onPlayerDeath -= StartWorld;
        timer.onTimerFinished -= OnTimeUp;
    }

    public void StartWorld()
    {
        timer.StopTimer();
        timer.ResetTimer();

        rotation.canRotate = false;

        rotation.Reset();

        player.Disable();

        entry.transform.position = player.transform.position;

        foreach (var s in shrinkingTiles)
        {
            s.Reset();
        }

        if (teleports != null && teleports.Length > 0)
        {
            foreach (var t in teleports)
            {
                t.Reset();
            }
        }

     
        StartCoroutine(ScaleTeleport());
        StartCoroutine(SpawnPlayer());
    }

    public float GetCompleteTime()
    {
        return (float)timer.GetTimeInSeconds();
    }

    public float GetTimeRemainder()
    {
        return timer.GetTimeRemainder();
    }

    private IEnumerator ScaleTeleport()
    {
        entry.transform.localScale = Vector3.zero;


        while (entry.transform.localScale.x < 1.4f)
        {
            entry.transform.localScale += Vector3.one * 4f * Time.deltaTime;
            yield return null;
        }

        while (entry.transform.localScale.x > 0f)
        {
            entry.transform.localScale -= Vector3.one * 3f * Time.deltaTime;
            yield return null;
        }

        entry.transform.localScale = Vector3.zero;
    }

    private IEnumerator SpawnPlayer()
    {
        player.transform.localScale = Vector3.zero;

        while (player.transform.localScale.x < 1f)
        {
            player.transform.Rotate(-Vector3.forward * 360f * Time.deltaTime);
            player.transform.localScale += Vector3.one * 2f * Time.deltaTime;
            yield return null;
        }

        player.Reset();

        yield return new WaitForSeconds(0.2f);

        player.Enable();

        timer.ResetTimer();
        timer.StartTimer();

        rotation.canRotate = true;

        foreach (var s in shrinkingTiles)
        {
            s.StartShrinking();
        }
    }

    private void OnTimeUp()
    {
        timer.StopTimer();
        timer.ResetTimer();
        player.onPlayerDeath();
    }


}
