using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.CameraUtils
{
    public enum FadeMode
    {
        None,
        FadeOutOfBlack,
        FadeIntoBlack
    }

    public class CameraFadeUtility : MonoBehaviour
    {

        public AnimationCurve fadeInCurve;
        public AnimationCurve fadeOutCurve;

        private FadeMode fadeMode = FadeMode.None;

        // Handle checking if we start or stop a new fade for resetting inerpolation T value.
        private bool isFadeDirty;

        private float interpolateT;
        private float fadeTime = 1.0f;
        private float fadeAlpha;

        private GlobalUIController uiController;

        private void Start()
        {
            uiController = FindObjectOfType<GlobalUIController>();

            if(uiController == null)
            {
                Debug.LogWarning("No GlobalUIController found. Trying to fade the camera with this object will not work!");
            }
        }

        private void StartFade(FadeMode _fadeMode, float _time)
        {
            isFadeDirty = true;
            fadeTime = _time;
            fadeMode = _fadeMode;
            fadeAlpha = _fadeMode == FadeMode.FadeIntoBlack || _fadeMode == FadeMode.None ? 0.0f : 1.0f;
        }

        public void FadeIntoBlack(float _time)
        {
            StartFade(FadeMode.FadeIntoBlack, _time);
        }

        public void FadeOutOfBlack(float _time)
        {
            StartFade(FadeMode.FadeOutOfBlack, _time);
        }

        public void SetFullBlack()
        {
            fadeMode = FadeMode.None;
            fadeAlpha = 1.0f;

            SetFadeImageAlpha(fadeAlpha);
        }

        public void ClearFadeIfPresent()
        {
            fadeMode = FadeMode.None;
            fadeAlpha = 0.0f;

            SetFadeImageAlpha(fadeAlpha);
        }

        public bool IsFading()
        {
            return fadeMode != FadeMode.None;
        }

        private void Update()
        {
            if (uiController == null) return;

            if (isFadeDirty || fadeMode == FadeMode.None)
            {
                isFadeDirty = false;
                interpolateT = 0.0f;
                return;
            }

            var startAlpha = fadeMode == FadeMode.FadeIntoBlack ? 0.0f : 1.0f;
            var endAlpha = fadeMode == FadeMode.FadeIntoBlack ? 1.0f : 0.0f;
            fadeAlpha = Mathf.Lerp(startAlpha, endAlpha, fadeMode == FadeMode.FadeIntoBlack ? fadeInCurve.Evaluate(interpolateT) : fadeOutCurve.Evaluate(interpolateT));

            SetFadeImageAlpha(fadeAlpha);

            interpolateT += Time.deltaTime / fadeTime;

            if (interpolateT >= 1.0f)
            {
                fadeMode = FadeMode.None;
                interpolateT = 0.0f;
            }
        }

        private void SetFadeImageAlpha(float _alpha)
        {
            var fadeImgCol = uiController.cameraFader.GetComponentInChildren<Image>().color;
            fadeImgCol.a = _alpha;
            uiController.cameraFader.GetComponentInChildren<Image>().color = fadeImgCol;
        }
    }
}
