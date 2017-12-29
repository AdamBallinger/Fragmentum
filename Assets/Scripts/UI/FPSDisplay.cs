using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class FPSDisplay : MonoBehaviour
    {

        public float updateInterval = 0.25f;

        private float accumFPS = 0.0f;
        private int framesDrawn = 0;
        private float intervalTimeLeft = 0.0f;

        private TextMeshProUGUI text;

        private void Start()
        {
            text = GetComponent<TextMeshProUGUI>();

            intervalTimeLeft = updateInterval;
        }

        private void Update()
        {
            intervalTimeLeft -= Time.deltaTime;
            accumFPS += Time.timeScale / Time.deltaTime;
            framesDrawn++;

            if(intervalTimeLeft <= 0.0f)
            {
                text.text = "FPS - " + (accumFPS / framesDrawn).ToString("F2");
                intervalTimeLeft = updateInterval;
                accumFPS = 0.0f;
                framesDrawn = 0;
            }
        }
    }
}
