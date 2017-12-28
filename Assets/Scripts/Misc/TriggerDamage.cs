using Assets.Scripts.Character;
using UnityEngine;

namespace Assets.Scripts.Misc
{
    public class TriggerDamage : MonoBehaviour
    {
        public int damage = 0;

        private Player player;

        private void Start()
        {
            player = FindObjectOfType<Player>();

            if(player == null)
            {
                Debug.Log("This damage trigger will not damage the player as the Player script couldn't be found.");
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("PlayerMain"))
            {
                player.RemoveHealth(damage);
            }
        }
    }
}
