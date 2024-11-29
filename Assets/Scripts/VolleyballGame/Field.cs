using System;
using UnityEngine;

namespace VolleyballGame
{
    public class Field : MonoBehaviour
    {
        public event Action BallHit;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out VolleyballBall ball))
            {
                BallHit?.Invoke();
            }
        }
    }
}
