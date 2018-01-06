using Assets.Scripts.Character;
using UnityEngine;

namespace Assets.Scripts.Objects
{
    public class ScoreGem : MonoBehaviour
    {
        public int score = 100;

        public float bobSpeed = 0.1f;
        public float rotationSpeed = 0.6f;

        private float bobAngle;

        private Vector3 motion = Vector3.zero;
        private Vector3 euler = new Vector3(0, 1, 0);

        public AudioClip collectSound;

        private bool collected;

        private Transform _transform;
        private AudioSource _audioSource;

        private void Start()
        {
            _transform = transform;
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            bobAngle += 0.1f;

            if (bobAngle >= Mathf.PI * 2)
            {
                bobAngle = 0.0f;
            }

            motion.y = Mathf.Sin(bobAngle);
            _transform.position += motion * Time.deltaTime;
            _transform.Rotate(euler, rotationSpeed);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("PlayerMain") && !collected)
            {
                var player = other.gameObject.GetComponent<Player>();
                player.Score += score;

                //Debug.Log(player.score);
                collected = true;

                if (gameObject.CompareTag("Mushroom Pickup"))
                {
                    foreach (var rend in GetComponentsInChildren<Renderer>())
                    {
                        rend.enabled = false;
                    }

                    _audioSource.clip = collectSound;
                    _audioSource.Play();
                }
                else
                {
                    foreach (var rend in GetComponentsInChildren<Renderer>())
                    {
                        rend.enabled = false;
                    }

                    _audioSource.clip = collectSound;
                    _audioSource.Play();
                }

            }
        }
    }
}
