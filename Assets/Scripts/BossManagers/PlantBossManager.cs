using Assets.Scripts.AI.Bosses.Plant;
using Assets.Scripts.Misc.BatBoss;
using Assets.Scripts.UI.BatBoss;
using UnityEngine;

namespace Assets.Scripts.BossManagers
{
    public class PlantBossManager : MonoBehaviour
    {
        public GameObject plantBossObject;
        public GameObject levelExit;

        public PlantBossController plantAIController;
        public GameObject bossBlockingCollider;
        public PlantBossUIController uiManager;
        public BossDefeatedController defeatedController;
        public GameObject relic;
        public GameObject spotlight;

        private bool cutsceneStarted = false;

        private void Start()
        {
            relic.SetActive(false);
            spotlight.SetActive(false);
            levelExit.SetActive(false);

            if (plantBossObject == null)
            {
                Debug.LogError("No plant boss object given to boss manager!");
                return;
            }

            plantAIController = plantBossObject.GetComponent<PlantBossController>();

            if (plantAIController == null)
            {
                Debug.LogError("No plant boss controller on given plant boss object reference!");
            }
        }

        private void Update()
        {
            if (plantAIController.currentHealth <= 0)
            {
                if (!cutsceneStarted)
                {
                    EndFight();
                    relic.SetActive(true);
                    spotlight.SetActive(true);
                    levelExit.SetActive(true);
                    cutsceneStarted = true;
                }
            }
        }

        public void EndFight()
        {
            uiManager.gameObject.SetActive(false);
            defeatedController.StartCutscene();

            bossBlockingCollider.SetActive(false);
            plantBossObject.SetActive(false);
        }

        public void PlayRoarAnimation()
        {
            plantAIController.Roar();
        }
    }
}