using System.Collections;
using Assets.Scripts.BossManagers;
using Assets.Scripts.Character;
using Assets.Scripts.Objects.BatBoss;
using UnityEngine;
using Assets.Scripts.AI.Bosses.Bat;
using Assets.Scripts.CameraUtils;

namespace Assets.Scripts.Misc.BatBoss
{
    public class BossIntroController : MonoBehaviour
    {
        [Tooltip("If checked, the intro will be skipped")]
        public bool fastForwardIntro = false;

        public BatBossManager bossManager;

        public GateController gate;

        public CameraCutscene cutscene;

        public GameObject playerObject;
        public Player playerControls;

        public float delayForMainTrack = 19.0f;
        public AudioSource loopTrack;

        public ButtonController[] buttons;

        private void Start()
        {
            if (!fastForwardIntro)
            {
                foreach (var button in buttons)
                {
                    button.locked = true;
                }

                Invoke("PlayMainTrack", delayForMainTrack);
                StartCoroutine(SetupFight());
            }
            else
            {
                playerObject.transform.position = new Vector3(-8.0f, 0.85f, -3.4f);
                playerControls.controlsEnabled = true;
                gate.CloseGate();
                PlayMainTrack();

                foreach(var bonfire in bossManager.bonfires)
                {
                    bonfire.strength = 1.0f;
                }

                bossManager.batBossObject.transform.position = bossManager.bossSpawnDroptoPosition;
                bossManager.batBossObject.GetComponent<BatBossController>().StartMovement();
                bossManager.StartCoroutine(bossManager.MinionSpawnCoroutine());
            }
        }

        /// <summary>
        /// Coroutine for setting up the boss fight when the scene loads. 
        /// Controls everything from the player running into the scene, and the minions coming in lighting up the bonfires and lowering the boss into the level.
        /// </summary>
        /// <returns></returns>
        private IEnumerator SetupFight()
        {
            playerControls.controlsEnabled = false;
            playerControls.AlwaysGrounded = true;

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

            yield return bossManager.SetupBonfiresAndBoss();

            foreach(var button in buttons)
            {
                button.locked = false;
            }
        }

        private IEnumerator StartPlayerEntrance()
        {
            var position = playerObject.transform.position;

            while (true)
            {
                position.x = Mathf.MoveTowards(position.x, -8.0f, 5.0f * Time.deltaTime);
                playerObject.transform.position = position;

                if(position.x == -8.0f)
                {
                    gate.CloseGate();
                    playerObject.GetComponent<Animator>().SetBool("isRunning", false);
                    break;
                }

                yield return null;
            }
        }

        public void PlayMainTrack()
        {
            loopTrack.Play();
        }

        public void OnCutsceneFinish()
        {
            playerControls.controlsEnabled = true;
            playerControls.AlwaysGrounded = false;
            bossManager.batBossObject.GetComponent<BatBossController>().StartMovement();
        }
    }
}
