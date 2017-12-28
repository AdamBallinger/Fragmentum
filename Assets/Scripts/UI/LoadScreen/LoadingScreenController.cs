using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI.LoadScreen
{
    public class LoadingScreenController : MonoBehaviour
    {

        public Image fillImage;
        public Text loadText;

        private AsyncOperation asyncOp;

        private void OnEnable()
        {
            fillImage.fillAmount = 0.0f;
        }

        public void StartAsyncLoad(string _sceneName)
        {
            StopAllCoroutines();
            asyncOp = null;
            StartCoroutine(LoadAsync(_sceneName));
        }

        private IEnumerator LoadAsync(string _sceneName)
        {
            loadText.text = "LOADING OBJECTS...";
            asyncOp = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Single);
            asyncOp.allowSceneActivation = false;

            while(asyncOp.progress < 0.9f)
            {
                fillImage.fillAmount = asyncOp.progress;
                yield return null;
            }

            fillImage.fillAmount = 1.0f;
            loadText.text = "ACTIVATING SCENE...";

            // yield for a short delay so the next frame has time to render the updated text (Activating the scene freezes the thread whilst it works)
            yield return new WaitForSeconds(0.1f);

            asyncOp.allowSceneActivation = true;

            yield return new WaitWhile(() => !asyncOp.isDone);
        }
    }
}
