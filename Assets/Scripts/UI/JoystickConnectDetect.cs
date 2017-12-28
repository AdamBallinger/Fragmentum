using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class JoystickConnectDetect : MonoBehaviour
    {
        public GameObject connectedBox;
        public GameObject disconnectedBox;

        public float flyInTime = 1.0f;
        public AnimationCurve curve;

        public float offScreenY;
        public float onScreenY;

        [Tooltip("Time in seconds to wait before moving the notification window back out of screen.")]
        public float waitTime = 1.0f;

        private int joysticksLastFrame;

        private void Start()
        {
            joysticksLastFrame = Input.GetJoystickNames().Length;
        }

        private void Update()
        {         
            var joysticks = Input.GetJoystickNames();
            var currentJoysticksCount = joysticks.Count(joy => joy != string.Empty);

            if (currentJoysticksCount > joysticksLastFrame)
            {
                StopAllCoroutines();
                StartCoroutine(NotifyJoystickConnectionChange(connectedBox));
            }
            else if (currentJoysticksCount < joysticksLastFrame)
            {
                StopAllCoroutines();
                StartCoroutine(NotifyJoystickConnectionChange(disconnectedBox));
            }

            joysticksLastFrame = currentJoysticksCount;
        }

        private IEnumerator NotifyJoystickConnectionChange(GameObject _connectState)
        {
            var t = 0.0f;
            var rectPos = _connectState.GetComponent<RectTransform>().anchoredPosition;

            while(t <= 1.0f)
            {
                rectPos.y = Mathf.Lerp(offScreenY, onScreenY, curve.Evaluate(t));
                _connectState.GetComponent<RectTransform>().anchoredPosition = rectPos;

                t += Time.deltaTime / flyInTime;

                yield return null;
            }

            yield return new WaitForSeconds(waitTime);

            t = 0.0f;

            while(t <= 1.0f)
            {
                rectPos.y = Mathf.Lerp(onScreenY, offScreenY, curve.Evaluate(t));
                _connectState.GetComponent<RectTransform>().anchoredPosition = rectPos;

                t += Time.deltaTime;

                yield return null;
            }

        }
    }
}
