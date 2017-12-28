using Assets.Scripts.AI.Bosses.Plant;
using Assets.Scripts.BossManagers;
using Assets.Scripts.CameraUtils;
using Assets.Scripts.Character;
using Assets.Scripts.Objects.BatBoss;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Misc.PlantBoss
{
    public class BossIntroController : MonoBehaviour
    {
        [Tooltip("If checked, the intro will be skipped")]
        public bool fastForwardIntro = false;

        public PlantBossManager bossManager;

        public GateController gate;

        public CameraCutscene cutscene;

        public GameObject playerObject;
        public Player playerControls;

        public GameObject playerSpawn;
        public GameObject introWaypoint;

        public float playerMoveSpeed = 3.0f;

        public void Start()
        {
            if (!fastForwardIntro)
            {
                playerObject.transform.position = playerSpawn.transform.position;

                StartCoroutine(SetupFight());
            }
            else
            {
                playerObject.transform.position = introWaypoint.transform.position;

                playerControls.controlsEnabled = true;
                gate.CloseGate();
            }
        }

        /// <summary>
        /// Coroutine for setting up the boss fight when the scene loads. 
        /// </summary>
        /// <returns></returns>
        private IEnumerator SetupFight()
        {
            playerControls.controlsEnabled = false;
            playerControls.alwaysGrounded = true;

            playerControls.Animator.SetBool("isRunning", true);
            playerControls.Animator.SetFloat("animSpeedMod", 1.0f);

            cutscene.StartCutscene(true);

            // Wait until player entrance coroutine is done.
            yield return StartPlayerEntrance();

            yield return new WaitForSeconds(0.1f);

            playerControls.Animator.SetBool("isVisorDown", true);
            playerControls.Animator.SetBool("isPullingDownVisor", true);
            yield return null;
            playerControls.Animator.SetBool("isPullingDownVisor", false);
        }

        private IEnumerator StartPlayerEntrance()
        {
            var position = playerObject.transform.position;
            var targetPosition = introWaypoint.transform.position;

            while (true)
            {
                position.x = Mathf.MoveTowards(position.x, targetPosition.x, playerMoveSpeed * Time.deltaTime);
                playerObject.transform.position = position;

                if (position.x == targetPosition.x)
                {
                    gate.CloseGate();
                    playerObject.GetComponent<Animator>().SetBool("isRunning", false);
                    break;
                }

                yield return null;
            }
        }

        public void PlayBossRoarAnimation()
        {
            bossManager.PlayRoarAnimation();
        }

        public void OnCutsceneFinish()
        {
            playerControls.controlsEnabled = true;
            playerControls.alwaysGrounded = false;
            bossManager.plantBossObject.GetComponent<PlantBossController>().StartMovement();
        }
    }
}
