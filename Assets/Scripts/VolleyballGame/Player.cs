using System;
using System.Collections;
using UnityEngine;

namespace VolleyballGame
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private Vector2 _playZoneMin = new Vector2(-7.5f, -3f);
        [SerializeField] private Vector2 _playZoneMax = new Vector2(0f, 3f);

        private Vector2 _movementDirection = Vector2.zero;
        private Coroutine _movementCoroutine;
        private Transform _transform;
        private Vector2 _defaultPosition;

        private void Awake()
        {
            _transform = transform;
            _defaultPosition = _transform.position;
        }

        public void MoveUp()
        {
            StartMoving(Vector2.up);
        }
        
        public void MoveDown()
        {
            StartMoving(Vector2.down);
        }

        public void MoveLeft()
        {
            StartMoving(Vector2.left);
        }
        
        public void MoveRight()
        {
            StartMoving(Vector2.right);
        }
        
        public void StartMoving(Vector2 direction)
        {
            _movementDirection = direction;
            if (_movementCoroutine == null)
            {
                _movementCoroutine = StartCoroutine(Move());
            }
        }

        public void ResetPosition()
        {
            _transform.position = _defaultPosition;
        }

        public void StopMoving()
        {
            _movementDirection = Vector2.zero;
            if (_movementCoroutine != null)
            {
                StopCoroutine(_movementCoroutine);
                _movementCoroutine = null;
            }
        }

        private IEnumerator Move()
        {
            while (true)
            {
                Vector3 newPosition = _transform.position + (Vector3)(_movementDirection * (_moveSpeed * Time.deltaTime));

                newPosition.x = Mathf.Clamp(newPosition.x, _playZoneMin.x, _playZoneMax.x);
                newPosition.y = Mathf.Clamp(newPosition.y, _playZoneMin.y, _playZoneMax.y);

                _transform.position = newPosition;

                yield return null;
            }
        }
    }
}
