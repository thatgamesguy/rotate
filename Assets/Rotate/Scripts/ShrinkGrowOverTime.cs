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
    private bool shouldBeScalingUp;

    void Start()
    {
        shouldBeScalingUp = isScalingUp;
    }

    public void StartShrinking()
    {
        transform.localScale = Vector2.one;
        isScalingUp = shouldBeScalingUp;
        isScaling = true;

        StartCoroutine(DoScale());
    }

    public void Reset()
    {
        isScaling = false;
        StopCoroutine(DoScale());
        transform.localScale = Vector2.one;
    }

    private IEnumerator DoScale()
    {
        if (isScalingUp)
        {
            yield return new WaitForSeconds(delayToStartScalingUp);
        }
        else
        {
            yield return new WaitForSeconds(delayToStartScalingDown);
        }

        while (isScaling)
        {

            if (isScalingUp)
            {
                if (transform.localScale.x < toScale)
                {
                    transform.localScale += Vector3.one * scaleUpSpeed * Time.deltaTime;
                }
                else
                {
                    transform.localScale = new Vector3(toScale, toScale, toScale);
                    isScalingUp = false;

                    yield return new WaitForSeconds(delayToStartScalingDown);
                }
            }
            else
            {
                if (transform.localScale.x > fromScale)
                {
                    transform.localScale -= Vector3.one * scaleDownSpeed * Time.deltaTime;
                }
                else
                {
                    transform.localScale = new Vector3(fromScale, fromScale, fromScale);
                    isScalingUp = true;



                    yield return new WaitForSeconds(delayToStartScalingUp);
                }
            }

            yield return null;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag(Tags.Player))
        {
            if (isScalingUp)
            {
                var playerPos = other.transform.position;
                var cubePos = transform.position;

                var heading = playerPos - cubePos;
                var dist = heading.magnitude;
                var dir = heading / dist;

                other.gameObject.GetComponent<Rigidbody2D>().AddForce(dir * 100f);
            }
        }
    }

}
