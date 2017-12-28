using Assets.Scripts.Character;
using UnityEngine;

namespace Assets.Scripts.Misc
{
    public class PhobiaTrigger : MonoBehaviour
    {
        public virtual void ApplyPhobia(Player player) { }

        public virtual void RemovePhobia(Player player) { }
    }
}
