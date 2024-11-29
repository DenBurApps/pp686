using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlightGame
{
    [RequireComponent(typeof(Collider2D))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private float _movingSpeed;
        [SerializeField] private float _maxXposition;
        [SerializeField] private float _minXposition;

        private Vector2 _defaultPosition;
        private Vector2 _previousTouchPosition;
        private Transform _transform;
        private int _currentDirection = 0;

        public event Action<MovingObject> VirusCatched;
        public event Action<MovingObject> BallCatched; 
        
        private void Awake()
        {
            _transform = transform;
            _defaultPosition = _transform.position;
        }

        private void Start()
        {
            _transform.position = _defaultPosition;
        }

        public void ReturnToDefaultPosition()
        {
            _transform.position = _defaultPosition;
        }

        private void Update()
        {
            if (_currentDirection != 0)
            {
                Move(_currentDirection);
            }
        }

        public void Move(int direction)
        {
            Vector2 newPosition = _transform.position;
            newPosition.x += direction * _movingSpeed * Time.deltaTime;
            
            newPosition.x = Mathf.Clamp(newPosition.x, _minXposition, _maxXposition);
            
            _transform.position = newPosition;
        }

        public void SetDirection(int direction)
        {
            _currentDirection = direction;
        }
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out IIntractable interactable))
            {
                if (interactable is Virus)
                {
                    VirusCatched?.Invoke((MovingObject)interactable);
                }
                else if(interactable is Ball)
                {
                    BallCatched?.Invoke((MovingObject)interactable);
                }
            }
        }
    }
}