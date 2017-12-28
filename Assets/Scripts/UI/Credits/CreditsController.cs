using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Credits
{
    public class CreditsController : MonoBehaviour 
    {

        [Header("GameObject References")]
        public GameObject titleGO;
        public GameObject creditsGO;

        public Image logo;

        [Space(2)]
        [Header("Timings")]
        [Tooltip("Delay in seconds before starting the actual credits animations.")]
        public float startDelay = 2.0f;
        public float titleMoveUpTime = 1.0f;
        public float creditsDelay = 1.0f;
        public float creditsMoveUpTime = 1.0f;
        public float delayBetweenCredit = .35f;

        [Space(2)]
        [Header("Positionings")]
        public float titleTargetY = 0.0f;
        public Vector3 creditsStartPos = Vector3.zero;
        public Vector3 creditsTargetPos = Vector3.zero;

        [Space(2)]
        [Header("Curves")]
        public bool useCurves = true;
        public AnimationCurve titleCurve;
        public AnimationCurve creditsCurve;

        private void Start () 
        {
            titleGO.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            creditsGO.GetComponent<RectTransform>().anchoredPosition = creditsStartPos;

            foreach(var txt in creditsGO.GetComponentsInChildren<TextMeshProUGUI>())
            {
                var col = txt.color;
                col.a = 0.0f;
                txt.color = col;
            }

            var logoCol = logo.color;
            logoCol.a = 0.0f;
            logo.color = logoCol;

            StartCoroutine(StartCredits());
        }

        private IEnumerator StartCredits()
        {
            yield return new WaitForSeconds(startDelay);
            StartCoroutine(Title());

            yield return new WaitForSeconds(creditsDelay);
            StartCoroutine(Credits());

            foreach (var txt in creditsGO.GetComponentsInChildren<TextMeshProUGUI>())
            {
                StartCoroutine(FadeCredits(txt));
                yield return new WaitForSeconds(delayBetweenCredit);
            }

            var t = 0.0f;
            var color = logo.color;

            while(true)
            {
                color.a = Mathf.Lerp(0.0f, 1.0f, creditsCurve.Evaluate(t));
                logo.color = color;

                t += Time.deltaTime / creditsMoveUpTime;

                if (t >= 1.0f) break;

                yield return null;
            }

            Invoke("ToMenu", 8.0f);
        }

        private IEnumerator Title()
        {
            var titleRect = titleGO.GetComponent<RectTransform>().anchoredPosition;

            var titleStartY = titleRect.y;

            var t = 0.0f;

            // Slide title up
            while (true)
            {
                titleRect.y = Mathf.Lerp(titleStartY, titleTargetY, useCurves ? titleCurve.Evaluate(t) : t);
                titleGO.GetComponent<RectTransform>().anchoredPosition = titleRect;

                t += Time.deltaTime / titleMoveUpTime;

                if (t >= 1.0f)
                {
                    break;
                }

                yield return null;
            }
        }

        private IEnumerator Credits()
        {
            var creditsRect = creditsGO.GetComponent<RectTransform>();

            var t = 0.0f;

            while(true)
            {
                creditsRect.anchoredPosition = Vector3.Lerp(creditsStartPos, creditsTargetPos, creditsCurve.Evaluate(t));

                t += Time.deltaTime / creditsMoveUpTime;

                if(t >= 1.0f)
                {
                    break;
                }

                yield return null;
            }
        }

        private IEnumerator FadeCredits(TextMeshProUGUI _text)
        {
            var t = 0.0f;

            while(true)
            {
                var col = _text.color;
                col.a = Mathf.Lerp(0.0f, 1.0f, creditsCurve.Evaluate(t));
                _text.color = col;

                t += Time.deltaTime / creditsMoveUpTime;

                if (t >= 1.0f) break;

                yield return null;               
            }
        }

        private void ToMenu()
        {
            FindObjectOfType<GlobalUIController>().LoadLevel("MainMenu");
        }
    }
}
