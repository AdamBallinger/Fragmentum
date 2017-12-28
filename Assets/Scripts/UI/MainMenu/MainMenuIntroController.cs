using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.MainMenu
{
    public class MainMenuIntroController : MonoBehaviour
    {

        public bool skipIntro = false;

        [Space]
        [Header("Object References")]
        public TextMeshProUGUI titlePrimaryText;
        public TextMeshProUGUI titleSecondaryText;
        public TextMeshProUGUI pressToStartText;

        public Image titleUnderline;

        public GameObject[] buttons;

        public GameObject flyTrap;


        [Space(2)]
        [Header("Timer Settings")]
        [Tooltip("Time in seconds that the primary title text and underline take to fade in.")]
        public float primaryFadeIn = 2.0f;

        [Tooltip("Delay before starting the primary fade in.")]
        public float primaryDelay = 1.0f;

        [Tooltip("Time in seconds that the secondary title text takes to fade in.")]
        public float secondaryFadeIn = 2.0f;

        [Tooltip("The delay in seconds before starting the secondary title text fade in.")]
        public float secondaryDelay = 1.5f;

        public float pressToStartPulseSpeed = 2.0f;
        public float pulseMin = 0.2f;

        [Tooltip("Time in seconds that the buttons will take to fade in.")]
        public float buttonMoveIn = 1.0f;

        [Tooltip("Delay in seconds before starting to move in the buttons.")]
        public float buttonsDelay = 2.0f;

        [Tooltip("Time in seconds to wait between moving each button into the screen.")]
        public float buttonDelayBetween = 0.0f;

        [Space(2)]
        [Header("Popup Settings")]
        public float flyTrapUpY = 0.0f;
        public float flyTrapPopupStayTime = 0.0f;
        public float flyTrapPopupDelay = 0.0f;
        public float flyTrapPopupSpeed = 0.0f;

        [Space(2)]
        [Header("Animation Curve Settings")]
        public AnimationCurve primaryCurve;
        public AnimationCurve secondaryCurve;
        public AnimationCurve pressToStartCurve;
        public AnimationCurve buttonCurve;
        public AnimationCurve flyTrapPopupCurve;

        private List<Vector3> buttonFadePositions = new List<Vector3>();

        public bool IntroPlaying { get; private set; }

        private bool pressedToStart;
        private bool waitingForPress;

        private void Awake()
        {
            StartCoroutine(FlyTrapPopUp());

            IntroPlaying = false;

            // If not skipping intro, set all necessary initial values.
            if (!skipIntro)
            {
                buttonFadePositions.Clear();

                var primaryCol = titlePrimaryText.color;
                primaryCol.a = 0.0f;
                titlePrimaryText.color = primaryCol;

                titleSecondaryText.materialForRendering.SetFloat("_FaceDilate", -1.0f);

                var underLineCol = titleUnderline.color;
                underLineCol.a = 0.0f;
                titleUnderline.color = underLineCol;

                var pressToStartCol = pressToStartText.color;
                pressToStartCol.a = 0.0f;
                pressToStartText.color = pressToStartCol;

                foreach (var button in buttons)
                {
                    buttonFadePositions.Add(button.GetComponent<RectTransform>().localPosition);
                    button.GetComponent<RectTransform>().localPosition -= Vector3.up * 300.0f;
                }

                IntroPlaying = true;
                StartCoroutine(Intro());
            }
        }

        private void OnDestroy()
        {
            titleSecondaryText.materialForRendering.SetFloat("_FaceDilate", 0.0f);
        }

        public void SkipIntro()
        {
            if (IntroPlaying)
            {
                StopAllCoroutines();

                // Similar issue to the level select back button, skippin the intro becuase there is 2 different controllers with update functions listening for the submit
                // button it activates at the same time you press A on controller because of timings. Got to wait a frame before marking the intro as finished.
                StartCoroutine(Skip());
            }
        }

        private IEnumerator Skip()
        {
            var primaryCol = titlePrimaryText.color;
            primaryCol.a = 1.0f;
            titlePrimaryText.color = primaryCol;

            titleSecondaryText.materialForRendering.SetFloat("_FaceDilate", 0.0f);

            var underLineCol = titleUnderline.color;
            underLineCol.a = 0.566f;
            titleUnderline.color = underLineCol;

            pressToStartText.gameObject.SetActive(false);

            for (var i = 0; i < buttons.Length; i++)
            {
                buttons[i].GetComponent<RectTransform>().localPosition = buttonFadePositions[i];
            }

            yield return null;

            IntroPlaying = false;
        }

        private IEnumerator Intro()
        {
            StartCoroutine(PrimaryFadeIn());
            StartCoroutine(SecondaryFadeIn());
            yield return new WaitWhile(() => !pressedToStart);
            yield return StartCoroutine(ButtonMoveIn());

            IntroPlaying = false;
        }

        private IEnumerator PrimaryFadeIn()
        {
            yield return new WaitForSeconds(primaryDelay);

            var txtCol = titlePrimaryText.color;
            var imgCol = titleUnderline.color;

            // store inital colors for lerping. (This is how you lerp properly)
            var initialTxtCol = txtCol;
            var initialImgCol = imgCol;

            // T value for lerping
            var t = 0.0f;

            while (true)
            {
                txtCol.a = Mathf.Lerp(initialTxtCol.a, 1.0f, primaryCurve.Evaluate(t));
                imgCol.a = Mathf.Lerp(initialImgCol.a, 0.566f, primaryCurve.Evaluate(t));

                titlePrimaryText.color = txtCol;
                titleUnderline.color = imgCol;

                t += Time.deltaTime / primaryFadeIn;

                if (t >= 1.0f)
                {
                    break;
                }

                yield return null;
            }
        }

        private IEnumerator SecondaryFadeIn()
        {
            yield return new WaitForSeconds(secondaryDelay);

            var t = 0.0f;

            while (true)
            {
                titleSecondaryText.materialForRendering.SetFloat("_FaceDilate", Mathf.Lerp(-1.0f, 0.0f, secondaryCurve.Evaluate(t)));

                t += Time.deltaTime / primaryFadeIn;

                if (t >= 1.0f)
                {
                    break;
                }

                yield return null;
            }

            yield return StartCoroutine(PulsePressToStart());
        }

        private IEnumerator PulsePressToStart()
        {
            waitingForPress = true;
            var t = 0.0f;

            var col = pressToStartText.color;

            while (!pressedToStart)
            {
                // Pulse alpha to 1
                while (!pressedToStart)
                {
                    col.a = Mathf.Lerp(0.0f, 1.0f, pressToStartCurve.Evaluate(t));
                    pressToStartText.color = col;

                    t += Time.deltaTime / pressToStartPulseSpeed;

                    if (t >= 1.0f)
                    {
                        break;
                    }

                    yield return null;
                }

                t = 0.0f;

                // Pulse back to min
                while (!pressedToStart)
                {
                    col.a = Mathf.Lerp(1.0f, 0.0f, pressToStartCurve.Evaluate(t));
                    pressToStartText.color = col;

                    t += Time.deltaTime / pressToStartPulseSpeed;

                    if (t >= 1.0f)
                    {
                        t = 0.0f;
                        break;
                    }

                    yield return null;
                }

                yield return null;
            }

            pressToStartText.gameObject.SetActive(false);
        }

        private IEnumerator ButtonMoveIn()
        {
            //yield return new WaitForSeconds(buttonsDelay);

            for (var i = 0; i < buttons.Length; i++)
            {
                yield return new WaitForSeconds(buttonDelayBetween);

                // if last button, wait for it to finish moving before ending the coroutine so we know when the mark the intro as finished.
                if (i == buttons.Length - 1)
                {
                    yield return StartCoroutine(MoveButton(buttons[i], i));
                    break;
                }

                StartCoroutine(MoveButton(buttons[i], i));
            }
        }

        private IEnumerator MoveButton(GameObject _button, int _buttonIndex)
        {
            var initialPosition = buttons[_buttonIndex].GetComponent<RectTransform>().localPosition;

            var t = 0.0f;

            while (true)
            {
                _button.GetComponent<RectTransform>().localPosition = Vector3.Lerp(initialPosition, buttonFadePositions[_buttonIndex], buttonCurve.Evaluate(t));

                t += Time.deltaTime / buttonMoveIn;

                if (t >= 1.0f)
                {
                    break;
                }

                yield return null;
            }
        }

        private IEnumerator FlyTrapPopUp()
        {
            var pos = flyTrap.transform.position;

            var initialY = pos.y;

            var t = 0.0f;

            while (true)
            {
                yield return new WaitForSeconds(flyTrapPopupDelay);

                while (true)
                {
                    pos.y = Mathf.Lerp(initialY, flyTrapUpY, flyTrapPopupCurve.Evaluate(t));
                    flyTrap.transform.position = pos;

                    t += Time.deltaTime / flyTrapPopupSpeed;

                    if (t >= 1.0f)
                    {
                        t = 0.0f;
                        break;
                    }

                    yield return null;
                }

                yield return new WaitForSeconds(flyTrapPopupStayTime);

                while (true)
                {
                    pos.y = Mathf.Lerp(flyTrapUpY, initialY, t);
                    flyTrap.transform.position = pos;

                    t += Time.deltaTime / flyTrapPopupSpeed;

                    if (t >= 1.0f)
                    {
                        t = 0.0f;
                        break;
                    }

                    yield return null;
                }
            }
        }

        private void Update()
        {
            if (IntroPlaying)
            {
                //if (Input.GetButtonDown("Submit") || Input.GetButtonDown("Jump") || Input.GetButtonDown("Pause"))
                //{
                //    SkipIntro();
                //}

                if (Input.anyKeyDown && waitingForPress)
                {
                    pressedToStart = true;
                    waitingForPress = false;
                }
            }
        }
    }
}
