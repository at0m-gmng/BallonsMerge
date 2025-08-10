using UnityEngine;

namespace GameResources.Features.Entities
{
    using Core;
    using Zenject;
    using Random = UnityEngine.Random;

    public sealed class BallEntity : BaseEntity
    {
        [SerializeField] private Sprite[] _colorSprites = new Sprite[3];
        
        public override void Reset() => SetRandomColor();

        public override void OnSpawned(IMemoryPool pool)
        {
            Reset();
            base.OnSpawned(pool);
        }

        private void SetRandomColor()
        {
            int randomIndex = Random.Range(0, 3);
            Type = (EntityType)randomIndex;
            spriteRenderer.sprite = _colorSprites[randomIndex];
        }
    }
}