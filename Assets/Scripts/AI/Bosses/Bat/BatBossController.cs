using System.Collections;
using Assets.Scripts.BossManagers;
using Assets.Scripts.Character;
using Assets.Scripts.Objects.BatBoss;
using UnityEngine;

namespace Assets.Scripts.AI.Bosses.Bat
{
    public class BatBossController : MonoBehaviour
    {
        [Header("Stats")]
        public int maxHealth = 3;
        [HideInInspector]
        public int currentHealth;
        [HideInInspector]
        public bool invulnerable;

        [Header("Object References")]
        public GameObject bombProjectilePrefab;
        public GameObject plasmaProjectilePrefab;
        public GameObject batModelParent;

        [Header("Projectile Settings")]
        public float bombProjectileSpeed = 1.0f;
        public float plasmaProjectileSpeed = 1.0f;
        public string[] projectileIgnoreTags;

        [Header("Movement Settings")]
        public float batMoveSpeed = 1.5f;
        public Vector3[] floatingPositions;
        public float deathTime;
        // The raycast mask to check for ground position when bat is killed.
        public LayerMask deathRayMask;

        [Header("Attack Settings")]
        [Tooltip("Minimum time before a special attack can be performed.")]
        public float minDelayBetweenAttacks = 4.0f;
        [Tooltip("Maximum time before a special attack can be performed.")]
        public float maxDelayBetweenAttacks = 8.0f;
        public float minBasicAttackDelay = 3.0f;
        public float maxBasicAttackDelay = 5.0f;

        [Header("Bombard Attack Settings")]
        public float batMoveToBombardSpeed = 3.0f;
        public Vector3 leftSideBombardPosition = new Vector3(-10, 7.5f, -1.63f);
        public Vector3 rightSideBombardPosition = new Vector3(14.0f, 7.5f, -1.63f);
        public int bombardProjectileCount = 8;
        public float bombardProjectileSpacing = 0.25f;
        public float delayBetweenProjectileFire = 0.1f;

        [Header("Flamethrower Attack Settings")]
        public GameObject flameThrowerObject;
        [Tooltip("Position the bat moves to for this attack behaviour.")]
        public Vector3 flameAttackPosition = Vector3.zero;
        [Tooltip("How quick does the bat move to position.")]
        public float flameMoveToSpeed = 3.0f;
        public Vector3 leftSideFlameInitialPosition = Vector3.zero;
        public Vector3 rightSideFlameInititalPosition = Vector3.zero;
        public Vector3 leftSideFlameEndPosition = Vector3.zero;
        public Vector3 rightSideFlameEndPosition = Vector3.zero;
        public float flameThrowerRotateSpeed = 2.0f;

        [Header("Sound")]
        public GameObject standardizedCam;
        public Vector3 soundPos;
        public AudioClip flamethrower;
        public AudioClip deathSound;
        public AudioClip batPlasma;

        [Header("Gizmo settings")]
        public bool drawMovementGizmos = true;
        public bool drawBombardAttackGizmos = true;
        public bool drawFlameThrowerAttackGizmos = true;

        private Transform targetTransform;
        private Animator batAnimator;
        private bool rotateToTarget;

        private Transform _transform;
        private Transform parentTransform;


        private void Start()
        {
            _transform = transform;
            parentTransform = batModelParent.transform;

            currentHealth = maxHealth;
            targetTransform = GameObject.FindGameObjectWithTag("PlayerMain").transform;
            batAnimator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            if (targetTransform == null)
            {
                targetTransform = GameObject.FindGameObjectWithTag("PlayerMain").transform;
                return;
            }

            if (rotateToTarget)
            {
                RotateToTarget();
            }

            //get sound pos
            soundPos = standardizedCam.transform.position;
        }

        /// <summary>
        /// Instantly rotates the bat to the curent target if one exists.
        /// </summary>
        private void RotateToTarget()
        {
            if (targetTransform != null)
            {
                RotateToPosition(targetTransform.position);
            }
        }

        /// <summary>
        /// Rotates the bat instantly to a given position around the Y axis.
        /// </summary>
        /// <param name="_position"></param>
        private void RotateToPosition(Vector3 _position)
        {
            parentTransform.LookAt(_position);

            var euler = parentTransform.rotation.eulerAngles;
            euler.x = 0.0f;
            euler.z = 0.0f;
            euler.y -= 90.0f;

            var newRotation = Quaternion.Euler(euler);
            parentTransform.rotation = newRotation;
        }

        /// <summary>
        /// Fires a bomb projectile to the current target if one exists.
        /// </summary>
        public void FireBombProjectileAtTarget()
        {
            if (targetTransform == null)
            {
                return;
            }

            FireBombProjectileAtPosition(targetTransform.position);
        }

        /// <summary>
        /// Fires a bomb projectile at a given position.
        /// </summary>
        /// <param name="_position"></param>
        public void FireBombProjectileAtPosition(Vector3 _position)
        {
            var direction = (_position - _transform.position).normalized;
            var projectile = Instantiate(bombProjectilePrefab, _transform.position, parentTransform.rotation);

            GetComponent<AudioSource>().clip = batPlasma;
            GetComponent<AudioSource>().Play();

            projectile.GetComponent<BatProjectileController>().ignoreTags = projectileIgnoreTags;

            projectile.GetComponent<Rigidbody>().velocity = direction * bombProjectileSpeed;
        }

        /// <summary>
        /// Fires a plasma projectile at the current target if one exists.
        /// </summary>
        public void FirePlasmaProjectileAtTarget()
        {
            if (targetTransform == null)
            {
                return;
            }

            FirePlasmaProjectileAtPosition(targetTransform.position);
        }

        /// <summary>
        /// Fires a plasma projectile at a given position.
        /// </summary>
        /// <param name="_position"></param>
        public void FirePlasmaProjectileAtPosition(Vector3 _position)
        {
            var direction = (_position - _transform.position).normalized;
            var projectile = Instantiate(plasmaProjectilePrefab, _transform.position, _transform.rotation);

            GetComponent<AudioSource>().clip = batPlasma;
            GetComponent<AudioSource>().Play();

            projectile.GetComponent<BatProjectileController>().ignoreTags = projectileIgnoreTags;

            projectile.GetComponent<Rigidbody>().velocity = direction * plasmaProjectileSpeed;
        }

        public void StartMovement()
        {
            StartCoroutine(BatBasicMovement());
        }

        /// <summary>
        /// Coroutine for handling the movement around the room for the bat, special attack timings and basic attack timings.
        /// </summary>
        /// <returns></returns>
        private IEnumerator BatBasicMovement()
        {
            // index of floating points the bat is currently moving towards.
            var currentFloatingPointIndex = 0;

            var timeSinceLastSpecialAttack = 0.0f;
            var specialAttackTime = Random.Range(minDelayBetweenAttacks, maxDelayBetweenAttacks);

            var timeSinceLastBasicAttack = 0.0f;
            var basicAttackTime = Random.Range(minBasicAttackDelay, maxBasicAttackDelay);

            // the main while loop for handling the functionality of this coroutine
            while (true)
            {
                timeSinceLastSpecialAttack += Time.deltaTime;
                timeSinceLastBasicAttack += Time.deltaTime;

                // Handle special attack timing.
                if (timeSinceLastSpecialAttack >= specialAttackTime)
                {
                    timeSinceLastSpecialAttack = 0.0f;
                    specialAttackTime = Random.Range(minDelayBetweenAttacks, maxDelayBetweenAttacks);
                    var attack = Random.Range(0, 2);

                    // Select the special attack to use, and yield this coroutine until it finished execution.
                    switch (attack)
                    {
                        case 0:
                            yield return FlamethrowerAttack();
                            break;

                        case 1:
                            yield return BombardAttack();
                            break;

                        default:
                            yield return FlamethrowerAttack();
                            break;
                    }
                }

                rotateToTarget = true;

                var pos = _transform.position;
                var targetPoint = floatingPositions[currentFloatingPointIndex];

                pos = Vector3.MoveTowards(pos, targetPoint, batMoveSpeed * Time.deltaTime);
                _transform.position = pos;

                // Handle basic movement
                if (Vector3.Distance(_transform.position, targetPoint) <= 0.15f)
                {
                    if (currentFloatingPointIndex == floatingPositions.Length - 1)
                    {
                        currentFloatingPointIndex = 0;
                    }
                    else
                    {
                        currentFloatingPointIndex++;
                    }
                }

                // Handle basic attack
                if (timeSinceLastBasicAttack >= basicAttackTime)
                {
                    timeSinceLastBasicAttack = 0.0f;
                    basicAttackTime = Random.Range(minBasicAttackDelay, maxBasicAttackDelay);

                    var firePos = targetTransform.position + Vector3.up * 0.5f;

                    batAnimator.SetBool("spit", true);
                    yield return new WaitForSeconds(0.4f);
                    FirePlasmaProjectileAtPosition(firePos);
                    batAnimator.SetBool("spit", false);
                }

                yield return null;
            }
        }

        /// <summary>
        /// Coroutine for handling the bombard special attack.
        /// </summary>
        /// <returns></returns>
        private IEnumerator BombardAttack()
        {
            var bombardPosition = _transform.position.x <= targetTransform.position.x ? leftSideBombardPosition : rightSideBombardPosition;

            while (true)
            {
                _transform.position = Vector3.MoveTowards(_transform.position, bombardPosition, batMoveToBombardSpeed * Time.deltaTime);

                if (_transform.position == bombardPosition)
                {
                    break;
                }

                yield return null;
            }

            yield return new WaitForSeconds(1.0f);

            var targetPosition = targetTransform.position;
            var projectilePosition = targetPosition;
            // Offset the initial shot based on side the bat is on
            projectilePosition.x += targetPosition.x <= _transform.position.x ? -2.0f : 2.0f;
            var projectileFireCount = 0;

            while (true)
            {
                if (projectileFireCount >= bombardProjectileCount)
                {
                    yield break;
                }

                var sideModifier = targetPosition.x <= _transform.position.x ? 1.0f : -1.0f;
                projectilePosition.x -= projectileFireCount * bombardProjectileSpacing * sideModifier;

                FireBombProjectileAtPosition(projectilePosition);

                projectileFireCount++;
                yield return new WaitForSeconds(delayBetweenProjectileFire);
            }
        }

        /// <summary>
        /// Coroutine for handling the flamethrower special attack.
        /// </summary>
        /// <returns></returns>
        private IEnumerator FlamethrowerAttack()
        {
            var pos = _transform.position;
            rotateToTarget = false;

            while (true)
            {
                RotateToPosition(flameAttackPosition);
                pos = Vector3.MoveTowards(pos, flameAttackPosition, flameMoveToSpeed * Time.deltaTime);
                _transform.position = pos;

                if (_transform.position == flameAttackPosition)
                {
                    break;
                }

                yield return null;
            }

            // Position the flame attack starts.
            var flameThrowerLookAtPos = _transform.position.x >= targetTransform.position.x ? leftSideFlameInitialPosition : rightSideFlameInititalPosition;

            // Position the flame attack ends.
            var flameThrowerTargetLookAt = _transform.position.x >= targetTransform.position.x ? leftSideFlameEndPosition : rightSideFlameEndPosition;

            batAnimator.SetBool("flamethrower", true);
            yield return new WaitForSeconds(0.5f);

            flameThrowerObject.GetComponentInChildren<ParticleSystem>().Play(true);
            //SoundFX
            GetComponent<AudioSource>().clip = flamethrower;
            GetComponent<AudioSource>().Play();

            while (true)
            {
                RotateToPosition(flameThrowerLookAtPos);
                flameThrowerObject.transform.LookAt(flameThrowerLookAtPos);
                flameThrowerLookAtPos = Vector3.MoveTowards(flameThrowerLookAtPos, flameThrowerTargetLookAt, flameThrowerRotateSpeed * Time.deltaTime);

                if (flameThrowerLookAtPos == flameThrowerTargetLookAt)
                {
                    break;
                }

                yield return null;
            }

            // Finished flamethrower attack.
            ResetFlamethrower();
            rotateToTarget = true;
        }

        /// <summary>
        /// Resets the flamethrower object ready for the next use.
        /// </summary>
        private void ResetFlamethrower()
        {
            var flameThrowerRotation = flameThrowerObject.transform.rotation.eulerAngles;
            flameThrowerRotation.y = 90.0f;
            flameThrowerRotation.x = 0.0f;
            flameThrowerRotation.z = 0.0f;
            flameThrowerObject.transform.rotation = Quaternion.Euler(flameThrowerRotation);

            batAnimator.SetBool("flamethrower", false);
            flameThrowerObject.GetComponentInChildren<ParticleSystem>().Stop(true);
        }

        private IEnumerator StartDeath()
        {
            rotateToTarget = false;

            batAnimator.SetBool("death", true);

            RaycastHit hitData;
            var ray = new Ray(_transform.position + Vector3.down * 2.0f, Vector3.down);
            var hit = Physics.Raycast(ray, out hitData, 100.0f, deathRayMask);

            GetComponent<AudioSource>().clip = deathSound;
            GetComponent<AudioSource>().Play();

            if (hit)
            {
                Debug.DrawLine(ray.origin, hitData.point, Color.red, 5.0f);
                var initialPos = _transform.position;
                var t = 0.0f;

                while (true)
                {
                    _transform.position = Vector3.Lerp(initialPos, hitData.point, t);

                    t += Time.deltaTime / deathTime;

                    if (t >= 1.0f) break;

                    yield return null;
                }
            }
        }

        public void OnTriggerStay(Collider _collider)
        {
            if (currentHealth <= 0) return;

            if (_collider.gameObject.CompareTag("PlayerMain") && !invulnerable)
            {
                var player = _collider.gameObject.GetComponent<Player>();

                if (player.Dashing)
                {
                    currentHealth--;
                    if (currentHealth <= 0)
                    {
                        FindObjectOfType<BatBossManager>().EndFight();
                        StopAllCoroutines();
                        ResetFlamethrower();
                        StartCoroutine(StartDeath());
                    }
                    else
                    {
                        invulnerable = true;
                        StopAllCoroutines();
                        ResetFlamethrower();
                        FindObjectOfType<BatBossManager>().StartNextStage();
                    }
                }
            }
        }

        public void OnDrawGizmos()
        {
            if (drawMovementGizmos)
            {
                Gizmos.color = Color.cyan;

                foreach (var point in floatingPositions)
                {
                    Gizmos.DrawCube(point, Vector3.one * 0.25f);
                }
            }

            if (drawBombardAttackGizmos)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(leftSideBombardPosition, Vector3.one);
                Gizmos.DrawCube(rightSideBombardPosition, Vector3.one);
            }

            if (drawFlameThrowerAttackGizmos)
            {
                // Cube for showing the attack position for the flame thrower attack.
                Gizmos.color = new Color32(255, 165, 0, 255);
                Gizmos.DrawCube(flameAttackPosition, Vector3.one * 0.5f);

                // Cubes to show start and end for each side of flame thrower attack.
                Gizmos.DrawCube(leftSideFlameInitialPosition, Vector3.one * 0.5f);
                Gizmos.DrawCube(rightSideFlameInititalPosition, Vector3.one * 0.5f);
                Gizmos.DrawCube(leftSideFlameEndPosition, Vector3.one * 0.5f);
                Gizmos.DrawCube(rightSideFlameEndPosition, Vector3.one * 0.5f);

                // Line to show attack position to initial position for each side of flame thrower attack.
                Gizmos.color = Color.green;
                Gizmos.DrawLine(flameAttackPosition, leftSideFlameInitialPosition);
                Gizmos.DrawLine(flameAttackPosition, rightSideFlameInititalPosition);

                // Line to show the attack start and end for each side of flame thrower attack.
                Gizmos.color = Color.red;
                Gizmos.DrawLine(leftSideFlameInitialPosition, leftSideFlameEndPosition);
                Gizmos.DrawLine(rightSideFlameInititalPosition, rightSideFlameEndPosition);
            }

            if (targetTransform != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, targetTransform.position);
            }
        }
    }
}