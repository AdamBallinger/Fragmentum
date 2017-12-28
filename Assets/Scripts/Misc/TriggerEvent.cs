using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Misc
{
    public class TriggerEvent : MonoBehaviour
    {

        public bool limitToTags = true;
        public List<string> triggerableTags;

        public UnityEvent onTriggerEnter;
        public UnityEvent onTriggerStay;
        public UnityEvent onTriggerExit;

        private void OnTriggerEnter(Collider _other)
        {
            if (limitToTags)
            {
                foreach(var _tag in triggerableTags)
                {
                    if(_other.CompareTag(_tag))
                    {
                        if (onTriggerEnter != null)
                        {
                            onTriggerEnter.Invoke();
                        }

                        return;
                    }
                }
            }

            if (onTriggerEnter != null)
            {
                onTriggerEnter.Invoke();
            }
        }

        private void OnTriggerStay(Collider _other)
        {
            if (limitToTags)
            {
                foreach (var _tag in triggerableTags)
                {
                    if (_other.CompareTag(_tag))
                    {
                        if (onTriggerStay != null)
                        {
                            onTriggerStay.Invoke();
                        }

                        return;
                    }
                }
            }

            if (onTriggerStay != null)
            {
                onTriggerStay.Invoke();
            }
        }

        private void OnTriggerExit(Collider _other)
        {
            if (limitToTags)
            {
                foreach (var _tag in triggerableTags)
                {
                    if (_other.CompareTag(_tag))
                    {
                        if (onTriggerExit != null)
                        {
                            onTriggerExit.Invoke();
                        }

                        return;
                    }
                }
            }

            if (onTriggerExit != null)
            {
                onTriggerExit.Invoke();
            }
        }
    }
}
