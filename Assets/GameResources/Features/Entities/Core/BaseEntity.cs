namespace GameResources.Features.Entities.Core
{
    using UnityEngine;

    public abstract class BaseEntity : MonoBehaviour
    {
        [field: SerializeField] public Rigidbody2D Rigidbody2D { get; private set; } = default;
        [field: SerializeField] public DistanceJoint2D DistanceJoint2D { get; private set; } = default;
        [field: SerializeField] public Collider2D Collider2D { get; private set; } = default;
        
        [SerializeField]
        protected SpriteRenderer spriteRenderer;

        public abstract void Reset();
    }
}