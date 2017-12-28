using System.Collections;
using UnityEngine;

namespace Assets.Scripts.AI.Enemy
{
    public class EnemyBatAI : MonoBehaviour
    {

        public float enemyMovementSpeed;
        // used to make the enemy face the right way
        public Transform player;

        // used for enemy attacking
        public float chargeTime;
        public int attackDamage = 5;
        public Rigidbody enemyRigidBody;

        private Transform _transform;

        private void Start()
        {
            _transform = transform;

            enemyRigidBody = GetComponent<Rigidbody>();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("PlayerMain"))
            {
                var direction = player.position - _transform.position;
                direction.y = -direction.y;
                var rotation = Quaternion.LookRotation(direction).eulerAngles;
                rotation.y += 180.0f;
                _transform.rotation = Quaternion.Euler(rotation);
                StartCoroutine(Charge());
            }
        }

        private void OnTriggerExit(Collider other)
        {   
            if (other.CompareTag("PlayerMain"))
            {
                StopAllCoroutines();
            }
        }

        private IEnumerator Charge()
        {
            yield return new WaitForSeconds(chargeTime);
            // apply force to the enemy to move towards the player based on where he is facing.
            enemyRigidBody.AddForce(-_transform.forward * enemyMovementSpeed, ForceMode.Impulse);
            yield return null;
        }
    }
}