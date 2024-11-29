using System;
using System.Collections;
using UnityEngine;

namespace VolleyballGame
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 0.4f;
        [SerializeField] private Vector2 _playZoneMin = new Vector2(0f, -3f);
        [SerializeField] private Vector2 _playZoneMax = new Vector2(7.5f, 3f);
        [SerializeField] private Transform _ball;

        private Coroutine _movementRoutine;
        private Transform _transform;
        private Vector2 _defaultPosition;

        private void Awake()
        {
            _transform = transform;
            _defaultPosition = _transform.position;
        }

        private void Start()
        {
            StopFollowing();
            ResetPosition();
        }

        private void OnDisable()
        {
            if (_movementRoutine != null)
            {
                StopCoroutine(_movementRoutine);
            }
        }

        public void ResetPosition()
        {
            _transform.position = _defaultPosition;
        }

        public void StartFollowing()
        {
            _movementRoutine = StartCoroutine(FollowBall());
        }

        public void StopFollowing()
        {
            if (_movementRoutine != null)
            {
                StopCoroutine(_movementRoutine);
            }

            _movementRoutine = null;
        }

        public void IncreaseSpeed()
        {
            _moveSpeed += 0.1f;
        }

        private IEnumerator FollowBall()
        {
            while (true)
            {
                if (_ball != null)
                {
                    Vector3 targetPosition = new Vector3(
                        Mathf.Clamp(_ball.position.x, _playZoneMin.x, _playZoneMax.x),
                        Mathf.Clamp(_ball.position.y, _playZoneMin.y, _playZoneMax.y),
                        _transform.position.z
                    );

                    _transform.position =
                        Vector3.MoveTowards(_transform.position, targetPosition, _moveSpeed * Time.deltaTime);
                }

                yield return null;
            }
        }
    }
}