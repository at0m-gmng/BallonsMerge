namespace GameResources.Features.Entities
{
    using Core;
    using UnityEngine;
    using Zenject;
    using Random = UnityEngine.Random;

    public sealed class BallEntity : BaseEntity
    {
        [SerializeField] private float _liveTime = 5f;

        public override void Reset() => SetRandomColor();

        public override void OnSpawned(IMemoryPool pool)
        {
            Reset();
            base.OnSpawned(pool);
        }

        private void SetRandomColor()
        {
            Type = (EntityType)Random.Range(0, 3);
            spriteRenderer.color = Type switch
            {
                EntityType.Red => UnityEngine.Color.red,
                EntityType.Blue => UnityEngine.Color.blue,
                EntityType.Green => UnityEngine.Color.green,
                _ => UnityEngine.Color.white
            };
        }
    }
}