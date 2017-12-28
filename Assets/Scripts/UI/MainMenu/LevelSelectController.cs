using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.MainMenu
{
    public class LevelSelectController : MonoBehaviour
    {

        public Button[] buttons;

        private GlobalUIController uiController;

        private int currentControllerSelection;
        private bool dpadDown;

        private void Start()
        {
            uiController = FindObjectOfType<GlobalUIController>();
        }

        public void OnCastleExtClick()
        {
            uiController.LoadLevel("Castle Exterior");
        }

        public void OnForestClick()
        {
            uiController.LoadLevel("The_Forest");
        }

        public void OnFlyTrapClick()
        {
            uiController.LoadLevel("PlantBossScene");
        }

        public void OnCaveClick()
        {
            uiController.LoadLevel("CaveScene");
        }

        public void OnBatClick()
        {
            uiController.LoadLevel("BatBossScene");
        }

        public void OnBackClick()
        {
            StopAllCoroutines();
            StartCoroutine(Hack());
        }

        /// <summary>
        /// Hacky back button because of weird unity logic where deactivating the level select ui for somereason carrys the submit button down
        /// event over to the main menu ui which obviously then reopens the level select ui.. FUN
        /// Needs to wait a frame before activating the main menu ui so it doesn't mess up.
        /// </summary>
        /// <returns></returns>
        private IEnumerator Hack()
        {
            FindObjectOfType<MainMenuController>().levelSelect.SetActive(false);
            yield return null;
            FindObjectOfType<MainMenuController>().mainMenu.SetActive(true);
        }

        private void Update()
        {
            if (uiController == null || !FindObjectOfType<MainMenuController>().levelSelect.activeSelf) return;

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
                //Debug.Log("LEVEL SUB");
                buttons[currentControllerSelection].onClick.Invoke();
            }

            // B button on controller.
            if (Input.GetButtonDown("Block"))
            {
                OnBackClick();
            }
        }
    }
}
