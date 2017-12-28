using System.Linq;
using Assets.Scripts.AI.Bosses.Bat;
using Assets.Scripts.BossManagers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.BatBoss
{
    public class BatBossUIController : MonoBehaviour
    {

        public Image batHealth;
        public Image shieldBar;

        public BatBossManager batBossManager;

        private BatBossController bossController;

        private void Start()
        {
            bossController = batBossManager.batBossObject.GetComponent<BatBossController>();
        }

        private void Update()
        {
            if(bossController != null)
            {
                var combinedBonfireStrength = batBossManager.bonfires.Sum(bonfire => bonfire.strength);

                batHealth.fillAmount = Mathf.MoveTowards(batHealth.fillAmount, 1.0f / bossController.maxHealth * bossController.currentHealth, 1.0f * Time.deltaTime);
                shieldBar.fillAmount = Mathf.MoveTowards(shieldBar.fillAmount, 1.0f / 4.0f * combinedBonfireStrength, 1.0f * Time.deltaTime);
            }
        }

    }
}
