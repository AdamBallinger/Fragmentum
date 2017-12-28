using UnityEngine;

namespace Assets.Scripts.Character.PlayerCollisions
{
    public class PlayerFootCollisions : MonoBehaviour
    {

        private Player player;

        private void Start()
        {
            player = GetComponentInParent<Player>();
        }

        private void OnTriggerStay(Collider _other)
        {
            var collidingObject = _other.gameObject;

            if (collidingObject.CompareTag("Untagged") || collidingObject.CompareTag("Player"))
            {
                return;
            }

            // Also need this to check if the tag contains the word player because of the PlayerMain and Camera tags. Above checks help to reduce
            // GC Allocation with getting a gameobject tag directly :)
            var collidingTag = collidingObject.tag;
            if (collidingTag.Contains("Player"))
            {
                return;
            }

            collidingObject.SendMessage("OnPlayerFootCollision", player, SendMessageOptions.DontRequireReceiver);
        }
    }
}