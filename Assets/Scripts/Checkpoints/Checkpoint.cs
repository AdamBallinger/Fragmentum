using UnityEngine;

namespace Assets.Scripts.Checkpoints
{
    public class Checkpoint : MonoBehaviour
    {
        public CheckpointManager checkpointManager;
        public bool Active;
        public int ID = 0;

        private void Start ()
        {
            if (checkpointManager == null)
            {   
                Debug.LogWarning("Checkpoint manager is null!");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("PlayerMain"))
            {
                //Debug.Log("Checkpoint " + ID + " reached");

                Active = true;
                checkpointManager.SetActiveCheckpoint(ID);
            }
        }
    }
}
