using System.Collections;
using System.Linq;
using Assets.Scripts.AI.Bosses.Bat;
using Assets.Scripts.CameraUtils;
using Assets.Scripts.Misc.BatBoss;
using Assets.Scripts.Objects.BatBoss;
using UnityEngine;
using Assets.Scripts.UI.BatBoss;

namespace Assets.Scripts.BossManagers
{
    public class BatBossManager : MonoBehaviour
    {
        // Draw gizmos for this script?
        public bool drawGizmos = true;

        // Object reference to the bat boss.
        public GameObject batBossObject;

        public Material batShieldMaterial;

        // Prefab reference to the bat minions that will spawn to relight the fire pits.
        public GameObject batMinionPrefab;

        // Position the boss spawns at.
        public Vector3 bossSpawnPosition = Vector3.zero;

        // Position the boss will move to at the start of the fight when dropping into the scene.
        public Vector3 bossSpawnDroptoPosition = Vector3.zero;

        // Speed the bat moves to the spawn dropto position.
        public float batDroptoSpeed = 1.0f;
        public float batRetreatSpeed = 3.0f;

        // Position in the world the minions will spawn at.
        public Vector3 minionSpawnPosition = Vector3.zero;

        // Where do the minions drop to from their spawn position before moving towards the light (Used for drop in effect).
        public Vector3 minionInitialDroptoPosition = Vector3.zero;

        // List of fire pits keeping bat shield active.
        public BonfireController[] bonfires;

        // Time in seconds between spawning a minion.
        public float minionSpawnTime = 5.0f;

        public GameObject[] sinkingPlatforms;
        public ParticleSystem[] sinkingParticles;
        public AnimationCurve sinkCurve;
        public float platformSinkTime = 1.0f;
        public float platformSinkAmount = 1.0f;

        private bool changingStage;
        private int stage;
        private float shieldStrength;

        private BatBossController batAIController;

        public AudioClip batEnter;

        //private BossIntroController bossIntroController;

        private void Start()
        {
            //bossIntroController = FindObjectOfType<BossIntroController>();

            if (batBossObject == null)
            {
                Debug.LogError("No bat boss object given to boss manager!");
                return;
            }

            batAIController = batBossObject.GetComponent<BatBossController>();

            if (batAIController == null)
            {
                Debug.LogError("No bat boss controller on given bat boss object reference!");
            }
        }

        private void Update()
        {
            // Deactivate shield if all fire pits have been extinguished.
            if (batBossObject == null)
            {
                // boss killed
                StopAllCoroutines();
                return;
            }

            foreach(var bonfire in bonfires)
            {
                shieldStrength += bonfire.strength;
            }

            shieldStrength /= 2.0f;

            //shieldStrength = bonfires.Sum(bonfire => bonfire.strength / 2);
            batShieldMaterial.SetFloat("_Strength", shieldStrength + 0.5f);

            batAIController.invulnerable = shieldStrength > 0.0f || changingStage;

            // Editor only code
#if UNITY_EDITOR
            // Instant bonfire extinguish
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                foreach(var bonfire in bonfires)
                {
                    bonfire.strength = 0.0f;
                }
            }

            // Instant 1 hp (for bat)
            if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                if(batAIController.currentHealth > 0)
                    batAIController.currentHealth = 1;
            }

            // Relight all bonfires instantly
            if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                foreach(var bonfire in bonfires)
                {
                    bonfire.strength = 1.0f;
                }
            }
#endif
        }

        private IEnumerator SetupBonfires()
        {
            foreach (var pit in bonfires)
            {
                var minion = Instantiate(batMinionPrefab, minionSpawnPosition, Quaternion.Euler(0.0f, 90.0f, 0.0f));
                var minionController = minion.GetComponent<BatMinionController>();

                minionController.invulnerable = true;
                minionController.targetPit = pit;
                minionController.moveSpeed *= 3.0f; // move twice speed for initial setup
                minionController.entranceFallPosition = minionInitialDroptoPosition;
                // delay between each spawn so they dont all just spawn inside eachother at once.
                yield return new WaitForSeconds(0.5f);
            }
        }

        private IEnumerator SetupBoss()
        {
            //Play sound
            GetComponent<AudioSource>().clip = batEnter;
            GetComponent<AudioSource>().Play();

            while (true)
            {
                var pos = batBossObject.transform.position;
                pos = Vector3.MoveTowards(pos, bossSpawnDroptoPosition, batDroptoSpeed * Time.deltaTime);
                batBossObject.transform.position = pos;

                if (pos == bossSpawnDroptoPosition)
                {
                    break;
                }

                yield return null;
            }

            StartCoroutine(MinionSpawnCoroutine());
        }

        public IEnumerator SetupBonfiresAndBoss()
        {
            yield return SetupBonfires();
            // Delay before bringing the boss down into scene.
            yield return new WaitForSeconds(7.5f);

            yield return SetupBoss();
        }

        public IEnumerator MinionSpawnCoroutine()
        {
            while (true)
            {
                // Get any fire pits that are extinguished if none exist.
                var extinguishedPit = GetExtinguishedBonfire();

                if (extinguishedPit != null)
                {
                    yield return new WaitForSeconds(minionSpawnTime);

                    var minion = Instantiate(batMinionPrefab, minionSpawnPosition, Quaternion.Euler(0.0f, 90.0f, 0.0f));
                    var minionController = minion.GetComponent<BatMinionController>();

                    minionController.targetPit = extinguishedPit;
                    minionController.entranceFallPosition = minionInitialDroptoPosition;

                    extinguishedPit.hasMinionAttention = true;
                }

                yield return new WaitForSeconds(0.5f);
            }
        }

        public void EndFight()
        {
            StopAllCoroutines();
            FindObjectOfType<BatBossUIController>().gameObject.SetActive(false);

            foreach(var obj in GameObject.FindGameObjectsWithTag("Bat Minion"))
            {
                Destroy(obj);
            }

            FindObjectOfType<BossDefeatedController>().StartCutscene();
        }

        public void StartNextStage()
        {
            StopAllCoroutines();

            // For now just destroy any minions wondering around.
            foreach (var minion in GameObject.FindGameObjectsWithTag("Bat Minion"))
            {
                Destroy(minion);
            }

            StartCoroutine(NextStage());

            if(stage < 2)
            {
                StartCoroutine(SinkPlatform(sinkingPlatforms[stage], sinkingParticles[stage]));
                stage++;
            }       
        }

        private IEnumerator NextStage()
        {
            changingStage = true;
            var currentMoveTarget = bossSpawnDroptoPosition;

            // Move boss back to where he dropped from.
            while (true)
            {
                var pos = batBossObject.transform.position;
                pos = Vector3.MoveTowards(pos, currentMoveTarget, batRetreatSpeed * Time.deltaTime);
                batBossObject.transform.position = pos;

                if (pos == currentMoveTarget)
                {
                    if (currentMoveTarget == bossSpawnDroptoPosition)
                    {
                        currentMoveTarget = bossSpawnPosition;
                    }
                    else if (currentMoveTarget == bossSpawnPosition)
                    {
                        yield return SetupBonfires();
                        yield return new WaitForSeconds(7.5f);
                        changingStage = false;
                        break;
                    }
                }

                yield return null;
            }

            currentMoveTarget = bossSpawnDroptoPosition;

            while (true)
            {
                var pos = batBossObject.transform.position;
                pos = Vector3.MoveTowards(pos, currentMoveTarget, batDroptoSpeed * 2.0f * Time.deltaTime);
                batBossObject.transform.position = pos;

                if (pos == currentMoveTarget)
                {
                    batAIController.invulnerable = false;
                    break;
                }

                yield return null;
            }

            StartCoroutine(MinionSpawnCoroutine());
            batAIController.StartMovement();
        }

        private IEnumerator SinkPlatform(GameObject _platform, ParticleSystem _ps)
        {
            var pos = _platform.transform.position;
            var startY = pos.y;
            var t = 0.0f;

            FindObjectOfType<CameraShake>().StartEndlessShake();
            _ps.Play();

            while(true)
            {
                pos.y = Mathf.Lerp(startY, startY - platformSinkAmount, sinkCurve.Evaluate(t));

                _platform.transform.position = pos;

                t += Time.deltaTime / platformSinkTime;

                if(t >= 1.0f)
                {
                    FindObjectOfType<CameraShake>().StopShaking();
                    _ps.Stop();
                    yield break;
                }

                yield return null;
            }
        }

        /// <summary>
        /// Returns whether all room bonfires are extinguished by the player.
        /// </summary>
        /// <returns></returns>
        public bool CheckAllBonfiresExtinguished()
        {
            return !bonfires.Any(bonfire => bonfire.strength > 0.0f);
        }

        /// <summary>
        /// Return first reference to a bonfire controller that has been extinguished if one exists. Returns null if none are extinguished.
        /// </summary>
        /// <returns></returns>
        public BonfireController GetExtinguishedBonfire()
        {
            return bonfires.FirstOrDefault(bonfire => bonfire.strength <= 0.0f && !bonfire.hasMinionAttention);
        }

        public void OnDrawGizmos()
        {
            if (!drawGizmos)
            {
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(minionSpawnPosition, 0.5f);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(minionInitialDroptoPosition, 0.5f);

            Gizmos.DrawCube(bossSpawnDroptoPosition, Vector3.one * 0.75f);
        }
    }
}
