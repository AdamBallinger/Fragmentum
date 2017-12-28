using Assets.Scripts.Character;
using UnityEngine;

namespace Assets.Scripts.Objects.PlantBoss
{
    public class PetalProjectileController : MonoBehaviour
    {
        public int damage = 5;

        public ParticleSystem contactParticleSystem;

        private void Update()
        {
            GetComponent<Rigidbody>().velocity += Physics.gravity * Time.deltaTime;
        }

        private bool collided;

        private void OnTriggerEnter(Collider _collision)
        {
            if (collided) return;

            if (_collision.CompareTag("PlantBoss"))
            {
                return;
            }

            if (_collision.CompareTag("PlayerMain"))
            {
                var player = _collision.gameObject.GetComponent<Player>();
                player.RemoveHealth(damage);
            }

            collided = true;
            contactParticleSystem.Play(true);

            GetComponent<Renderer>().enabled = true;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            Destroy(gameObject, 2.0f);
        }
    }
}