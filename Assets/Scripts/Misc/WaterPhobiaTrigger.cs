using Assets.Scripts.Character;
using UnityEngine;

namespace Assets.Scripts.Misc
{
    public class WaterPhobiaTrigger : PhobiaTrigger
    {
        public PlayerPhobia phobia;

        public int staminaDrain = 4;

        public int tickTime = 1000;
        private float tickTimer;
        private bool triggered;

        private void Start()
        {
            if (phobia == null)
            {
                phobia = GameObject.FindGameObjectWithTag("PlayerMain").GetComponent<PlayerPhobia>();
            }
        }

        private void Update()
        {
            if (triggered)
            {
                tickTimer += Time.deltaTime * 1000;
                if (tickTimer > tickTime)
                {
                    tickTimer = 0.0f;
                    triggered = false;
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag.Equals("PlayerMain"))
            {
                if (!triggered)
                {
                    ApplyPhobia(other.gameObject.GetComponent<Player>());
                    triggered = true;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag.Equals("PlayerMain"))
            {
                RemovePhobia(other.gameObject.GetComponent<Player>());
            }
        }

        public override void ApplyPhobia(Player player)
        {
            phobia.exposed = true;

            player.RemoveStamina(staminaDrain);
            player.RemoveHealth(1);
        }

        public override void RemovePhobia(Player player)
        {
            phobia.exposed = false;
        }
    }
}
