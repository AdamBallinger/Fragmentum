using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Objects.BatBoss
{
    public class GateController : MonoBehaviour
    {

        public float moveSpeed = 1.0f;

        public float closeDistance = 1.0f;

        private bool isClosed;

        private Vector3 openPos = Vector3.zero;
        private bool runtime;

        private Transform _transform;

        private void Start()
        {
            _transform = transform;

            runtime = true;
            openPos = _transform.position;
        }

        public void CloseGate()
        {
            if (isClosed)
            {
                return;
            }

            isClosed = true;
            StartCoroutine(Close());
        }

        private IEnumerator Close()
        {
            var pos = _transform.position;
            var closePos = pos - Vector3.down * closeDistance;

            while (true)
            {
                pos = Vector3.MoveTowards(pos, closePos, moveSpeed * Time.deltaTime);
                _transform.position = pos;

                if(pos == closePos)
                {
                    yield break;
                }

                yield return null;
            }
        }

        private void OnDrawGizmos()
        {
            var bottom = transform.position + Vector3.down * GetComponent<BoxCollider>().bounds.size.y / 2;
            var closePos = (runtime ? openPos : transform.position) - Vector3.down * closeDistance;
            closePos.y -= GetComponent<BoxCollider>().bounds.size.y / 2;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(bottom, closePos);
        }
    }
}
