using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts
{

    public class ButtonController : MonoBehaviour
    {

        // The button object that will be pressed down.
        public Transform buttonTransform;
        public float buttonPressSpeed = 1.0f;

        // If true then the button will remain down and red until told by code to return to normal state.
        public bool remainActiveUntilFinished = false;

        public List<ButtonController> disableWhenActive;

        [SerializeField]
        private ButtonEvent onButtonPress = null;

        [SerializeField]
        private UnityEvent onButtonPressedContinuous = null;

        [SerializeField]
        private UnityEvent onButtonReleased = null;

        private Renderer buttonRenderer;

        private float buttonNormalY = 0.7f;
        private float buttonPressedY = 0.2f;

        private bool isPressed = false;

        [HideInInspector]
        public bool locked = false;

        public void Start()
        {
            buttonRenderer = buttonTransform.gameObject.GetComponent<Renderer>();
        }

        public void PressButton()
        {
            //Debug.Log("Press");
            StopAllCoroutines();
            StartCoroutine(ButtonDown());
            isPressed = true;

            foreach (var controller in disableWhenActive)
            {
                controller.locked = true;
            }
        }

        private IEnumerator ButtonDown()
        {
            buttonRenderer.material.SetColor("_EmissionColor", new Color(0.4f, 0.0f, 0.0f));

            while (true)
            {
                var buttonPos = buttonTransform.localPosition;

                if (buttonPos.y == buttonPressedY)
                {
                    yield break;
                }

                buttonPos.y = Mathf.MoveTowards(buttonPos.y, buttonPressedY, buttonPressSpeed * Time.deltaTime);
                buttonTransform.localPosition = buttonPos;

                yield return null;
            }
        }

        public void ReleaseButton()
        {
            //Debug.Log("Release");
            StopAllCoroutines();
            StartCoroutine(ButtonUp());
            isPressed = false;

            foreach (var controller in disableWhenActive)
            {
                controller.locked = false;
            }
        }

        private IEnumerator ButtonUp()
        {
            buttonRenderer.material.SetColor("_EmissionColor", new Color(0.0f, 0.0f, 0.4f));

            while (true)
            {
                var buttonPos = buttonTransform.localPosition;

                if (buttonPos.y == buttonNormalY)
                {
                    yield break;
                }

                buttonPos.y = Mathf.MoveTowards(buttonPos.y, buttonNormalY, buttonPressSpeed * Time.deltaTime);
                buttonTransform.localPosition = buttonPos;

                yield return null;
            }
        }

        public void OnTriggerEnter(Collider _collider)
        {
            if (locked) return;

            if (_collider.gameObject.tag == "Player")
            {
                if (onButtonPress != null && !isPressed)
                    onButtonPress.Invoke(this);

                PressButton();
            }
        }

        public void OnTriggerStay(Collider _collider)
        {
            if (locked) return;

            if (_collider.gameObject.tag == "Player")
            {
                if (onButtonPressedContinuous != null)
                    onButtonPressedContinuous.Invoke();
            }
        }

        public void OnTriggerExit(Collider _collider)
        {
            if (locked) return;

            if (_collider.gameObject.tag == "Player")
            {
                if (onButtonReleased != null)
                    onButtonReleased.Invoke();

                if (!remainActiveUntilFinished)
                    ReleaseButton();
            }
        }
    }
}
