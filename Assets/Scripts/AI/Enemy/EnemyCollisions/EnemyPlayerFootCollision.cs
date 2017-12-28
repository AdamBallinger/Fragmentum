using Assets.Scripts.Character;
using UnityEngine;

namespace Assets.Scripts.AI.Enemy.EnemyCollisions
{
    public class EnemyPlayerFootCollision : MonoBehaviour
    {
        public GameObject rootObject;
        public Vector3 enemyPos;
        public GameObject soundPos;

        public AudioClip enemyKill;

        private Transform rootObjectTransform;

        private void Start()
        {
            soundPos = GameObject.Find("Death Sound Master");
            rootObjectTransform = rootObject.transform;
        }

        public void OnPlayerFootCollision(Player _player)
        {
            _player.BounceOffEnemyHead();

            enemyPos = rootObjectTransform.position;

            var sp = Instantiate(soundPos, enemyPos, Quaternion.identity);
            var psas = sp.GetComponent<AudioSource>();

            if (psas != null)
            {
                psas.clip = enemyKill;
                psas.Play();
            }

            rootObject.SendMessage("OnDeath", SendMessageOptions.DontRequireReceiver);
            rootObject.SetActive(false);
        }
    }
}