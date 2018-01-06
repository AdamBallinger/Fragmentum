using System.Collections.Generic;
using Assets.Scripts.Character;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.UI.DialogBox
{
    public class TextBoxManager : MonoBehaviour
    {
        public GameObject textBox;
        public Text textInBox;

        public int currentLine;
        public int endLine;

        public TextAsset theText;
        public TextAsset controllerText;
        public TextAsset keyboardText;
        public string[] textLines;

        public Player player;
        public bool stopMovement;

        public bool renableControlsOnDisable = true;

        public bool isActive;

        public bool controllerActive;
        public bool keyboardActive;

        private InputDevice controls;

        public UnityEvent onFinish;

        private void Start()
        {
            controls = FindObjectOfType<GlobalUIController>().CurrentInputDevice;

            player = FindObjectOfType<Player>();

            if (endLine == 0)
            {
                endLine = textLines.Length - 1;
            }

            if (isActive)
            {
                EnableTextBox();
            }
            else
            {
                DisableTextBox();
            }
        }

        private void Update()
        {
            controls = FindObjectOfType<GlobalUIController>().CurrentInputDevice;

            if (controls == InputDevice.Mouse_Keyboard)
            {
                keyboardActive = true;
                theText = keyboardText;

                if (theText != null)
                {
                    textLines = theText.text.Split('\n');
                }
            }
            else
            {
                controllerActive = true;
                theText = controllerText;

                if (theText != null)
                {
                    textLines = theText.text.Split('\n');
                }
            }

            if (!isActive)
            {
                return;
            }

            textInBox.text = textLines[currentLine];

            if (currentLine > endLine)
            {
                if (textBox.activeSelf)
                {
                    onFinish?.Invoke();

                    DisableTextBox();
                }
            }
            else
            {
                if (Input.anyKeyDown)
                {
                    currentLine += 1;
                }
            }
        }

        public void EnableTextBox()
        {
            isActive = true;
            textBox.SetActive(true);
            player.controlsEnabled = false;
            player.Animator.SetBool("isRunning", false);
        }

        public void DisableTextBox()
        {
            textBox.SetActive(false);

            if (renableControlsOnDisable)
                player.controlsEnabled = true;
        }

        public void ReloadScript(TextAsset _theText)
        {
            if (_theText != null)
            {
                textLines = _theText.text.Split('\n');
            }
        }

    }
}
