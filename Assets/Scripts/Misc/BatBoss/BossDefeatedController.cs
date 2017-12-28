using System.Collections;
using Assets.Scripts.CameraUtils;
using Assets.Scripts.Character;
using UnityEngine;

namespace Assets.Scripts.Misc.BatBoss
{
    public class BossDefeatedController : MonoBehaviour
    {

        public CameraCutscene defeatedCutscene;
        public Light relicSpotLight;
        public float relicLightTime = 1.0f;
        public AnimationCurve relicLightCurve;

        public void StartCutscene()
        {
            defeatedCutscene.StartCutscene(true);
            FindObjectOfType<Player>().controlsEnabled = false;           
        }

        public void StartLightFade()
        {
            StartCoroutine(FadeLightIn());
        }

        public void OnCutsceneFinish()
        {
            FindObjectOfType<Player>().controlsEnabled = true;
        }

        private IEnumerator FadeLightIn()
        {
            var t = 0.0f;

            while(t <= 1.0f)
            {
                relicSpotLight.intensity = Mathf.Lerp(0.0f, 2.0f, relicLightCurve.Evaluate(t));

                t += Time.deltaTime / relicLightTime;

                yield return null;
            }
        }
    }
}
