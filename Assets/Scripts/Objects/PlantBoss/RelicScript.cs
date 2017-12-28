using System.Collections;
using Assets.Scripts.Character;
using UnityEngine;

namespace Assets.Scripts.Objects.PlantBoss
{
    public class RelicScript : MonoBehaviour
    {
        public GameObject exit;
        public GameObject aquaphobiaHUD;
        public Player player;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("PlayerMain"))
            {
                exit.SetActive(true);
                StartCoroutine(ShowPhobia());
            }
        }

        public IEnumerator ShowPhobia()
        {
            player.controlsEnabled = false;
            aquaphobiaHUD.SetActive(true);

            yield return new WaitForSeconds(2);

            aquaphobiaHUD.SetActive(false);
            player.controlsEnabled = true;

            yield return null;
        }
    }
}
