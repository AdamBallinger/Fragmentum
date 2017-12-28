using Assets.Scripts.CameraUtils;
using Assets.Scripts.Character;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.AI.Bosses.Plant
{
    public class PlantBossController : MonoBehaviour
    {
        public bool isActive;
        private bool justTookDamage;
        private float recoveryTimer;
        private int recoveryTime = 6000;

        private bool isVulnerable;
        private int vulnerableTime = 3000;
        private float vulnerableTimer;
        private int calldownCount;
        private bool counterAttack;

        private int otherAttackCount;

        public GameObject waterfallObject;

        private const int totalAttackTypes = 7;

        private enum AttackType
        {
            Spit,
            Calldown,
            Acid,
            PersistantSpitAttack,
            PersistantCalldownAttack,
            PersistantAcidAttack,
        }

        private enum AttackPhase
        {
            One,
            Two,
            Three,
        }

        private AttackPhase currentAttackPhase = AttackPhase.One;

        private AttackType[] phaseOneAttacks =
        {
            AttackType.Spit,
        };

        private AttackType[] phaseTwoAttacks =
        {
            AttackType.PersistantSpitAttack,
            AttackType.Acid,
        };

        private AttackType[] phaseThreeAttacks =
        {
            AttackType.PersistantSpitAttack,
            AttackType.Acid,
            AttackType.PersistantAcidAttack,
        };

        [Header("Stats")]
        public int maxHealth = 3;
        public int currentHealth;

        [Header("Object References")]
        public GameObject spitProjectilePrefab;
        public GameObject rockProjectilePrefab;
        public GameObject acidSpoutPrefab;
        public GameObject plantModelParent;
        public GameObject shieldObject;

        public GameObject target;

        private System.Random random;

        private AttackType nextAttackType = AttackType.Spit;

        [Header("Global attack settings")]
        public int minTimeBetweenAttacks = 4000;
        public int maxTimeBetweenAttacks = 6000;
        public int attackCoolDown = 2000;
        private float attackTimer;

        [Header("SeedAttack")]
        public int seedAttackCooldown = 2000;

        [Header("PetalAttack")]
        public int petalAttackWarmupTime = 2000;
        public int petalAttackCooldown = 4000;

        [Header("Acid attack")]
        public int acidAttackCooldown = 6000;

        [Header("LungeAttack")]
        public int telegraphLungeTime = 1000;
        public int lungeSpeed = 16;
        public int maxAttacksBeforeLunge = 4;
        public int minAttacksBeforeLunge = 2;

        [Header("Projectile Settings")]
        public float seedProjectileSpeed = 64;
        public float rockProjectileSpeed = 6;

        [Header("Animation Timings")]
        public float spitAnimationAttackDelay = 0.8f;
        public float roarAnimationAttackDelay = 0.8f;

        public Animator animator;

        public GameObject mainCamera;

        //sounds
        public AudioClip roar;
        public AudioClip spit;
        public AudioClip roarDown;

        private Transform _transform;
        private Transform targetTransform;

        private void Start()
        {
            _transform = transform;
            targetTransform = target.transform;

            currentHealth = maxHealth;
            random = new System.Random(DateTime.Now.Millisecond);
            animator = plantModelParent.GetComponent<Animator>();

            shieldObject?.SetActive(true);
            waterfallObject?.SetActive(false);
        }

        public void StartMovement()
        {
            StartCoroutine(StartDelayCoroutine());
        }

        private IEnumerator StartDelayCoroutine()
        {
            yield return new WaitForSeconds(1);

            isActive = true;

            StartCoroutine(MainAttackRoutine());
        }

        public void Roar()
        {
            GetComponent<AudioSource>().clip = roar;
            GetComponent<AudioSource>().Play();
            StartCoroutine(RoarCoroutine());
        }

        private void Update()
        {
            if (currentHealth <= 0)
            {
                Die();
            }

            if (justTookDamage)
            {
                recoveryTimer += Time.deltaTime * 1000;

                if (recoveryTimer > recoveryTime)
                {
                    recoveryTimer = 0.0f;
                    justTookDamage = false;
                }
            }

            if (calldownCount == 3)
            {
                StartCoroutine(TriggerWaterfallCoroutine());
            }

            if (isVulnerable)
            {
                vulnerableTimer += Time.deltaTime * 1000;

                if (vulnerableTimer > vulnerableTime)
                {
                    vulnerableTimer = 0.0f;
                    isVulnerable = false;
                    shieldObject.SetActive(true);
                }
            }

#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.P))
            {
                currentHealth = 0;
            }
#endif
        }

        private void UpdateAttackTimers()
        {
            attackTimer += Time.deltaTime * 1000;
        }

        private bool CanAttack()
        {
            if (attackTimer > attackCoolDown)
            {
                attackTimer = 0;
                return true;
            }

            return false;
        }

        private AttackType GetRandomAttackFromPhase(AttackPhase phase)
        {
            switch (phase)
            {
                case AttackPhase.One:
                    return phaseOneAttacks[random.Next(phaseOneAttacks.Length)];

                case AttackPhase.Two:
                    return phaseTwoAttacks[random.Next(phaseTwoAttacks.Length)];

                case AttackPhase.Three:
                    return phaseThreeAttacks[random.Next(phaseThreeAttacks.Length)];

                default:
                    return AttackType.Spit;
            }
        }

        private int GetRandomAttackCooldown()
        {
            return random.Next(minTimeBetweenAttacks, maxTimeBetweenAttacks);
        }

        private void ChooseTotallyRandomAttack()
        {
            nextAttackType = (AttackType)random.Next(totalAttackTypes);
        }

        private void Attack()
        {
            switch (nextAttackType)
            {
                case AttackType.Spit:
                    StartCoroutine(SpitAttack());
                    break;

                case AttackType.Calldown:
                    StartCoroutine(CalldownAttack());
                    break;

                case AttackType.Acid:
                    StartCoroutine(AcidAttack());
                    break;

                case AttackType.PersistantSpitAttack:
                    PersistantSpitAttack();
                    break;

                case AttackType.PersistantCalldownAttack:
                    StartCoroutine(PersistantCalldownAttack());
                    break;

                case AttackType.PersistantAcidAttack:
                    StartCoroutine(PersistantAcidAttack());
                    break;
            }
        }

        private IEnumerator MainAttackRoutine()
        {
            while (currentHealth > 0)
            {
                currentAttackPhase = (AttackPhase)maxHealth - currentHealth;

                if (isActive && !isVulnerable)
                {
                    if (!CanAttack())
                    {
                        UpdateAttackTimers();
                    }
                    else
                    {
                        if (otherAttackCount >= 2)
                        {
                            nextAttackType = AttackType.Calldown;
                            otherAttackCount = 0;
                        }
                        else if (counterAttack)
                        {
                            nextAttackType = AttackType.PersistantAcidAttack;
                            otherAttackCount = 0;
                            counterAttack = false;
                        }
                        else
                        {
                            nextAttackType = GetRandomAttackFromPhase(currentAttackPhase);
                            otherAttackCount++;
                        }

                        Attack();
                    }
                }
                yield return null;
            }
        }

        private void Die()
        {
            StopAllCoroutines();

            animator.SetBool("Dead", true);

            shieldObject.SetActive(false);
            isActive = false;
        }

        private IEnumerator RoarCoroutine()
        {
            animator.SetBool("Roaring", true);

            yield return null;

            animator.SetBool("Roaring", false);
        }

        private IEnumerator SpitAttack()
        {
            animator.SetBool("Spitting", true);
            yield return new WaitForSeconds(spitAnimationAttackDelay);
            animator.SetBool("Spitting", false);

            GetComponent<AudioSource>().clip = spit;
            GetComponent<AudioSource>().Play();

            for (var i = 0; i < 3; i++)
            {
                if (target != null)
                {
                    var direction = (targetTransform.position - _transform.position).normalized;
                    var projectile = Instantiate(spitProjectilePrefab, _transform.position, _transform.rotation);

                    projectile.GetComponent<Rigidbody>().velocity = direction * seedProjectileSpeed;
                }

                yield return new WaitForSeconds(0.25f);
            }

            yield return null;
        }

        private void PersistantSpitAttack()
        {
            StartCoroutine(SpitAttack());
        }

        private IEnumerator CalldownAttack()
        {
            mainCamera.GetComponent<CameraShake>().StartShake(4.0f);

            animator.SetBool("Roaring", true);

            GetComponent<AudioSource>().clip = roarDown;
            GetComponent<AudioSource>().Play();

            yield return new WaitForSeconds(roarAnimationAttackDelay);

            if (target != null)
            {
                var position = targetTransform.position;
                position.y = 10;

                var direction = Vector3.down;

                var position01 = position;
                position01.x -= 4;

                var position02 = position;
                position02.x += 4;

                var projectile = Instantiate(rockProjectilePrefab, position, _transform.rotation);
                var projectile1 = Instantiate(rockProjectilePrefab, position01, _transform.rotation);
                var projectile2 = Instantiate(rockProjectilePrefab, position02, _transform.rotation);

                projectile.GetComponent<Rigidbody>().velocity = direction * rockProjectileSpeed;
                projectile1.GetComponent<Rigidbody>().velocity = direction * rockProjectileSpeed;
                projectile2.GetComponent<Rigidbody>().velocity = direction * rockProjectileSpeed;
            }

            animator.SetBool("Roaring", false);
            calldownCount++;
            yield return null;
        }

        private IEnumerator TriggerWaterfallCoroutine()
        {
            calldownCount = 0;

            GetComponent<AudioSource>().clip = roar;
            GetComponent<AudioSource>().Play();

            waterfallObject.SetActive(true);
            yield return new WaitForSeconds(1.75f);
            shieldObject.SetActive(false);
            waterfallObject.SetActive(false);
            isVulnerable = true;
        }

        private Quaternion GetRandomQuarternion()
        {
            return Quaternion.Euler(UnityEngine.Random.Range(0.0f, 360.0f), UnityEngine.Random.Range(0.0f, 360.0f), UnityEngine.Random.Range(0.0f, 360.0f));
        }

        private IEnumerator PersistantCalldownAttack()
        {
            animator.SetBool("Roaring", true);

            GetComponent<AudioSource>().clip = roarDown;
            GetComponent<AudioSource>().Play();

            for (var i = 0; i < 4; i++)
            {
                var position = targetTransform.position;
                var direction = Vector3.down;
                var projectile = Instantiate(rockProjectilePrefab, position, GetRandomQuarternion());

                yield return new WaitForSeconds(1);

                projectile.GetComponent<Rigidbody>().velocity = direction * rockProjectileSpeed;

                yield return new WaitForSeconds(0.75f);
            }

            animator.SetBool("Roaring", false);
            yield return null;
        }

        private IEnumerator AcidAttack()
        {
            animator.SetBool("Roaring", true);
            mainCamera.GetComponent<CameraShake>().StartShake(2.0f);

            yield return new WaitForSeconds(roarAnimationAttackDelay);
            animator.SetBool("Roaring", false);

            GetComponent<AudioSource>().clip = roarDown;
            GetComponent<AudioSource>().Play();

            if (target != null)
            {
                var position = targetTransform.position;
                position.z -= 1;
                position.x += 4;
                position.y = -6;
                Instantiate(acidSpoutPrefab, position, new Quaternion(0, 0, 0, 0));
            }


            yield return null;
        }

        private IEnumerator PersistantAcidAttack()
        {
            mainCamera.GetComponent<CameraShake>().StartShake(6.0f);

            GetComponent<AudioSource>().clip = roarDown;
            GetComponent<AudioSource>().Play();

            for (var i = 0; i < 4; i++)
            {
                StartCoroutine(AcidAttack());

                yield return new WaitForSeconds(0.5f);
            }
        }

        private IEnumerator TakeDamage()
        {
            animator.SetBool("Damaged", true);
            yield return null;
            animator.SetBool("Damaged", false);
        }

        public void OnDamageFinish()
        {
            animator.SetBool("Damaged", false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("PlayerMain") && isActive)
            {
                var player = other.GetComponent<Player>();
                if (player.isDashing || player.transform.position.y > _transform.position.y && player.velocity.y < 0)
                {
                    if (!justTookDamage && isVulnerable)
                    {
                        currentHealth--;
                        justTookDamage = true;
                        counterAttack = true;

                        if (currentHealth > 0)
                        {
                            StartCoroutine(TakeDamage());
                            shieldObject.SetActive(true);
                        }
                        else if (currentHealth == 0)
                        {
                            isActive = false;
                        }
                    }

                    player.BounceOffEnemyHead();
                }
            }
        }
    }
}