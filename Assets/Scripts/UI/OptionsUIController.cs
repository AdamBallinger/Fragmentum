using Assets.Scripts.UI.MainMenu;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class OptionsUIController : MonoBehaviour
    {
        public Canvas audioCanvas;
        public Canvas graphicsCanvas;

        public Button[] buttons;

        private GlobalUIController uiController;

        private int currentControllerSelection;
        private bool dpadDown;

        private void Start()
        {
            uiController = FindObjectOfType<GlobalUIController>();
        }

        public void Audio()
        {

        }

        public void Graphics()
        {
            graphicsCanvas.gameObject.SetActive(true);
            uiController.optionsUI.SetActive(false);
        }

        public void BackFromOptions()
        {
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                FindObjectOfType<MainMenuController>().mainMenu.SetActive(true);
            }
            else
            {
                uiController.pauseUI.SetActive(true);
            }

            uiController.optionsUI.SetActive(false);
        }

        private void Update()
        {
            if (uiController == null)
            {
                return;
            }

            foreach (var button in buttons)
            {
                button.transform.Find("Highlight").GetComponent<Image>().enabled = false;
            }

            if (uiController.CurrentInputDevice == InputDevice.Controller)
            {
                buttons[currentControllerSelection].transform.Find("Highlight").GetComponent<Image>().enabled = true;
            }

            // Pressed Up on DPAD.
            if (Input.GetAxisRaw("DPadY") > 0.0f && !dpadDown)
            {
                dpadDown = true;
                if (currentControllerSelection == 0)
                {
                    currentControllerSelection = buttons.Length - 1;
                    return;
                }

                currentControllerSelection--;
            }

            // Pressed Down on DPAD.
            if (Input.GetAxisRaw("DPadY") < 0.0f && !dpadDown)
            {
                dpadDown = true;
                currentControllerSelection++;
                currentControllerSelection = currentControllerSelection % buttons.Length;
            }

            // DPAD released.
            if (Input.GetAxisRaw("DPadY") == 0.0f)
            {
                dpadDown = false;
            }

            // A button on controller.
            if (Input.GetButtonDown("Submit"))
            {
                buttons[currentControllerSelection].onClick.Invoke();
            }

            // B button on controller.
            if(Input.GetButtonDown("Block"))
            {
                BackFromOptions();
            }
        }
    }
}
