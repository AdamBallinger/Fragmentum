using System.Collections;
using UnityEngine;

namespace Assets.Scripts.CameraUtils
{
    [RequireComponent(typeof(BoxCollider))]
    public class CameraZoomTrigger : MonoBehaviour
    {

        public float zoomZ = 0.0f;
        public float zoomY = 0.0f;

        [Tooltip("Time in seconds the zoom takes to complete.")]
        public float zoomDuration = 2.0f;

        public AnimationCurve curve;

        private GameObject cam;

        //private float prevZoomZ = 0.0f;
        //private float prevZoomY = 0.0f;

        public void Start()
        {
            cam = GameObject.FindGameObjectWithTag("StandardizedCamera");

            if(cam == null)
            {
                Debug.LogWarning("No standardized camera found in the scene. This zoom trigger won't work!");
            }
        }

        public void OnTriggerEnter(Collider _collider)
        {
            if (cam == null) return;

            if(_collider.gameObject.tag.Equals("PlayerMain"))
            {
                //prevZoomZ = cam.transform.position.z;
                //prevZoomY = cam.transform.position.y;

                StopAllCoroutines();
                StartCoroutine(LerpCamera(GameObject.FindGameObjectWithTag("PlayerCameraTrigger"), zoomZ, zoomY));
            }
        }

        //public void OnTriggerExit(Collider _collider)
        //{
        //    if (cam == null) return;

        //    if(_collider.gameObject.tag.Equals("Player") && previousZoomOnExit)
        //    {
        //        StopAllCoroutines();
        //        StartCoroutine(LerpCamera(GameObject.FindGameObjectWithTag("PlayerCameraTrigger"), prevZoomZ, prevZoomY));
        //    }        
        //}

        private IEnumerator LerpCamera(GameObject _player, float _targetZ, float _targetY)
        {
            cam.GetComponent<CameraFollowPlayer>().enabled = false;
            var pos = cam.transform.position;

            var initialY = pos.y;
            var initialZ = pos.z;

            var t = 0.0f;

            while(true)
            {
                pos.x = _player.transform.position.x;

                pos.y = Mathf.Lerp(initialY, _targetY, curve.Evaluate(t));
                pos.z = Mathf.Lerp(initialZ, _targetZ, curve.Evaluate(t));
                cam.transform.position = pos;

                t += Time.deltaTime / zoomDuration;

                if(t >= 1.0f)
                {
                    cam.GetComponent<CameraFollowPlayer>().enabled = true;
                    yield break;
                }

                yield return null;
            }
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            var zoomPos = transform.position;
            zoomPos.z = zoomZ;
            zoomPos.y = zoomY;
            Gizmos.DrawSphere(zoomPos, 0.25f);
        }
    }
}