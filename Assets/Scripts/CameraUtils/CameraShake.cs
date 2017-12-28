using System.Collections;
using UnityEngine;

namespace Assets.Scripts.CameraUtils
{
    public class CameraShake : MonoBehaviour
    {

        public float duration = 1.0f;
        public float magnitude = 1.0f;

        private Transform _transform;

        private void Start()
        {
            _transform = transform;
        }

        public void StartShake()
        {
            StopAllCoroutines();
            StartCoroutine(Shake(duration));
        }

        public void StartShake(float customeShakeDuration)
        {
            StopAllCoroutines();
            StartCoroutine(Shake(customeShakeDuration));
        }

        public void StopShaking()
        {
            StopAllCoroutines();
        }

        public void StartEndlessShake()
        {
            StopAllCoroutines();
            StartCoroutine(Shake(99999.0f));
        }

        private IEnumerator Shake(float _duration)
        {
            var elapsed = 0.0f;

            var originalCamPos = _transform.position;

            while(elapsed < _duration)
            {
                elapsed += Time.deltaTime;

                var percentComplete = elapsed / _duration;
                var damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

                var x = Random.value * 2.0f - 1.0f;
                var y = Random.value * 2.0f - 1.0f;

                x *= magnitude * damper;
                y *= magnitude * damper;

                _transform.position = new Vector3(originalCamPos.x + x, originalCamPos.y + y, originalCamPos.z);

                yield return null;
            }

            _transform.position = originalCamPos;
        }
    }
}
