using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class PauseMenu : MonoBehaviour
    {

        public Button[] buttons;

        private GlobalUIController uiController;

        private int currentControllerSelection;
        private bool dpadDown;

        private void Start()
        {
            uiController = FindObjectOfType<GlobalUIController>();
        }

        private void OnEnable()
        {
            currentControllerSelection = 0;
        }

        public void Resume()
        {
            Time.timeScale = 1.0f;
            uiController.pauseUI.SetActive(false);
        }

        public void MainMenu()
        {
            uiController.LoadLevel("MainMenu");
            Time.timeScale = 1.0f;
            uiController.pauseUI.SetActive(false);
        }

        public void Options()
        {
            uiController.pauseUI.SetActive(false);
            uiController.optionsUI.SetActive(true);
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
        }
    }
}
