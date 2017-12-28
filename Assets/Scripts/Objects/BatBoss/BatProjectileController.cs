using System.Linq;
using Assets.Scripts.AI.Bosses.Bat;
using Assets.Scripts.Character;
using UnityEngine;

namespace Assets.Scripts.Objects.BatBoss
{
    public class BatProjectileController : MonoBehaviour
    {
        [Tooltip("Tags to ignore collisions with.")]
        public string[] ignoreTags;

        [Range(1, 100)]
        public int damage = 5;

        public ParticleSystem contactParticleSystem;

        private bool collided;

        private void OnTriggerEnter(Collider _collision)
        {
            if (collided) return;

            if(ignoreTags.Contains(_collision.gameObject.tag))
            {
                // Ignore collisions with self or minions.
                return;
            }

            //Debug.Log(_collision.gameObject.name);

            collided = true;

            if(_collision.CompareTag("PlayerMain"))
            {
                _collision.gameObject.GetComponent<Player>().RemoveHealth(damage);
            }

            if(contactParticleSystem != null)
            {
                contactParticleSystem.Play(true);
            }

            var projLight = GetComponent<Light>();

            if (projLight)
            {
                projLight.enabled = false;
            }

            GetComponent<Renderer>().enabled = false;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            Destroy(gameObject, 2.0f);
        }

    }
}
