using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWorld : MonoBehaviour
{
	public bool worldRotatable = true;
	
	private bool rotationActive = false;
	public bool canRotate { 
		set {
			rotationActive = value;

			if (sections != null && sections.Length > 0) {
				foreach (var s in sections) {
					s.canRotate = value;
				}
			}
		}
	}

	private static readonly float ROTATE_SPEED = 120f;

	private RotateSection[] sections;

	void Awake()
	{
		sections = GetComponentsInChildren<RotateSection> ();
	}

	public void Reset()
	{
		transform.rotation = Quaternion.identity;

		if (sections != null && sections.Length > 0) {
			foreach (var s in sections) {
				s.transform.rotation = Quaternion.identity;
			}
		}
	}

	void Update ()
	{
		if (!worldRotatable) {
			return;
		}
		
		if (rotationActive) {

			if (Application.isMobilePlatform) {
			
				if (Input.touchCount > 0) {

					var toucPos = Input.GetTouch (0).position;

					float delta = toucPos.x < Screen.width / 2 ? ROTATE_SPEED : -ROTATE_SPEED;
				
					transform.Rotate (Vector3.forward * delta * Time.deltaTime);
				}

			} else {
				var rotate = Input.GetAxisRaw ("Horizontal");

				if (rotate != 0f) {
					float delta = rotate < 0f ? ROTATE_SPEED : -ROTATE_SPEED;

					transform.Rotate (Vector3.forward * delta * Time.deltaTime);
				}
			}
		}
	}
}
