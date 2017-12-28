using System.Collections;
using Assets.Scripts.CameraUtils;
using Assets.Scripts.Character;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Misc
{
    public class CastleCutSceneController : MonoBehaviour
    {

        [Header("Object References")]
        public CameraCutscene cutScene;
        public GameObject playerObject;
        public GameObject knightObject;

        [Space(2)]
        [Header("Settings")]
        public float playerMoveTime = 10.0f;
        public AnimationCurve playerCurve;

        public string sceneToLoad;

        [Space(2)]
        [Header("Position Settings")]
        public Vector3[] playerPositions;

        private Player playerControls;

        private void Start()
        {
            playerControls = playerObject.GetComponent<Player>();

            if (playerControls == null)
            {
                Debug.LogWarning("Castle cut scene couldn't start as the player script couldn't be found on the given player object reference.");
                return;
            }

            playerControls.controlsEnabled = false;

            cutScene.StartCutscene(true);
        }

        public void OnCutsceneStart()
        {
            FindObjectOfType<GlobalUIController>().playerHUD.SetActive(false);
        }

        public void OnCutsceneFinish()
        {
            FindObjectOfType<GlobalUIController>().LoadLevel(sceneToLoad);
        }

        private void SetPlayerRunning(bool _running)
        {
            playerControls.Animator.SetBool("isRunning", _running);
        }

        public void SetAcceptingQuest(bool _accepting)
        {
            switch (_accepting)
            {
                case true:
                    playerControls.Animator.SetBool("acceptingQuest", true);
                    break;

                case false:
                    playerControls.Animator.SetBool("acceptingQuest", false);
                    break;
            }
        }

        public void MovePlayerToPosition(int _posIndex)
        {
            StopAllCoroutines();
            StartCoroutine(MovePlayer(playerPositions[_posIndex]));
        }

        public void SetPlayerMoveTime(float _time)
        {
            playerMoveTime = _time;
        }

        public void SetKnightStanding()
        {
            knightObject.GetComponent<Animator>().SetBool("Standing", true);
        }

        private IEnumerator MovePlayer(Vector3 _pos)
        {
            playerControls.controlsEnabled = false;
            SetPlayerRunning(true);
            var initialPos = playerObject.transform.position;

            var t = 0.0f;

            while (true)
            {
                playerObject.transform.position = Vector3.Lerp(initialPos, _pos, playerCurve.Evaluate(t));

                t += Time.deltaTime / playerMoveTime;

                if (t >= 1.0f)
                {
                    break;
                }

                yield return null;
            }

            SetPlayerRunning(false);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;

            foreach (var pos in playerPositions)
            {
                Gizmos.DrawCube(pos, Vector3.one);
            }
        }
    }
}
