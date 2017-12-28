using System.Collections;
using UnityEngine;

namespace Assets.Scripts.AI.Enemy
{
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyPlantAI : MonoBehaviour
    {
        public GameObject projectilePrefab;

        // used to make the enemy face the right way
        public Transform player;

        private Vector3 downPosition;
        private Vector3 upPosition;

        public float roarSpeedMultiplier = 1.0f;

        public float projectileSpeed;
        public float attackDelay;
        // Time in seconds to popin and out.
        public float lerpSpeed;
        public AnimationCurve lerpCurve;

        public AudioClip plantEnter;
        public AudioClip plantShoot;

        // Cached frequently used components.
        private Transform _transform;
        private Animator _animator;
        private AudioSource _audioSource;

        private void Start()
        {
            _transform = transform;
            _animator = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();

            downPosition = _transform.position;
            upPosition = downPosition + Vector3.up * 3.0f;
        }

        public void OnTriggerEnter(Collider _other)
        {
            if (_other.CompareTag("PlayerMain"))
            {
                StopAllCoroutines();
                StartCoroutine(LerpPos(upPosition, true));
            }
        }

        private IEnumerator LerpPos(Vector3 _pos, bool _roarAndFire)
        {
            var initialPos = _transform.position;
            var t = 0.0f;

            _audioSource.clip = plantEnter;
            _audioSource.Play();

            while (true)
            {
                _transform.position = Vector3.Lerp(initialPos, _pos, lerpCurve.Evaluate(t));

                t += Time.deltaTime / lerpSpeed;

                if(t >= 1.0f)
                {
                    if(_roarAndFire)
                    {
                        _animator.SetFloat("animSpeedMod", roarSpeedMultiplier);
                        _animator.SetBool("Roaring", true);
                        yield return null;
                        _animator.SetBool("Roaring", false);
                        yield return new WaitForSeconds(1.5f);
                        StartCoroutine(Fire(player));
                    }
                    
                    break;
                }

                yield return null;
            }
        }

        public void OnTriggerStay(Collider _other)
        {
            if (_other.CompareTag("PlayerMain"))
            {
                var direction = player.position - _transform.position;
                direction.y = 0f;
                _transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        public void OnTriggerExit(Collider _other)
        {
            if (_other.CompareTag("PlayerMain"))
            {
                StopAllCoroutines();
                StartCoroutine(LerpPos(downPosition, false));
                //when player leaves danger zone, allow the enemy to flip once more and stay still
                //enemyRigidBody.velocity = new Vector3(0.0f, 0.0f, 0.0f);
            }
        }

        private IEnumerator Fire(Transform target)
        { 
            while (true)
            {
                _animator.SetBool("Spitting", true);
                yield return null;
                _animator.SetBool("Spitting", false);
                yield return new WaitForSeconds(0.55f);

                var direction = (target.position + Vector3.up * 0.5f - _transform.position).normalized;
                var projectile = Instantiate(projectilePrefab, _transform.position, _transform.rotation);
                projectile.GetComponent<Rigidbody>().velocity = direction*projectileSpeed;

                _audioSource.clip = plantShoot;
                _audioSource.Play();

                yield return new WaitForSeconds(attackDelay);
            }
            
        }
    }
}
