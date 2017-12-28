using System.Collections;
using Assets.Scripts.Objects.BatBoss;
using UnityEngine;

namespace Assets.Scripts
{
    public class BucketRailController : MonoBehaviour
    {
        [SerializeField]
        private GameObject bucketObject = null;

        // The actual bucket part of the bucket object.
        [SerializeField]
        private GameObject bucketPart = null;

        public Vector3[] points;

        public float bucketMoveSpeed = 1.0f;
        public float bucketRotateSpeed = 1.0f;

        public float lightDimSpeed = 0.1f;

        public float gizmoSize = 0.5f;

        public Vector3 bucketFillPosition = new Vector3(0, -0.35f, -13.0f);

        private int currentBucketPoint = -1;

        private Transform bucketObjectTransform;

        private void Start()
        {
            bucketObjectTransform = bucketObject.transform;
        }

        public void Event_MoveBucketLeft(ButtonController _buttonController)
        {
            if (currentBucketPoint > 0)
            {
                currentBucketPoint--;
                StartCoroutine(MoveBucket(_buttonController));
            }
            else
            {
                StartCoroutine(MoveBucketToRefill(_buttonController));
            }       
        }

        public void Event_MoveBucketRight(ButtonController _buttonController)
        {
            if (currentBucketPoint < points.Length - 1)
            {
                currentBucketPoint++;
            }

            StartCoroutine(MoveBucket(_buttonController));
        }

        private IEnumerator MoveBucket(ButtonController _buttonController)
        {
            var point = points[currentBucketPoint];

            while (true)
            {
                if (bucketObjectTransform.position == point)
                {
                    yield return new WaitForSeconds(0.05f);
                    _buttonController.ReleaseButton();
                    yield break;
                }

                bucketObjectTransform.position = Vector3.MoveTowards(bucketObjectTransform.position, point, bucketMoveSpeed * Time.deltaTime);

                yield return null;
            }
        }

        public void Event_TipBucket(ButtonController _buttonController)
        {
            StartCoroutine(TipBucket(_buttonController));      
        }

        private IEnumerator TipBucket(ButtonController _buttonController)
        {
            var timeTipped = 0.0f;

            while(true)
            {
                if (bucketPart.transform.rotation.eulerAngles.z < 135.0f)
                    bucketPart.transform.Rotate(Vector3.forward, bucketRotateSpeed * Time.deltaTime);

                var hits = Physics.RaycastAll(bucketObject.transform.position, Vector3.down, 1000.0f);
                var hasHitBonfire = false;

                foreach (var hit in hits)
                {
                    if (hit.collider.CompareTag("Bonfire"))
                    {
                        hasHitBonfire = true;
                        var bonfire = hit.collider.gameObject.GetComponent<BonfireController>();
                        bonfire.strength -= lightDimSpeed * Time.deltaTime;

                        if(bonfire.strength <= 0.0f)
                        {
                            timeTipped = 3.0f;
                            break;
                        }
                    }
                }

                if(!hasHitBonfire)
                {
                    yield return new WaitForSeconds(0.1f);
                    StartCoroutine(ResetBucketRotation(_buttonController));
                    yield break;
                }

                if (timeTipped >= 3.0f)
                {
                    yield return new WaitForSeconds(0.1f);
                    StartCoroutine(ResetBucketRotation(_buttonController));
                    yield break;
                }

                timeTipped += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator MoveBucketToRefill(ButtonController _buttonController)
        {
            var point = bucketFillPosition;

            while (true)
            {
                if (bucketObjectTransform.position == point)
                {
                    yield return new WaitForSeconds(0.05f);
                    _buttonController.ReleaseButton();
                    yield break;
                }

                bucketObjectTransform.position = Vector3.MoveTowards(bucketObjectTransform.position, point, bucketMoveSpeed * Time.deltaTime);

                yield return null;
            }
        }

        private void ResetBucketPosition()
        {
            bucketObjectTransform.position = bucketFillPosition;
        }

        private IEnumerator ResetBucketRotation(ButtonController _buttonController)
        {
            while(true)
            {
                var rotation = bucketPart.transform.localRotation;
                rotation = Quaternion.RotateTowards(rotation, Quaternion.identity, bucketRotateSpeed * 2 * Time.deltaTime);
                bucketPart.transform.localRotation = rotation;

                if (rotation.z == 0.0f)
                {
                    _buttonController?.ReleaseButton();
                    yield break;
                }

                yield return null;
            }         
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            foreach(var point in points)
            {
                Gizmos.DrawCube(point, Vector3.one * 0.25f);
            }

            Gizmos.color = Color.green;
            Gizmos.DrawCube(bucketFillPosition, Vector3.one * 0.5f);
        }
    }
}
