using UnityEngine;

namespace Assets.Scripts.CameraUtils
{
    public enum CameraFollowAxis
    {
        X,
        Z
    }

    public class CameraFollowPlayer : MonoBehaviour
    {

        public float followSpeed = 10.0f;

        // How much to offset the Y by when following on the Y axis.
        public float yFollowOffset = 1.0f;
        public bool followOnY = false;

        public CameraFollowAxis axis = CameraFollowAxis.X;

        private Transform _transform;
        private Transform followTargetTransform;

        public void Update()
        {
            _transform = transform;

            if(followTargetTransform == null)
            {
                var player = GameObject.FindGameObjectWithTag("PlayerMain");

                if(player != null)
                {
                    followTargetTransform = player.transform.Find("CameraFollow").transform;
                }

                return;
            }

            var pos = _transform.position;

            if(axis == CameraFollowAxis.X)
            {
                pos.x = followTargetTransform.position.x;
            }

            if(axis == CameraFollowAxis.Z)
            {
                pos.z = followTargetTransform.position.z;
            }

            if(followOnY)
            {
                pos.y = followTargetTransform.position.y + yFollowOffset;
            }

            _transform.position = Vector3.MoveTowards(_transform.position, pos, followSpeed * Time.deltaTime);
        }

    }
}
