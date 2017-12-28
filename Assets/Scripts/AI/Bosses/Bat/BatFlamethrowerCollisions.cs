using Assets.Scripts.Character;
using UnityEngine;

namespace Assets.Scripts.AI.Bosses.Bat
{
    public class BatFlamethrowerCollisions : MonoBehaviour
    {

        [Range(0, 100)]
        public int flamesDamage;

        private Player player;

        private void Start()
        {
            player = FindObjectOfType<Player>();

            if (player == null)
            {
                Debug.LogWarning("Bat flamethrower won't damage player as the Player script couldn't be found.");
            }
        }

        private void OnParticleCollision(GameObject _other)
        {
            if (_other.CompareTag("PlayerMain"))
            {
                player?.RemoveHealth(flamesDamage);
            }
        }
    }
}
