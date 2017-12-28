using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public enum ButtonState
    {
        Toggled_ON,
        Toggled_OFF
    }

    public class ToggleButtonController : MonoBehaviour
    {
        public Image toggleBar;

        // The glow around the button for when controller is used to see which button is selected.
        public Image highlight;

        public float barMoveSpeed = 3.0f;

        public Color onColor;
        public Color offColor;

        public UnityEvent callback;

        //[HideInInspector]
        public ButtonState state = ButtonState.Toggled_ON;

        private RectTransform rectTrans;

        private float onStateX = -40.0f;
        private float offStateX = 40.0f;

        private void Start()
        {
            rectTrans = toggleBar.GetComponent<RectTransform>();
        }

        public void Toggle()
        {
            state = state == ButtonState.Toggled_ON ? ButtonState.Toggled_OFF : ButtonState.Toggled_ON;
            toggleBar.color = state == ButtonState.Toggled_ON ? onColor : offColor;

            callback.Invoke();

            StopAllCoroutines();
            StartCoroutine(MoveBar());
        }

        private IEnumerator MoveBar()
        {
            var targetX = state == ButtonState.Toggled_ON ? onStateX : offStateX;

            while(true)
            {
                var pos = rectTrans.anchoredPosition;
                pos.x = Mathf.MoveTowards(pos.x, targetX, barMoveSpeed * Time.unscaledDeltaTime);
                rectTrans.anchoredPosition = pos;

                if(pos.x == targetX)
                {
                    break;
                }

                yield return null;
            }
        }
    }
}

