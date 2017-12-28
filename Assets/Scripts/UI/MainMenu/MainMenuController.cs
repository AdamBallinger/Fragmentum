using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {

        public GameObject mainMenu;
        public GameObject levelSelect;

        public Button[] menuButtons;

        private GlobalUIController uiController;

        private int currentControllerSelection;
        private bool dpadDown;

        private void Start()
        {
            uiController = FindObjectOfType<GlobalUIController>();

            if (uiController == null)
            {
                Debug.LogWarning("No Global UI Controller present in scene. Make sure to run from bootstrap!");
            }
        }

        public void OnPlayClick()
        {
            uiController.LoadLevel("Castle1st"); // this scene needs renaming..
        }

        public void OnLevelSelectClick()
        {
            levelSelect.SetActive(true);
            mainMenu.SetActive(false);
        }

        public void OnOptionsClick()
        {
            uiController.optionsUI.SetActive(true);
            mainMenu.SetActive(false);
        }

        public void OnQuitClick()
        {
            Application.Quit();
        }

        private void Update()
        {
            if (uiController == null || !mainMenu.activeSelf || FindObjectOfType<MainMenuIntroController>().IntroPlaying)
            {
                return;
            }

            foreach (var button in menuButtons)
            {
                button.transform.Find("Highlight").GetComponent<Image>().enabled = false;
            }

            if (uiController.CurrentInputDevice == InputDevice.Controller)
            {
                menuButtons[currentControllerSelection].transform.Find("Highlight").GetComponent<Image>().enabled = true;
            }

            // Pressed Up on DPAD.
            if (Input.GetAxisRaw("DPadY") > 0.0f && !dpadDown)
            {
                dpadDown = true;
                if (currentControllerSelection == 0)
                {
                    currentControllerSelection = menuButtons.Length - 1;
                    return;
                }

                currentControllerSelection--;
            }

            // Pressed Down on DPAD.
            if (Input.GetAxisRaw("DPadY") < 0.0f && !dpadDown)
            {
                dpadDown = true;
                currentControllerSelection++;
                currentControllerSelection = currentControllerSelection % menuButtons.Length;
            }

            // DPAD released.
            if (Input.GetAxisRaw("DPadY") == 0.0f)
            {
                dpadDown = false;
            }

            // A button on controller.
            if (Input.GetButtonDown("Submit"))
            {
                menuButtons[currentControllerSelection].onClick.Invoke();
            }
        }
    }
}
