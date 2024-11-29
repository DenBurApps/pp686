using UnityEngine;
using Random = UnityEngine.Random;

namespace VolleyballGame
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class VolleyballBall : MonoBehaviour
    {
        [SerializeField] private float _initialSpeed = 1f;
        [SerializeField] private float _bounceAngleOffset = 0.1f;

        private Rigidbody2D _rb;
        private Vector2 _defaultPosition;
        private Transform _transform;
        private Vector2 _savedVelocity;

        private enum BallState { Idle, Moving, Paused }
        private BallState _currentState;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _defaultPosition = transform.position;
            _transform = transform;
            _currentState = BallState.Idle;
        }

        private void Start()
        {
            StopBall();
        }

        public void StopBall()
        {
            if (_currentState == BallState.Moving)
            {
                _savedVelocity = _rb.velocity;
                _rb.velocity = Vector2.zero;
                _rb.isKinematic = true;
                _currentState = BallState.Paused;
            }
            else
            {
                _rb.velocity = Vector2.zero;
                _rb.isKinematic = true;
                _currentState = BallState.Idle;
            }
        }

        public void StartBall()
        {
            if (_currentState == BallState.Paused)
            {
                _rb.isKinematic = false;
                _rb.velocity = _savedVelocity;
                _currentState = BallState.Moving;
            }
        }

        public void LaunchBall()
        {
            _rb.isKinematic = false;
            
            Vector2 randomDirection = GenerateRandomDirection();
            _rb.velocity = randomDirection * _initialSpeed;

            _currentState = BallState.Moving;
        }

        public void ResetBall()
        {
            _rb.velocity = Vector2.zero;
            _transform.position = _defaultPosition;
            _currentState = BallState.Idle;
        }

        private Vector2 GenerateRandomDirection()
        {
            float randomX = Random.Range(0.3f, 1f) * (Random.value > 0.5f ? 1 : -1);
            float randomY = Random.Range(-0.3f, 0.3f);
            return new Vector2(randomX, randomY).normalized;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_currentState != BallState.Moving) return;
            
            Vector2 velocity = _rb.velocity;
            
            velocity.y += Random.Range(-_bounceAngleOffset, _bounceAngleOffset);
            
            _rb.velocity = velocity.normalized * _initialSpeed;
        }
    }
}
