using UnityEngine;

namespace Assets.Scripts.UI.DialogBox.Scripts
{
    public class PhobiaDisplay : MonoBehaviour
    {
        public Canvas phobiaDisplay;

        public GameObject relic;

        private bool canvasActive;
        private bool relicDestroyed;

        private void Start()
        {
            phobiaDisplay.enabled = false;
            relicDestroyed = false;
            canvasActive = false;
        }

        private void Update()
        {
            DisplayCanvas();
        }

        private void DisplayCanvas()
        {
            if (!relic.activeInHierarchy)
            {
                relicDestroyed = true;

                if (relicDestroyed)
                {
                    phobiaDisplay.enabled = true;
                    canvasActive = true;
                }

                else
                {
                    relicDestroyed = false;
                }
            }

            if (canvasActive)
            {
                Time.timeScale = 0.0f;
                if (Input.anyKeyDown)
                {
                    Time.timeScale = 1.0f;
                    phobiaDisplay.enabled = false;
                    canvasActive = false;
                    Destroy(GameObject.Find("Nyctophobia"));
                }
            }

        }
    }
}


