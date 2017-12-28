using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.CameraUtils
{
    [DisallowMultipleComponent]
    public class CameraCutscene : MonoBehaviour
    {
        public bool enableGizmos = true;

        public bool startOnInitialize = true;

        public AnimationCurve defaultCurve;

        public List<CutsceneData> cutSceneData = new List<CutsceneData>();

        public UnityEvent onStart;
        public UnityEvent onPointReached;
        public UnityEvent onFinish;

        private GameObject cam;

        private bool halted;
        private bool resumeQueued;

        private Transform camTransform;

        private void Start()
        {
            cam = Camera.main.gameObject.transform.root.gameObject;

            if(cam != null)
            {
                camTransform = cam.transform;
            }

            if (startOnInitialize)
            {
                StartCutscene(true);
            }
        }

        public void StartCutscene(bool _moveCamToStart)
        {
            if (cam == null)
            {
                Debug.LogWarning("Can't play cutscene without a standardized camera object in the scene.");
                return;
            }

            if (_moveCamToStart)
            {
                camTransform.position = cutSceneData[0].camPosition;
                camTransform.rotation = Quaternion.Euler(cutSceneData[0].camRotation);
            }


            StopAllCoroutines();
            StartCoroutine(Cutscene());
        }

        public void Resume()
        {
            resumeQueued = false;

            if (halted)
            {
                halted = false;
            }
            else
            {
                Debug.LogWarning("Trying to resume a cutscene when it is not halted!");
            }
        }

        public void EnqueueResume(float _time)
        {
            if (resumeQueued)
            {
                Debug.LogWarning("Resume already enqueued on cut scene!");
                return;
            }

            resumeQueued = true;
            Invoke("Resume", _time);
        }

        private IEnumerator Cutscene()
        {
            if (onStart != null)
            {
                onStart.Invoke();
            }

            cam.GetComponent<CameraFollowPlayer>().enabled = false;

            foreach (var point in cutSceneData)
            {
                yield return LerpCameraToPoint(point);
            }

            if (onFinish != null)
            {
                onFinish.Invoke();
            }

            yield return null;
        }

        private IEnumerator LerpCameraToPoint(CutsceneData _data)
        {
            var initialPos = camTransform.position;
            var initialRot = camTransform.rotation.eulerAngles;

            var rotation = initialRot;

            var t = 0.0f;

            while (true)
            {
                if (_data.transitionTime == 0.0f)
                {
                    camTransform.position = _data.camPosition;
                    camTransform.rotation = Quaternion.Euler(_data.camRotation);
                    t = 1.0f;
                }
                else
                {
                    camTransform.position = Vector3.Lerp(initialPos, _data.camPosition, _data.enableCustomCurve ? _data.moveCurve.Evaluate(t) : defaultCurve.Evaluate(t));
                    rotation.x = Mathf.LerpAngle(initialRot.x, _data.camRotation.x, _data.enableCustomCurve ? _data.moveCurve.Evaluate(t) : defaultCurve.Evaluate(t));
                    rotation.y = Mathf.LerpAngle(initialRot.y, _data.camRotation.y, _data.enableCustomCurve ? _data.moveCurve.Evaluate(t) : defaultCurve.Evaluate(t));
                    rotation.z = Mathf.LerpAngle(initialRot.z, _data.camRotation.z, _data.enableCustomCurve ? _data.moveCurve.Evaluate(t) : defaultCurve.Evaluate(t));
                    camTransform.rotation = Quaternion.Euler(rotation);
                }

                t += Time.deltaTime / _data.transitionTime;

                if (t >= 1.0f)
                {
                    if (onPointReached != null)
                    {
                        onPointReached.Invoke();
                    }

                    if (_data.enableCallback && _data.onReached != null)
                    {
                        _data.onReached.Invoke();
                    }

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
            if (cutSceneData == null || !enableGizmos) return;

            Gizmos.color = Color.green;

            for (var i = 0; i < cutSceneData.Count; i++)
            {
                if (i > 0 && i < cutSceneData.Count - 1)
                {
                    Gizmos.color = Color.black;
                }

                if (i == cutSceneData.Count - 1)
                {
                    Gizmos.color = Color.red;
                }

                Gizmos.DrawCube(cutSceneData[i].camPosition, Vector3.one * 0.4f);
            }

            for (var i = 1; i < cutSceneData.Count; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(cutSceneData[i - 1].camPosition, cutSceneData[i].camPosition);
            }
        }
    }

    [Serializable]
    public class CutsceneData
    {
        public Vector3 camPosition;
        public Vector3 camRotation;

        public AnimationCurve moveCurve;

        // Time in seconds for the cutscene point to take to finish.
        public float transitionTime = 1.0f;

        public bool enableCustomCurve = false;

        // If true, then the cutscene will be halted when reaching this point and will need to be resumed before continuing.
        public bool haltOnReach = false;

        // Delay in seconds a cutscene will wait when the camera is at this data's position/rotation.
        public float delay = 0.0f;

        // Should this point invoke its onReached callback?
        public bool enableCallback = false;

        // Callback executed as the cutscene point is reached and before the delay if there is one.
        public UnityEvent onReached;
    }
}
