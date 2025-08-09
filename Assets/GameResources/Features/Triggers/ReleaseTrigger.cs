using GameResources.Features.Entities;
using UniRx;
using UnityEngine;

namespace GameResources.Features.Triggers
{
    using UniRx.Triggers;

    public sealed class ReleaseTrigger : MonoBehaviour
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private void Start()
        {
            this.OnTriggerEnter2DAsObservable()
                .Where(collider => collider.TryGetComponent(out BallEntity _))
                .Subscribe(collider => collider.GetComponent<BallEntity>().ReturnToPool())
                .AddTo(_disposables);
        }

        private void OnDestroy() => _disposables.Dispose();
    }
}