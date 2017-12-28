using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class FadeIn : MonoBehaviour
    {
        public Texture2D fadeOutTexture;
        public float fadeSpeed = 2f;

        private int drawDepth = -1000;
        private float alpha = 1.0f;
        private int fadeDir = -1; //direction of fade, -1 = fade in, 1 = fade out.

        public void Start()
        {
            SceneManager.sceneLoaded += SceneLoaded;
        }

        private void OnGUI()
        {
            alpha += fadeDir*fadeSpeed*Time.deltaTime;
            alpha = Mathf.Clamp01((alpha));

            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
            GUI.depth = drawDepth;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture );
        }

        public float BeginFade(int direction)
        {
            fadeDir = direction;
            return fadeSpeed;
        }

        private void SceneLoaded(Scene _sceneLoaded, LoadSceneMode _loadMode)
        {
            alpha = 1f;
            // fade in
            BeginFade(-1);
        }
    }
}
