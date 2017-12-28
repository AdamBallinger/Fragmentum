using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Objects
{
    public class FallingPlatform : MonoBehaviour
    {
        public Rigidbody platform;
        public float waitTime;
        public float platformFadeSpeed;

        public float shakeAmount = 1.0f;
        public AnimationCurve shakeCurve;

        private void Start()
        {
            platform = GetComponent<Rigidbody>();
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                StopAllCoroutines();
                StartCoroutine(Fall());
            }
        }

        private IEnumerator Fall()
        {
            Debug.Log("Falling");
            var initialPos = transform.position;
            var t = 0.0f;

            while(true)
            {
                transform.position = Vector3.Lerp(initialPos, initialPos + Vector3.down * shakeAmount, shakeCurve.Evaluate(t));

                t += Time.deltaTime / waitTime;

                if(t >= 1.0f)
                {
                    platform.isKinematic = false;
                    break;
                }

                yield return null;
            }

            StartCoroutine(Fade());
        }

        private IEnumerator Fade()
        {
            var t = 0.0f;

            var platformRenderer = GetComponent<Renderer>();
            var color = platformRenderer.material.color;

            while (true)
            {
                color.a = Mathf.Lerp(1.0f, 0.0f, t);

                platformRenderer.material.color = color;

                t += Time.deltaTime / platformFadeSpeed;

                if (t >= 1.0f)
                {
                    DestroyImmediate(platform.gameObject);
                }

                yield return null;
            }
        }
    }
}
