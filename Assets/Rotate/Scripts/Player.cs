using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour 
{
	public Rigidbody2D rigidbody2d;
	public Collider2D collider2d;

	public Action onPlayerDeath;

	private bool checkForDeath;
	private Goal worldGoal;
	private Vector3 initialPos;
	private float initialGravity;

	void Awake()
	{
		worldGoal = transform.parent.GetComponentInChildren<Goal> ();

		initialPos = transform.position;

		initialGravity = rigidbody2d.gravityScale;

		collider2d = GetComponent<Collider2D> ();
	}

	void OnEnable()
	{
		checkForDeath = true;

		worldGoal.onGoalReached += StopCheckingForDeath;
	}

	void OnDisable()
	{
		worldGoal.onGoalReached -= StopCheckingForDeath;
	}

	void OnTriggerExit2D(Collider2D other) {
		if (checkForDeath 
			&& other.CompareTag (Tags.Bounds) && onPlayerDeath != null) {
			onPlayerDeath ();
		}
	}

	public void Disable()
	{
		rigidbody2d.isKinematic = true;
		rigidbody2d.velocity = Vector2.zero;
		rigidbody2d.rotation = 0f;
		transform.localRotation = Quaternion.identity;
		transform.position = initialPos;
		rigidbody2d.freezeRotation = true;
	}

	public void Reset()
	{
		rigidbody2d.rotation = 0f;
		transform.localRotation = Quaternion.identity;
		transform.localScale = Vector3.one;
		rigidbody2d.freezeRotation = false;
		rigidbody2d.gravityScale = initialGravity;
	}

	public void Enable()
	{
		rigidbody2d.isKinematic = false;
	}

	public void StartCheckingForDeath()
	{
		checkForDeath = true;
	}

	public void StopCheckingForDeath()
	{
		checkForDeath = false;	
	}
}
