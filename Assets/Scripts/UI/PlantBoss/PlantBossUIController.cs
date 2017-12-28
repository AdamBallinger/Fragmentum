using Assets.Scripts.AI.Bosses.Bat;
using Assets.Scripts.AI.Bosses.Plant;
using Assets.Scripts.BossManagers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.BatBoss
{
    public class PlantBossUIController : MonoBehaviour
    {

        public Image health;

        public PlantBossManager bossManager;

        private PlantBossController bossController;

        public void Start()
        {
            bossController = bossManager.plantBossObject.GetComponent<PlantBossController>();
        }

        public void Update()
        {
            if (bossController != null)
            {
                health.fillAmount = Mathf.MoveTowards(health.fillAmount, (1.0f / bossController.maxHealth) * bossController.currentHealth, 1.0f * Time.deltaTime);
            }
        }

    }
}
