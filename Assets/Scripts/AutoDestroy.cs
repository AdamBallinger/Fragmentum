using UnityEngine;

namespace Assets.Scripts
{
    public class AutoDestroy : MonoBehaviour
    {

        [Tooltip("Time in seconds to wait before destroying this object upon instantiation.")]
        public float destroyDelay = 1.0f;

        private void Start()
        {
            Destroy(gameObject, destroyDelay);
        }
    }
}
