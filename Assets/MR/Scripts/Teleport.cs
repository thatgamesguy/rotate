using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
	public Transform[] targets;

	private Player player;
	private bool stickPlayer = false;
	private int currentTargetIndex = 0;

	private Vector2 initialPos;
	private Vector2 initialScale;

	void Awake()
	{
		initialPos = transform.position;
		initialScale = transform.localScale;
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (stickPlayer || player != null) {
			return;
		}

		if (other.CompareTag (Tags.Player)) {

			player = other.GetComponent<Player> ();
		
			StartCoroutine (StartPlayerTransfer ());
		}

	}
		
	void Update ()
	{
		if (stickPlayer) {
			player.transform.position = transform.position;
			player.transform.rotation = transform.rotation;
			player.rigidbody2d.velocity = Vector2.zero;
			//player.rigidbody2d.rotation = 0f;
		}
	}

	public void Reset()
	{
		StopCoroutine (StartPlayerTransfer ());

		currentTargetIndex = 0;
		transform.position = initialPos;
		transform.localScale = initialScale;
		stickPlayer = false;
		player = null;
	}

	private IEnumerator StartPlayerTransfer ()
	{
		player.rigidbody2d.isKinematic = true;
		player.rigidbody2d.velocity = Vector2.zero;

		while (Vector2.Distance (transform.position, player.transform.position) > 0.1f) {
			var dir = (transform.position - player.transform.position).normalized;

			player.transform.position += dir * 3f * Time.deltaTime;

			yield return null;
		}

		yield return ScaleDown ();
	}

	private IEnumerator ScaleDown()
	{
		player.StopCheckingForDeath ();

		stickPlayer = true;

		while (transform.localScale.x < 1.4f) {
			transform.localScale += Vector3.one * 2f * Time.deltaTime;
			yield return null;
		}	

		while (transform.localScale.x > 0.4f) {
			transform.localScale -= Vector3.one * 4f * Time.deltaTime;
			player.transform.localScale -= Vector3.one * 2.8f * Time.deltaTime;
			yield return null;
		}

		yield return MoveToOtherTeleport ();
	}

	private IEnumerator MoveToOtherTeleport ()
	{
		player.collider2d.enabled = false;

		while (Vector2.Distance (transform.position, targets [currentTargetIndex].transform.position) > 0.05f) {
			var dir = (targets [currentTargetIndex].transform.position - transform.position).normalized;

			transform.position += dir * 5f * Time.deltaTime;

			yield return null;
		}

		transform.position = targets [currentTargetIndex].position;

		player.collider2d.enabled = true;

		yield return ScaleUp ();
	}

	private IEnumerator ScaleUp ()
	{
		while (transform.localScale.x < 1f) {
			transform.localScale += Vector3.one * 4f * Time.deltaTime;
			player.transform.localScale += Vector3.one * 2f * Time.deltaTime;
			yield return null;
		}	

		transform.localScale = Vector3.one;
		player.transform.localScale = Vector3.one;

		IncrementTarget ();

		player.rigidbody2d.isKinematic = false;
		player.StartCheckingForDeath ();
		stickPlayer = false;
		player = null;
	}

	private void IncrementTarget ()
	{
		currentTargetIndex = (currentTargetIndex + 1) % targets.Length;
	}


}
