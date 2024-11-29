using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ArcheryGame
{
    [RequireComponent(typeof(Collider2D))]
    public class Target : MonoBehaviour
    {
        [SerializeField] private Sprite _red;
        [SerializeField] private Sprite _green;
        [SerializeField] private float _radius = 5f;
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        private Vector3 _centerPosition;
        private Vector3 _targetPosition;
        private Transform _transform;
        
        private Coroutine _movementCoroutine;

        private void Awake()
        {
            _transform = transform;
        }

        private void Start()
        {
            _centerPosition = _transform.position;
        }
        
        public void StartRandomMovement()
        {
            if (_movementCoroutine == null)
            {
                _movementCoroutine = StartCoroutine(RandomMovement());
            }
        }

        public void StopRandomMovement()
        {
            if (_movementCoroutine != null)
            {
                StopCoroutine(_movementCoroutine);
                _movementCoroutine = null;
            }
        }

        public Vector2 GetCurrentPosition()
        {
            return _transform.position;
        }

        public void IncreaseSpeed()
        {
            _moveSpeed += 0.5f;
        }

        public void ReturnToDefaultPosition()
        {
            _transform.position = _centerPosition;
        }

        private IEnumerator RandomMovement()
        {
            while (true)
            {
                SetRandomTargetPosition();
                
                while (Vector3.Distance(_transform.position, _targetPosition) > 0.1f)
                {
                    _transform.position = Vector3.MoveTowards(_transform.position, _targetPosition, _moveSpeed * Time.deltaTime);
                    yield return null;
                }

                yield return null;
            }
        }

        private void SetRandomTargetPosition()
        {
            Vector2 randomPoint = Random.insideUnitCircle * _radius;
            
            _targetPosition = _centerPosition + new Vector3(randomPoint.x, randomPoint.y, 0f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _spriteRenderer.sprite = _green;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _spriteRenderer.sprite = _red;
        }
    }
}