using UnityEngine;

namespace ArcheryGame
{
    public class HitTarget : MonoBehaviour
    {
        [SerializeField] private CircleCollider2D _circleCollider;

        public CircleCollider2D CircleCollider2D => _circleCollider;
    }
}
