using System;
using System.Collections;
using UnityEngine;

namespace ArcheryGame
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Arrow : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed;

        private Transform _transform;
        private Coroutine _movingCoroutine;
        private BoxCollider2D _boxCollider2D;

        private Vector2 _target;
        private Vector3 _defaultPosition;

        public event Action TargetHit;
        public event Action Missed;

        private void Awake()
        {
            _transform = transform;
            _boxCollider2D = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            _defaultPosition = _transform.position;
        }

        public void StartMovement(Vector2 target)
        {
            _target = target;
            _boxCollider2D.enabled = false;

            if (_movingCoroutine == null)
            {
                _movingCoroutine = StartCoroutine(Movement());
            }
        }

        public void StartMovement()
        {
            if (_movingCoroutine == null)
            {
                _movingCoroutine = StartCoroutine(Movement());
            }
        }

        public void StopMovement()
        {
            if (_movingCoroutine != null)
            {
                StopCoroutine(_movingCoroutine);
                _movingCoroutine = null;
            }
        }

        public void ReturnToDefaultPosition()
        {
            _transform.position = _defaultPosition;
        }

        private IEnumerator Movement()
        {
            while (Vector3.Distance(_transform.position, _target) > 0.1f)
            {
                _transform.position =
                    Vector3.MoveTowards(_transform.position, _target, _moveSpeed * Time.deltaTime);
                yield return null;
            }

            _boxCollider2D.enabled = true;
            _boxCollider2D.isTrigger = true;
            
            ValidateHit();
        }

        private void ValidateHit()
        {
            Collider2D hitCollider = Physics2D.OverlapPoint(_transform.position);

            if (hitCollider != null && hitCollider.TryGetComponent(out HitTarget target))
            {
                TargetHit?.Invoke();
            }
            else
            {
                Missed?.Invoke();
            }

            _boxCollider2D.enabled = false;
        }
    }
}