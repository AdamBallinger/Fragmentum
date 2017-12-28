using Assets.Scripts.Character;
using UnityEngine;

namespace Assets.Scripts.AI.Enemy.Rock_Beast
{
    public class BeamParticlesCollision : MonoBehaviour
    {
        [Range(0, 100)]
        public int beamsDamage;

        private Player player;

        private void Start()
        {
            player = FindObjectOfType<Player>();

            if(player == null)
            {
                Debug.LogWarning("Beam particles will not damage player as the Player script couldn't be found.");
            }
        }

        private void OnParticleCollision(GameObject _other)
        {
            if (_other.CompareTag("PlayerMain"))
            {
                if(player != null)
                {
                    player.RemoveHealth(beamsDamage);
                }
            }
        }
    }
}
