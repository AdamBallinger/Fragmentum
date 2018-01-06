using Assets.Scripts.Character;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI.PlayerHUD
{
    public class PlayerHUDController : MonoBehaviour
    {

        public Text playerScore;

        public Image healthBar;
        public Image staminaBar;

        private Player player;

        public GameObject defaultHealthStamina;
        public GameObject visorHealthStamina;

        private void Start()
        {
            
            player = FindObjectOfType<Player>();
        }

        private void Update()
        {
            HUDCheck();

            if (player == null)
            {
                player = FindObjectOfType<Player>();
                return;
            }

            healthBar.fillAmount = Mathf.MoveTowards(healthBar.fillAmount, 1.0f / player.maxHealth * player.Health, 1.0f * Time.deltaTime);
            staminaBar.fillAmount = Mathf.MoveTowards(staminaBar.fillAmount, 1.0f / player.maxStamina * player.Stamina, 1.0f * Time.deltaTime);

            UpdateScoreText();
        }

        private void UpdateScoreText()
        {
            playerScore.text = "Score: " + player.Score;
        }

        private void HUDCheck()
        {
            var sceneName = SceneManager.GetActiveScene().name;

            if (sceneName.Equals("BatBossScene") || sceneName.Equals("PlantBossScene"))
            {
                visorHealthStamina.SetActive(true);
                defaultHealthStamina.SetActive(false);
            }
            else
            {
                visorHealthStamina.SetActive(false);
                defaultHealthStamina.SetActive(true);
            }
        }
    }
}
