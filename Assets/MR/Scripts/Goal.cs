using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
	//public float minAttractionDist = 0.5f;
	public float attractionForce = 10f;

	[Header ("Goal")]
	public float goalScaleToSize = 1.4f;
	public float goalScaleUpSpeed = 5f;
	public float goalScaleDownSpeed = 1.4f;

	[Header ("Player")]
	public Player player;
	public float playerScaleSpeed = 2f;
	public float playerRotateSpeed = 360f;

	public Action onGoalReached;
	public Action onWorldComplete;

	private static LevelTimer levelTimer;


	private bool startedScaling = false;
	private bool reachedByPlayer = false;

	void Awake ()
	{
		if (player == null) {
			player = transform.parent.GetComponentInChildren<Player> ();
		}

		if (levelTimer == null) {
			levelTimer = FindObjectOfType<LevelTimer> ();
		}
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.CompareTag (Tags.Player)) {
			reachedByPlayer = true;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (startedScaling) {
			player.transform.position = transform.position;
			return;
		} else if (reachedByPlayer) {

			Vector2 to = transform.position - player.transform.position;
			float dist = to.magnitude;
			Vector3 heading = to / dist;
		
			player.rigidbody2d.isKinematic = true;
			player.rigidbody2d.velocity = Vector2.zero;

			if (!startedScaling && dist < 0.15f) {

				levelTimer.StopTimer ();

				if (onGoalReached != null) {
					onGoalReached ();
				}
					


				startedScaling = true;
				StartCoroutine (ScaleGoal ());
				StartCoroutine (ScalePlayer ());
			} else {
				player.transform.position += heading * attractionForce * Time.deltaTime;
			}

		}
	}

	private IEnumerator ScalePlayer ()
	{
		while (player.transform.localScale.x > 0f) {
			player.transform.Rotate (Vector3.forward * playerRotateSpeed * Time.deltaTime);
			player.transform.localScale -= Vector3.one * playerScaleSpeed * Time.deltaTime;
			yield return null;
		}

		if (onWorldComplete != null) {
			onWorldComplete ();
		}
	}

	private IEnumerator ScaleGoal ()
	{
		while (transform.localScale.x < goalScaleToSize) {
			transform.localScale += Vector3.one * goalScaleUpSpeed * Time.deltaTime;
			yield return null;
		}

		while (transform.localScale.x > 0f) {
			transform.localScale -= Vector3.one * playerScaleSpeed * goalScaleDownSpeed * Time.deltaTime;
			yield return null;
		}
	}
}