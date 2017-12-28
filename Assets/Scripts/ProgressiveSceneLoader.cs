using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class ProgressiveSceneLoader : MonoBehaviour
    {

        public List<GameObject> staticObjects = new List<GameObject>();
        public List<GameObject> dynamicObjects = new List<GameObject>();

        public float delayBetweenStatic = 0.2f;
        public float delayBetweenDynamic = 0.1f;

        private void Start()
        {
            StartCoroutine(LoadSceneProgressive());
        }

        private IEnumerator LoadSceneProgressive()
        {
            foreach(var staticObj in staticObjects)
            {
                staticObj.SetActive(true);
                yield return new WaitForSeconds(delayBetweenStatic);
            }

            foreach(var dynObj in dynamicObjects)
            {
                dynObj.SetActive(true);
                yield return new WaitForSeconds(delayBetweenDynamic);
            }

            yield return null;
        }
    }
}
