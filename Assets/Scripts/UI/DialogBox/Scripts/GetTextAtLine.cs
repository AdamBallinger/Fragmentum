using UnityEngine;

namespace Assets.Scripts.UI.DialogBox.Scripts
{
    public class GetTextAtLine : MonoBehaviour
    {
        public TextAsset controllerText;
        public TextAsset keyboardText;
        public TextAsset theText;

        public int startAtLine;
        public int endAtLine;

        public TextBoxManager textBox;

        public bool destroyWhenActive;

        private void Start ()
        {
            textBox = FindObjectOfType<TextBoxManager>();
        }

        private void OnTriggerEnter(Collider other)
        {
            theText = textBox.controllerActive ? controllerText : keyboardText;

            textBox.ReloadScript(theText);
            textBox.currentLine = startAtLine;
            textBox.endLine = endAtLine;
            textBox.EnableTextBox();

            if (destroyWhenActive)
            {
                Destroy(gameObject);
            }
        }
    }
}