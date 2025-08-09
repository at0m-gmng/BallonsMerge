using System;
using UnityEngine;
using Zenject;

namespace GameResources.Features.VFXEntities
{
    public class VFXEntity : MonoBehaviour, IPoolable<Quaternion, Vector3, IMemoryPool>, IDisposable
    {
        [SerializeField] protected ParticleSystem particleSystem = default;
        protected IMemoryPool pool;
        
        public void OnSpawned(Quaternion rotation, Vector3 position, IMemoryPool pool)
        {
            this.pool = pool;
            transform.rotation = rotation;
            transform.position = position;
            particleSystem.Play();
        }

        public void OnDespawned()
        {
            pool = null;
            particleSystem.Stop();
        }

        public void Dispose()
        {
            if (pool != null)
            {
                pool.Despawn(this);
            }
        }
    }
}