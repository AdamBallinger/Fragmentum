using UnityEngine;
using UnityEngine.PostProcessing;

namespace Assets.Scripts.Character
{
    public class PlayerPhobia : MonoBehaviour
    {
        public Camera mainCamera;

        public string phobia = "Water Phobia";
        public bool exposed = false;

        public float maxExposure = 10;

        private float exposure;
        private float increaseRate = 0.4f;
        private float decreaseRate = 0.8f;

        private const float defaultExposure = 0.2f;

        private PostProcessingProfile postFX;

        private void Start()
        {
            if (mainCamera == null)
            {
                mainCamera = GameObject.FindGameObjectWithTag("StandardizedCamera").GetComponentInChildren<Camera>();
            }
        }

        private void Update()
        {
            if (exposed)
            {
                if (exposure < maxExposure)
                    exposure += increaseRate;
            }
            else
            {
                if (exposure > 0)
                    exposure -= decreaseRate;
            }

            exposure = Mathf.Min(10, Mathf.Max(0, exposure));

            if (postFX == null)
            {
                postFX = mainCamera.GetComponent<PostProcessingBehaviour>().profile;
                return;
            }

            var settings = postFX.vignette.settings;
            settings.intensity = defaultExposure + exposure;
            postFX.vignette.settings = settings;
        }
    }
}
