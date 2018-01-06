using System.Linq;
using Assets.Scripts.CameraUtils;
using Assets.Scripts.Character;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Checkpoints
{
    public class CheckpointManager : MonoBehaviour
    {
        public Player player;
        public CameraFadeUtility fader;

        public Checkpoint[] checkpoints;

        public bool reloadSceneOnDeath;
        public float respawnTime = 2f;
        private float respawnTimer;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("PlayerMain").GetComponent<Player>();

            var temp = GameObject.FindGameObjectsWithTag("Checkpoint");

            var count = temp.Count();
            if (count == 0)
            {
                return;
            }

            checkpoints = new Checkpoint[count];
            for (var i = 0; i < count; i++)
            {
                checkpoints[i] = temp[i].gameObject.GetComponent<Checkpoint>();
            }
        }

        private void Update()
        {
            if (player.Dead)
            {
                if (respawnTimer == 0.0f)
                {
                    fader.FadeIntoBlack(respawnTime);
                }

                respawnTimer += Time.deltaTime;
                if (respawnTimer > respawnTime)
                {
                    if (reloadSceneOnDeath)
                    {
                        var globalUIController = FindObjectOfType<GlobalUIController>();
                        if (globalUIController != null)
                        {
                            globalUIController.LoadLevel(SceneManager.GetActiveScene().name);
                        }
                        else
                        {
                            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                        }
                    }

                    respawnTimer = 0.0f;
                    var respawnPoint = checkpoints.FirstOrDefault(c => c.Active);
                    player.transform.position = respawnPoint.transform.position;
                    player.Respawn();
                }
            }
        }

        public void SetActiveCheckpoint(int id)
        {
            if (checkpoints.Length == 0)
            {
                return;
            }

            var setInactive = checkpoints.Where(c => c.ID != id);

            foreach (var inactiveCheckpoint in setInactive)
            {
                inactiveCheckpoint.Active = false;
            }

        }
    }
}
