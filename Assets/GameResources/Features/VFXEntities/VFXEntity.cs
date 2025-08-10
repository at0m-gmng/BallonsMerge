using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace GameResources.Features.VFXEntities
{
    public class VFXEntity : MonoBehaviour, IPoolable<Quaternion, Vector3, IMemoryPool>, IDisposable
    {
        [SerializeField] protected ParticleSystem particleSystem = default;
        [SerializeField] protected float liveTime = 3f;
        
        protected IMemoryPool pool;
        protected CompositeDisposable disposables = new CompositeDisposable();

        public virtual void OnSpawned(Quaternion rotation, Vector3 position, IMemoryPool pool)
        {
            this.pool = pool;
            transform.rotation = rotation;
            transform.position = position;
            particleSystem.Play();
            disposables?.Clear();
            Observable.Timer(TimeSpan.FromSeconds(liveTime))
                .Subscribe(_ => ReturnToPool())
                .AddTo(disposables);
        }

        public virtual void OnDespawned()
        {
            pool = null;
            particleSystem.Stop();
        }

        public virtual void Dispose()
        {
            if (pool != null)
            {
                pool.Despawn(this);
            }
            disposables?.Dispose();
        }
        
        protected virtual void ReturnToPool()
        {
            if (pool != null)
            {
                pool.Despawn(this);
            }
        }
    }
}