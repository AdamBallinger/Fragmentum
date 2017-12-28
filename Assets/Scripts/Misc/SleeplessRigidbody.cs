using UnityEngine;

namespace Assets.Scripts.Misc
{
    [RequireComponent(typeof(Rigidbody))]
    public class SleeplessRigidbody : MonoBehaviour
    {

        private Rigidbody _rigidBody;

        private void Start()
        {
            _rigidBody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if(_rigidBody.IsSleeping())
            {
                _rigidBody.WakeUp();
            }
        }

    }
}
