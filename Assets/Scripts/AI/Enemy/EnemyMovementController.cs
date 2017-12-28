using System.Collections;
using UnityEngine;

namespace Assets.Scripts.AI.Enemy
{
    public class EnemyMovementController : MonoBehaviour
    {

        public GameObject deathParticleSystem;  
        public ParticleSystem beamParticles;
        public GameObject meshObjects;

        public float enemyMovementSpeed;
        // used to make the enemy face the right way
        public Transform player;

        // used for enemy attacking
        public float chargeTime;
        public Rigidbody enemyRigidBody;

        private Transform _transform;
        private Animator _animator;

        // Use this for initialization

        public void Start()
        {
            _transform = transform;
            _animator = GetComponent<Animator>();

            enemyRigidBody = GetComponent<Rigidbody>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("PlayerMain"))
            {
                if(enemyRigidBody.CompareTag("RockEnemy"))
                {
                    StartCoroutine(Beam());
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("PlayerMain"))
            {
                var direction = player.position - _transform.position;
                direction.y = 0f;
                var rotation = Quaternion.LookRotation(direction).eulerAngles;

                if (enemyRigidBody.CompareTag("MushroomEnemy"))
                {
                    rotation.y += -90.0f;
                }

                _transform.rotation = Quaternion.Euler(rotation);
                StartCoroutine(Charge());

                if(enemyRigidBody.CompareTag("MushroomEnemy"))
                {
                    _animator.SetBool("isWalking", true);
                }           
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("PlayerMain"))
            {
                StopAllCoroutines();

                if(enemyRigidBody.CompareTag("MushroomEnemy"))
                {
                    _animator.SetBool("isWalking", false);
                }

                if(beamParticles != null)
                {
                    beamParticles.Stop();
                }
            }
        }

        public void OnDeath()
        {
            Instantiate(deathParticleSystem, _transform.position, Quaternion.identity);
        }

        private IEnumerator Charge()
        {          
            // apply force to the enemy to move towards the player based on where he is facing.
            // enemyRigidBody.AddForce((transform.forward * enemyMovementSpeed), ForceMode.Impulse);
            if(enemyRigidBody.CompareTag("RockEnemy"))
            {               
                yield return new WaitForSeconds(chargeTime);
                enemyRigidBody.AddForce(_transform.forward * enemyMovementSpeed, ForceMode.Impulse);

            }
            if (enemyRigidBody.CompareTag("MushroomEnemy"))
            {
                yield return new WaitForSeconds(chargeTime);
                enemyRigidBody.AddForce(_transform.right * enemyMovementSpeed, ForceMode.Impulse);
            }

            yield return null;
        }

        private IEnumerator Beam()
        {
            while (true)
            {
                beamParticles.Play();
                yield return new WaitForSeconds(4.0f);
                beamParticles.Stop();
            }
        }
    }
}
