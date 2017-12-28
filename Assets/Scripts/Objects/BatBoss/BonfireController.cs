using UnityEngine;

namespace Assets.Scripts.Objects.BatBoss
{
    public class BonfireController : MonoBehaviour
    {

        // Does this fire pit already have a minion working on re lighting it?
        [HideInInspector]
        public bool hasMinionAttention;

        // Particle system root object for the bonfire.
        public GameObject psObjectRoot;

        // The strength of the bonfire fires (0 - 1). Controls the size of the fire particle system.
        [Range(0, 1)]
        public float strength;

        public AudioClip fire;
        private AudioSource _audio;
        private bool canPlay = true;

        private Transform psObjectRootTransform;

        private void Start()
        {
            _audio = GetComponent<AudioSource>();
            psObjectRootTransform = psObjectRoot.transform;
        }

        private void Update()
        {
            PlaySound();

            strength = Mathf.Clamp01(strength);
            psObjectRootTransform.localScale = Vector3.one * strength;
        }

        private void PlaySound()
        {
            if (canPlay && strength != 0)
            {
                canPlay = false;
                _audio.clip = fire;
                _audio.Play();
            }

            if (strength == 0)
            {
                canPlay = true;
                _audio.Stop();
            }
        }
    }
}
