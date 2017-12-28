using Assets.Scripts.Character;
using UnityEngine;

namespace Assets.Scripts.Objects.PlantBoss
{
    public class AcidSpoutProjectileController : MonoBehaviour
    {
        public int damage = 5;
        private bool collided;

        public int poolingTime = 3000;
        private float poolingTimer;

        public int sustainTime = 2000;
        private float sustainTimer;

        private bool pooling = true;
        private bool sustaining;
        private bool growing;

        public ParticleSystem contactParticleSystem;
        public ParticleSystem contactParticleSystemA;
        public GameObject poolCollider;
        public GameObject streanCollider;

        private void Start ()
        {
            contactParticleSystemA.Stop();
        }

        private void Update()
        {
            var colliderScale = transform.localScale;

            if (pooling)
            {
                Pool();
            }
            else if (growing)
            {
                Grow(colliderScale);
            }
            else if (sustaining)
            {
                Sustain();
            }
            else if (!sustaining && !growing)
            {
                ShrinkAndDestroy(colliderScale);
            }
        }

        private void ShrinkAndDestroy(Vector3 colliderScale)
        {
            contactParticleSystemA.Stop();
            contactParticleSystem.Stop();
            Destroy(gameObject);
        }

        private void Sustain()
        {
            sustainTimer += Time.deltaTime * 1000;
            if (sustainTimer > sustainTime)
            {
                sustaining = false;
                sustainTimer = 0.0f;
            }
        }

        private void Grow(Vector3 colliderScale)
        {
            contactParticleSystemA.Play();
            growing = false;
            sustaining = true;
        }

        private void Pool()
        {
            if (!contactParticleSystem.isPlaying)
            {
                contactParticleSystem.Play();
            }

            poolingTimer += Time.deltaTime * 1000;
            if (poolingTimer > poolingTime)
            {
                pooling = false;
                growing = true;
                poolingTimer = 0.0f;
            }
        }

        private void OnTriggerEnter(Collider _collision)
        {
            if (collided) return;

            if (_collision.CompareTag("PlantBoss"))
            {
                return;
            }

            collided = true;

            if (_collision.CompareTag("PlayerMain"))
            {
                var player = _collision.gameObject.GetComponent<Player>();
                player.RemoveHealth(damage);
            }

            //GetComponent<Renderer>().enabled = true;
            //GetComponent<Rigidbody>().velocity = Vector3.zero;
            //Destroy(gameObject, 0.0f);
        }
    }
}
