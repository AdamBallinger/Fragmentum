using UnityEngine;

namespace Assets.Scripts.Objects
{
    [RequireComponent(typeof(Rigidbody))]
    public class PushableObject : MonoBehaviour
    {

        [Tooltip("The power of the movement when this object gets pushed by the player. Higher will result in a faster push and vice versa.")]
        public float pushPower = 1.0f;

    }
}
