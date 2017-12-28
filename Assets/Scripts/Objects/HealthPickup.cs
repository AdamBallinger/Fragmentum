using Assets.Scripts.Character;
using UnityEngine;

namespace Assets.Scripts.Objects
{
    public class HealthPickup : MonoBehaviour
    {

        public float rotationSpeed = 1.0f;
        public float floatSpeed = 1.0f;
        public float floatIntensity = 1.0f;

        [Range(1, 100)]
        public int healAmount = 10;

        private Vector3 startPos;

        private Transform _transform;

        private void Start()
        {
            _transform = transform;

            startPos = _transform.position;
        }

        private void OnTriggerEnter(Collider _collider)
        {
            if(_collider.CompareTag("PlayerMain"))
            {
                _collider.gameObject.GetComponent<Player>().AddHealth(healAmount);
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            _transform.Rotate(Vector3.up, rotationSpeed);

            var pos = _transform.position;
            pos.y = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatIntensity;
            _transform.position = pos;
        }
    }
}
