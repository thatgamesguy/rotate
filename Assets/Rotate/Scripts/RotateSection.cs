using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSection : MonoBehaviour
{
	public bool canRotate = false;

	public float rotationAngleSec = 120f;

	public bool limitRotation = false;
	public Vector2 minMaxZ = Vector2.zero;

	void Update ()
	{
		if (canRotate) {
			if (Application.isMobilePlatform) {

				if (Input.touchCount > 0) {

					var toucPos = Input.GetTouch (0).position;

					float delta = toucPos.x < Screen.width / 2 ? rotationAngleSec : -rotationAngleSec;

					transform.Rotate (Vector3.forward * delta * Time.deltaTime);

				}

			} else {
				var rotate = Input.GetAxisRaw ("Horizontal");

				if (rotate != 0f) {
					float delta = rotate < 0f ? rotationAngleSec : -rotationAngleSec;

					transform.Rotate (Vector3.forward * delta * Time.deltaTime);

					if (limitRotation) {
					
						transform.localRotation = new Quaternion (
							transform.localRotation.x, 
							transform.localRotation.y, 
							Mathf.Clamp(transform.localRotation.z, minMaxZ.x, minMaxZ.y), 
							transform.localRotation.w);

					}
				}
			}
		}
	}
}
