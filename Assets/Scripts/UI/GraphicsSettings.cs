using UnityEngine;
using UnityEngine.PostProcessing;

namespace Assets.Scripts.UI
{
    public class GraphicsSettings : MonoBehaviour
    {
        public ToggleButtonController[] buttons;

        private bool antiAliasing = true;
        private bool ambientOcclusion = true;
        private bool bloom = true;
        private bool vSync = true;

        private GlobalUIController uiController;

        private PostProcessingProfile postFX;

        private int currentControllerSelection;
        private bool dpadDown ;

        private void Start()
        {
            uiController = FindObjectOfType<GlobalUIController>();
            postFX = GetStandardizedCamera()?.GetComponent<PostProcessingBehaviour>().profile;
        }

        public void OnAntiAliasToggle()
        {
            antiAliasing = !antiAliasing;
        }

        public void OnAmbientOcclusionToggle()
        {
            ambientOcclusion = !ambientOcclusion;
        }

        public void OnBloomToggle()
        {
            bloom = !bloom;
        }

        public void OnVSyncToggle()
        {
            vSync = !vSync;
        }

        public void OnBackClick()
        {
            uiController.optionsUI.SetActive(true);
            uiController.graphicsUI.SetActive(false);
        }

        private GameObject GetStandardizedCamera()
        {
            var cameraRootObj = GameObject.FindGameObjectWithTag("StandardizedCamera");
            return cameraRootObj?.transform.Find("Camera").gameObject;
        }

        private void Update()
        {
            if(postFX != null)
            {
                // Update camera post fx settings
                postFX.antialiasing.enabled = antiAliasing;
                postFX.ambientOcclusion.enabled = ambientOcclusion;
                postFX.bloom.enabled = bloom;
                QualitySettings.vSyncCount = vSync ? 1 : 0;
            }         

            // Reset each 1 every frame so we dont have to manually do it each time the selection changes :P
            foreach (var button in buttons)
            {
                button.highlight.gameObject.SetActive(false);
            }

            if (uiController.CurrentInputDevice == InputDevice.Controller)
            {
                buttons[currentControllerSelection].highlight.gameObject.SetActive(true);
            }

            // Pressed Up on DPAD.
            if (Input.GetAxisRaw("DPadY") > 0.0f && !dpadDown)
            {
                dpadDown = true;
                if (currentControllerSelection == 0) return;

                currentControllerSelection--;
            }

            // Pressed Down on DPAD.
            if (Input.GetAxisRaw("DPadY") < 0.0f && !dpadDown)
            {
                dpadDown = true;
                if (currentControllerSelection == buttons.Length - 1) return;

                currentControllerSelection++;
            }

            // DPAD released.
            if (Input.GetAxisRaw("DPadY") == 0.0f)
            {
                dpadDown = false;
            }

            // A button on controller.
            if (Input.GetButtonDown("Submit"))
            {
                buttons[currentControllerSelection].Toggle();
            }

            // B button on controller.
            if(Input.GetButtonDown("Block") || Input.GetKeyDown(KeyCode.Escape))
            {
                OnBackClick();
            }
        }
    }
}