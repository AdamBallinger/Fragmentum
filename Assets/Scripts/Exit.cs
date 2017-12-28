using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts
{
    public class Exit : MonoBehaviour
    {

        public string sceneName;

        private bool loading = false;

        private void OnTriggerEnter(Collider _collider)
        {
            if (!_collider.tag.Contains("Player")) return;

            if(!loading)
            {
                loading = true;
                // Load with the loading screen.
                FindObjectOfType<GlobalUIController>().LoadLevel(sceneName);
            }           
        }
    }
}
