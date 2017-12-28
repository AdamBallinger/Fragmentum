using UnityEngine;

namespace Assets.Scripts.Character
{
    public enum MovementAxis
    {
        X,
        Z
    }

    public class PlayerPositionClamp : MonoBehaviour
    {

        public MovementAxis axis = MovementAxis.Z;
        private float clampValue;

        private Transform _transform;

        public void Start()
        {
            _transform = transform;

            switch (axis)
            {
                case MovementAxis.Z:
                    clampValue = _transform.position.z;
                    break;

                case MovementAxis.X:
                    clampValue = _transform.position.x;
                    break;
            }
        }

        public void Update()
        {
            var pos = _transform.position;

            switch (axis)
            {
                case MovementAxis.Z:
                    pos.z = clampValue;
                    break;

                case MovementAxis.X:
                    pos.x = clampValue;
                    break;

                default:
                    pos.z = clampValue;
                    break;
            }

            _transform.position = pos;
        }
    }
}