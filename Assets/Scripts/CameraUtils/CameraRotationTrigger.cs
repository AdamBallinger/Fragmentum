using System.Collections;
using UnityEngine;

namespace Assets.Scripts.CameraUtils
{
    [RequireComponent(typeof(BoxCollider))]
    public class CameraRotationTrigger : MonoBehaviour
    {

        [Tooltip("The euler angles to rotate the camera too when entering this trigger.")]
        public Vector3 targetRotation;

        [Tooltip("The time in seconds it takes to complete")]
        public float rotationDuration = 1.0f;

        public AnimationCurve curve;

        // The array of rotation triggers to cancel out when this trigger is invoked.
        public CameraRotationTrigger[] cancelList;

        private GameObject cam;

        private void Start()
        {
            cam = GameObject.FindGameObjectWithTag("StandardizedCamera");

            if (cam == null)
            {
                Debug.LogWarning("No standardized camera found in the scene. This rotation trigger won't work!");
            }
        }

        public void OnTriggerEnter(Collider _collider)
        {
            if (cam == null) return;

            if (_collider.gameObject.tag.Equals("PlayerMain"))
            {
                StopAllCoroutines();

                foreach(var trig in cancelList)
                {
                    trig.StopAllCoroutines();
                }

                StartCoroutine(LerpCamera(GameObject.FindGameObjectWithTag("PlayerCameraTrigger")));
            }
        }

        private IEnumerator LerpCamera(GameObject _player)
        {
            //cam.GetComponent<CameraFollowPlayer>().enabled = false;
            var pos = cam.transform.position;

            var rotation = cam.transform.rotation.eulerAngles;
            var initialRotation = rotation;

            var t = 0.0f;

            while (true)
            {
                pos.x = _player.transform.position.x;
                //cam.transform.position = pos;

                rotation.x = Mathf.LerpAngle(initialRotation.x, targetRotation.x, curve.Evaluate(t));
                rotation.y = Mathf.LerpAngle(initialRotation.y, targetRotation.y, curve.Evaluate(t));
                rotation.z = Mathf.LerpAngle(initialRotation.z, targetRotation.z, curve.Evaluate(t));

                cam.transform.rotation = Quaternion.Euler(rotation);

                t += Time.deltaTime / rotationDuration;

                if (t >= 1.0f)
                {
                    //cam.GetComponent<CameraFollowPlayer>().enabled = true;
                    yield break;
                }

                yield return null;
            }
        }
    }
}