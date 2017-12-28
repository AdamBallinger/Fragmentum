using UnityEngine;

namespace Assets.Scripts.Character
{
    public class PlayerStepParticles : MonoBehaviour
    {

        public GameObject particleSysPrefab;

        public Vector3 spawnPosition = Vector3.zero;
        public Vector3 spawnRotation = new Vector3(-90.0f, 0.0f, 0.0f);

        [Tooltip("Color over lifetime of the particles. Light intensity is affected by alpha.")]
        public Gradient particleColor;

        public float distanceBetweenSpawns = 1.0f;

        public float sizeMultiplier = 1.0f;

        private Vector3 lastSpawnDistance = Vector3.zero;

        private Transform _transform;

        private void Start()
        {
            _transform = transform;

            lastSpawnDistance = _transform.position;
        }

        private void Update()
        {
            var distSinceLastSpawn = Vector3.Distance(lastSpawnDistance, _transform.position);

            if(distSinceLastSpawn >= distanceBetweenSpawns && GetComponent<Player>().Grounded())
            {
                var ps = Instantiate(particleSysPrefab, _transform.position + spawnPosition, Quaternion.Euler(spawnRotation));

                var mainModule = ps.GetComponent<ParticleSystem>().main;
                mainModule.startSize = mainModule.startSize.constant * sizeMultiplier;

                lastSpawnDistance = transform.position;
            }
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.position + spawnPosition, Vector3.one * 0.15f);
        }
    }
}
