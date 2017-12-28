using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Objects
{
    [DisallowMultipleComponent]
    public class ObjectDirector : MonoBehaviour
    {
        public bool enableGizmos = true;

        public bool startOnInitialize = true;

        public GameObject directedObject;
        public AnimationCurve defaultCurve;

        public List<DirectorData> directorPoints = new List<DirectorData>();

        public UnityEvent onStart;
        public UnityEvent onPointReached;
        public UnityEvent onFinish;

        private bool halted = false;

        private void Start()
        {
            if (directedObject == null)
            {
                Debug.LogWarning("No object reference on object director!");
                return;
            }

            if (startOnInitialize)
            {
                StartDirector(true);
            }
        }

        public void StartDirector(bool _moveObjectToStart)
        {
            if (directedObject == null)
            {
                Debug.LogWarning("Can't play director without an object reference to direct.");
                return;
            }

            if (_moveObjectToStart)
            {
                directedObject.transform.position = directorPoints[0].objectPosition;
                directedObject.transform.rotation = Quaternion.Euler(directorPoints[0].objectRotation);
            }


            StopAllCoroutines();
            StartCoroutine(Director());
        }

        private IEnumerator Director()
        {
            if (onStart != null)
                onStart.Invoke();

            foreach (var point in directorPoints)
            {
                yield return LerpObjectPositionAndRotation(point);
            }

            if (onFinish != null)
                onFinish.Invoke();

            yield return null;
        }

        private IEnumerator LerpObjectPositionAndRotation(DirectorData _data)
        {
            var initialPos = directedObject.transform.position;
            var initialRot = directedObject.transform.rotation.eulerAngles;

            var rotation = initialRot;

            var t = 0.0f;

            while (true)
            {
                if (_data.transitionTime == 0.0f)
                {
                    directedObject.transform.position = _data.objectPosition;
                    directedObject.transform.rotation = Quaternion.Euler(_data.objectRotation);
                    t = 1.0f;
                }
                else
                {
                    directedObject.transform.position = Vector3.Lerp(initialPos, _data.objectPosition, _data.enableCustomCurve ? _data.moveCurve.Evaluate(t) : defaultCurve.Evaluate(t));
                    rotation.x = Mathf.LerpAngle(initialRot.x, _data.objectRotation.x, _data.enableCustomCurve ? _data.moveCurve.Evaluate(t) : defaultCurve.Evaluate(t));
                    rotation.y = Mathf.LerpAngle(initialRot.y, _data.objectRotation.y, _data.enableCustomCurve ? _data.moveCurve.Evaluate(t) : defaultCurve.Evaluate(t));
                    rotation.z = Mathf.LerpAngle(initialRot.z, _data.objectRotation.z, _data.enableCustomCurve ? _data.moveCurve.Evaluate(t) : defaultCurve.Evaluate(t));
                    directedObject.transform.rotation = Quaternion.Euler(rotation);
                }

                t += Time.deltaTime / _data.transitionTime;

                if (t >= 1.0f)
                {
                    if (onPointReached != null)
                        onPointReached.Invoke();

                    if (_data.enableCallback && _data.onReached != null)
                        _data.onReached.Invoke();

                    if (_data.haltOnReach)
                    {
                        halted = true;
                        yield return new WaitUntil(() => !halted);
                        break;
                    }

                    yield return new WaitForSeconds(_data.delay);
                    break;
                }

                yield return null;
            }
        }

        private void OnDrawGizmos()
        {
            if (directorPoints == null || !enableGizmos) return;

            Gizmos.color = Color.cyan;

            for (var i = 0; i < directorPoints.Count; i++)
            {
                if (i > 0 && i < directorPoints.Count - 1)
                {
                    Gizmos.color = Color.blue;
                }

                if (i == directorPoints.Count - 1)
                {
                    Gizmos.color = Color.red;
                }

                Gizmos.DrawCube(directorPoints[i].objectPosition, Vector3.one * 0.4f);
            }

            for (var i = 1; i < directorPoints.Count; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(directorPoints[i - 1].objectPosition, directorPoints[i].objectPosition);
            }
        }
    }

    [Serializable]
    public class DirectorData
    {
        public Vector3 objectPosition;
        public Vector3 objectRotation;

        public AnimationCurve moveCurve;

        // Time in seconds for the object to take  in order to finish the transition to this point.
        public float transitionTime = 1.0f;

        public bool enableCustomCurve = false;

        // If true, then the cutscene will be halted when reaching this point and will need to be resumed before continuing.
        public bool haltOnReach = false;

        // Delay in seconds a cutscene will wait when the object is at this data's position/rotation.
        public float delay = 0.0f;

        // Should this point invoke its onReached callback?
        public bool enableCallback = false;

        // Callback executed as the director point is reached and before the delay if there is one.
        public UnityEvent onReached;
    }
}