using Assets.Scripts.Character;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Objects.PlantBoss
{
    public class SeedProjectileController : MonoBehaviour
    {
        public int damage = 5;

        private List<string> ignoreTags = new List<string> { "PlantBoss", "IgnoreProjectile", "EnemyProjectile", "PlantEnemy", "Player", "Checkpoint" };

        private void OnTriggerEnter(Collider _collision)
        {
            if (ignoreTags.Contains(_collision.gameObject.tag))
            {
                return;
            }

            //Debug.Log(_collision.gameObject.tag);

            if (_collision.CompareTag("PlayerMain"))
            {
                var player = _collision.gameObject.GetComponent<Player>();
                player.RemoveHealth(damage);
            }

            GetComponent<Renderer>().enabled = false;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            Destroy(gameObject);
        }
    }
}