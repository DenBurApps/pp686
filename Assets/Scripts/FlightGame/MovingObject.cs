using System;
using System.Collections;
using UnityEngine;

namespace FlightGame
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class MovingObject : MonoBehaviour,IIntractable
    {
        [SerializeField] private float _speed = 0;
        
        private Transform _transform;
        private IEnumerator _movingCoroutine;

        private void Awake()
        {
            _transform = transform;
        }

        public void EnableMovement()
        {
            if (_movingCoroutine == null)
                _movingCoroutine = StartMoving();

            StartCoroutine(_movingCoroutine);
        }
    
        public void DisableMovement()
        {
            if (_movingCoroutine != null)
            {
                StopCoroutine(_movingCoroutine);
                _movingCoroutine = null;
            }
        }

        public void SetSpeed(float speed)
        {
            _speed = speed;
        }

        private IEnumerator StartMoving()
        {
            while (true)
            {
                _transform.position += Vector3.down * _speed * Time.deltaTime;
            
                yield return null;
            }
        }
    }
}
