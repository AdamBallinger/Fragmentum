using UnityEngine;

namespace Assets.Scripts.Misc
{
    [RequireComponent(typeof(BoxCollider))]
    public class ParentTrigger : MonoBehaviour
    {

        private GameObject playerRoot;

        private void Start()
        {
            playerRoot = GameObject.FindGameObjectWithTag("PlayerMain");
        }

        private void OnTriggerEnter(Collider _collider)
        {
            if(_collider.CompareTag("PlayerMain"))
            {
                //Debug.Log("Player parented to object: " + gameObject.name);
                playerRoot.transform.SetParent(gameObject.transform);
            }
        }

        private void OnTriggerExit(Collider _collider)
        {
            if (_collider.CompareTag("PlayerMain"))
            {
                //Debug.Log("Unparenting player from object: " + gameObject.name);
                playerRoot.transform.SetParent(null);
            }
        }
    }
}
