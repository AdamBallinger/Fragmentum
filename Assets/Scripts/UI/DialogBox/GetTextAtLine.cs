using UnityEngine;

namespace Assets.Scripts.UI.DialogBox
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

        public void Trigger()
        {
            theText = textBox.controllerActive ? controllerText : keyboardText;
            textBox.ReloadScript(theText);
            textBox.currentLine = startAtLine;
            textBox.endLine = endAtLine;
            textBox.EnableTextBox();
        }

        private void OnTriggerEnter(Collider other)
        {
            Trigger();

            if (destroyWhenActive)
            {
                Destroy(gameObject);
            }
        }
    }
}