using System.Collections;
using Assets.Scripts.BossManagers;
using Assets.Scripts.Objects.BatBoss;
using UnityEngine;

namespace Assets.Scripts.AI.Bosses.Bat
{
    public class BatMinionController : MonoBehaviour
    {

        public float moveSpeed = 1.0f;
        public float rotationSpeed = 1.0f;
        public AnimationCurve rotationCurve;

        // Time in seconds it takes to relight the bonfire.
        public float relightTime = 1.5f;

        // Particle system reference for the flamethrower.
        public ParticleSystem flameThrowerPS;

        [HideInInspector]
        public BonfireController targetPit;

        // Position the minion should fall too once spawned before moving to its fire pit target.
        [HideInInspector]
        public Vector3 entranceFallPosition = Vector3.zero;

        // Prevents player killing the minion (Used for the initial bats during the intro)
        [HideInInspector]
        public bool invulnerable = false;

        private bool isFallingToInitalPosition = true;
        private bool isMovingToTarget;

        //sound
        public GameObject standardizedCam;
        public Vector3 soundPos;
        public AudioClip flamethrower;

        private Transform _transform;
        private BatBossManager batBossManager;

        private void Start()
        {
            _transform = transform;
            batBossManager = FindObjectOfType<BatBossManager>();

            //location of sound
            standardizedCam = GameObject.FindGameObjectWithTag("StandardizedCamera");
        }

        private void Update()
        {
            //where to play sound
            soundPos = standardizedCam.transform.position;

            if (targetPit == null)
            {
                Debug.Log("No bonfire reference for bat minion!");
                return;
            }

            var position = _transform.position;

            if (isFallingToInitalPosition)
            {
                position = _transform.position;
                position = Vector3.MoveTowards(position, entranceFallPosition, moveSpeed * Time.deltaTime);
                _transform.position = position;

                if (_transform.position == entranceFallPosition)
                {
                    isFallingToInitalPosition = false;
                    isMovingToTarget = true;
                }
            }
            else
            {
                if (isMovingToTarget)
                {
                    var pitRelightPos = targetPit.transform.position;
                    pitRelightPos.z = entranceFallPosition.z;

                    position = Vector3.MoveTowards(position, pitRelightPos, moveSpeed * Time.deltaTime);
                    _transform.position = position;

                    if (_transform.position == pitRelightPos)
                    {
                        isMovingToTarget = false;
                        StartCoroutine(RelightPit());
                    }
                }
            }
        }

        private IEnumerator RelightPit()
        {
            var initialRotation = _transform.rotation;
            _transform.LookAt(targetPit.transform);
            var targetRotation = _transform.rotation.eulerAngles;
            targetRotation.x = 0.0f;
            targetRotation.z = 0.0f;
            targetRotation.y += 180f;

            _transform.rotation = initialRotation;

            var t = 0.0f;

            while (true)
            {
                var rot = _transform.rotation.eulerAngles;
                rot.x = Mathf.LerpAngle(initialRotation.x, targetRotation.x, t);
                rot.y = Mathf.LerpAngle(initialRotation.y, targetRotation.y, t);
                rot.z = Mathf.LerpAngle(initialRotation.z, targetRotation.z, t);

                _transform.rotation = Quaternion.Euler(rot);

                t += Time.deltaTime / rotationSpeed;

                if (t >= 1.0f)
                {
                    break;
                }

                yield return null;
            }

            flameThrowerPS.Play(true);

            //SFX
            GetComponent<AudioSource>().clip = flamethrower;
            GetComponent<AudioSource>().Play();

            t = 0.0f;

            while (true)
            {
                if (targetPit == null)
                {
                    // if the target pit is null somehow then destroy the object.
                    Destroy(gameObject);
                    yield break;
                }

                targetPit.strength = Mathf.Lerp(0.0f, 1.0f, t);

                t += Time.deltaTime / relightTime;

                if (t >= 1.0f)
                {
                    break;
                }

                yield return null;
            }

            flameThrowerPS.Stop(true);
            targetPit.hasMinionAttention = false;
            StartCoroutine(MoveBackToFallPosition());
        }

        private IEnumerator MoveBackToFallPosition()
        {
            var pos = _transform.position;
            var targetPos = entranceFallPosition;

            while (true)
            {
                pos = Vector3.MoveTowards(pos, targetPos, moveSpeed * Time.deltaTime);
                _transform.position = pos;

                if (_transform.position == entranceFallPosition)
                {
                    targetPos = batBossManager.minionSpawnPosition;
                    break;
                }

                yield return null;
            }

            while (true)
            {
                pos = Vector3.MoveTowards(pos, targetPos, moveSpeed * Time.deltaTime);
                _transform.position = pos;

                if (_transform.position == batBossManager.minionSpawnPosition)
                {
                    break;
                }

                yield return null;
            }

            // Destroy bat when its returned to its spawn position
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider _collider)
        {
            if (_collider.CompareTag("PlayerMain") && !invulnerable)
            {
                targetPit.hasMinionAttention = false;
                Destroy(gameObject);
            }
        }
    }
}
