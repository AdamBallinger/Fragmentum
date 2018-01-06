using Assets.Scripts.Character;
using UnityEngine;

namespace Assets.Scripts.AI.Enemy.EnemyCollisions
{
    public class EnemyPlayerBodyCollision : MonoBehaviour
    {
        public GameObject rootObject;

        public Vector3 enemyPos;
        public GameObject soundPos;

        public AudioClip enemyKill;

        [Tooltip("Damage to deal to the player on hit.")]
        public int playerHitDamage = 5;

        private Transform rootObjectTransform;

        private void Start()
        {
            soundPos = GameObject.Find("Death Sound Master");
            rootObjectTransform = rootObject.transform;
        }

        public void Update()
        {
            enemyPos = rootObjectTransform.position;
        }

        public void OnPlayerBodyCollision(Player _player)
        {
            if(_player.Dashing)
            {
                var sp = Instantiate(soundPos, enemyPos, Quaternion.identity);
                var psas = sp.GetComponent<AudioSource>();

                if (psas != null)
                {
                    psas.clip = enemyKill;
                    psas.Play();
                }

                rootObject.SetActive(false);
            }
            else
            {
                _player.RemoveHealth(playerHitDamage);
            }
        }
    }
}