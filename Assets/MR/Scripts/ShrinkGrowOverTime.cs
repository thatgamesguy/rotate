using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkGrowOverTime : MonoBehaviour
{
	public float fromScale = 0f;
	public float toScale = 1f;
	public bool isScalingUp = false;
	public float scaleUpSpeed = 4f;
	public float scaleDownSpeed = 4f;
	public float delayToStartScalingDown = 0f;
	public float delayToStartScalingUp = 0f;

	private bool isScaling = true;


	void OnEnable()
	{
		isScaling = true;
		StartCoroutine (DoScale ());
	}

	void OnDisable()
	{
		isScaling = false;
	}


	private IEnumerator DoScale ()
	{
		while (isScaling) {
		
			if (isScalingUp) {
				if (transform.localScale.x < toScale) {
					transform.localScale += Vector3.one * 4f * Time.deltaTime;
				} else {
					transform.localScale = new Vector3 (toScale, toScale, toScale);
					isScalingUp = false;

					yield return new WaitForSeconds (delayToStartScalingDown);
				}
			} else {
				if (transform.localScale.x > fromScale) {
					transform.localScale -= Vector3.one * 4f * Time.deltaTime;
				} else {
					transform.localScale = new Vector3 (fromScale, fromScale, fromScale);
					isScalingUp = true;



					yield return new WaitForSeconds (delayToStartScalingUp);
				}
			}

			yield return null;
		}
	}
		
	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.CompareTag (Tags.Player)) {
	
			if (isScalingUp) {

				var heading = other.transform.position - transform.position;
				var dist = heading.magnitude;
				var dir = heading / dist;

				Debug.Log (dist);

				other.gameObject.GetComponent<Rigidbody2D> ().AddForce (heading.normalized * 100f);
			}
		}
	}

}
