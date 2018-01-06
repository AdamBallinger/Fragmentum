using UnityEngine;
using Assets.Scripts.Character;

namespace Assets.Scripts.Objects
{
    public class DestroyOnDash : MonoBehaviour
    {
        public GameObject destroyParticleSystemPrefab;

        public AudioClip destroySound;

        private Player player;

        private void Start()
        {
            player = FindObjectOfType<Player>();
            GetComponent<AudioSource>().clip = destroySound;
        }

        private void OnTriggerEnter()
        {
            if (player.Dashing)
            {
                if (destroyParticleSystemPrefab != null)
                {
                    var ps = Instantiate(destroyParticleSystemPrefab, transform.position, Quaternion.identity);
                    var psas = ps.GetComponent<AudioSource>();
                    if(psas != null)
                    {
                        psas.clip = destroySound;
                        psas.Play();
                    }
                }

                gameObject.SetActive(false);
            }
        }
    }
}