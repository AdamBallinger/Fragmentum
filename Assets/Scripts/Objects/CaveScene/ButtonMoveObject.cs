using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Objects.CaveScene
{
    public class ButtonMoveObject : MonoBehaviour
    {

        public Vector3 originalPosition;
        public Vector3 moveToPosition;

        public float moveSpeed = 1.0f;
        public float returnDelay = 0.0f;

        private Transform _transform;

        private void Start()
        {
            _transform = transform;

            originalPosition = _transform.position;
        }

        public void ButtonPressed_MoveObject(ButtonController _buttonController)
        {
            StopAllCoroutines();
            StartCoroutine(MoveObject(moveToPosition, _buttonController));
        }

        public void ButtonPressed_ReturnObject()
        {
            StopAllCoroutines();
            StartCoroutine(MoveObject(originalPosition, null));
        }

        private IEnumerator MoveObject(Vector3 _targetPos, ButtonController _buttonController)
        {
            if(_targetPos == originalPosition)
            {
                yield return new WaitForSeconds(returnDelay);
            }

            while(true)
            {
                var pos = _transform.position;
                pos = Vector3.MoveTowards(pos, _targetPos, moveSpeed * Time.deltaTime);
                _transform.position = pos;

                if(pos == _targetPos)
                {
                    if(_buttonController != null)
                    {
                        _buttonController.ReleaseButton();
                    }

                    break;
                }

                yield return null;
            }
        }
    }
}
