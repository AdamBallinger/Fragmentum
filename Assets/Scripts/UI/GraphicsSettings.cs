using Assets.Shaders.FXAA.Scripts;
using Assets.Standard_Assets.Effects.ImageEffects.Scripts;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

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

        private int currentControllerSelection;
        private bool dpadDown ;

        private void Start()
        {
            uiController = FindObjectOfType<GlobalUIController>();
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
            return cameraRootObj == null ? null : cameraRootObj.transform.Find("Camera").gameObject;
        }

        private void Update()
        {
            if(GetStandardizedCamera() != null)
            {
                // Update camera post fx settings
                GetStandardizedCamera().GetComponent<FXAA>().enabled = antiAliasing;
                //GetStandardizedCamera().GetComponent<Antialiasing>().enabled = antiAliasing;
                GetStandardizedCamera().GetComponent<ScreenSpaceAmbientOcclusion>().enabled = ambientOcclusion;
                GetStandardizedCamera().GetComponent<Bloom>().enabled = bloom;
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