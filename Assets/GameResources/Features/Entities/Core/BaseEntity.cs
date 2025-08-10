namespace GameResources.Features.Entities.Core
{
    using System;
    using DG.Tweening;
    using UnityEngine;
    using Zenject;
    using ZoneController;

    public abstract class BaseEntity : MonoBehaviour, IPoolable<IMemoryPool>, IDisposable
    {
        public ZoneEntity IsBelongsZone { get; set; } = default;
        public EntityType Type { get; protected set; } = default;

        [field: SerializeField] public Rigidbody2D Rigidbody2D { get; private set; } = default;
        [field: SerializeField] public DistanceJoint2D DistanceJoint2D { get; private set; } = default;
        [field: SerializeField] public Collider2D Collider2D { get; private set; } = default;
        
        [SerializeField] protected SpriteRenderer spriteRenderer;

        protected IMemoryPool pool;
        protected Transform poolParent = default;
        
        private void Awake() => poolParent = transform.parent;

        public virtual T GetCollider<T>() where T : Collider2D => Collider2D as T;

        public abstract void Reset();

        public virtual void Drop()
        {
            DistanceJoint2D.enabled = false;
            Rigidbody2D.gravityScale = 1f;
            transform.SetParent(null);
        }

        public virtual void OnSpawned(IMemoryPool pool)
        {
            this.pool = pool;
            gameObject.SetActive(true);
        }

        public virtual void OnDespawned()
        {
            gameObject.SetActive(false);
            pool = null;
            transform.DOKill();
            transform.parent = poolParent;
        }

        public virtual void ReturnToPool() => OnDespawned();

        public void Dispose()
        {
            if (pool != null)
            {
                pool.Despawn(this);
            }
        }
    }
}