namespace GameResources.Features.Entities
{
    using Core;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public sealed class BallEntity : BaseEntity
    {
        public BallColor Color { get; private set; } = default;
        // public IZoneEntity IsBelongsZone = null;

        [SerializeField] private float _liveTime = 5f;

        public override void Reset() => SetRandomColor();

        public void SetRandomColor()
        {
            Color = (BallColor)Random.Range(0, 3);
            spriteRenderer.color = Color switch
            {
                BallColor.Red => UnityEngine.Color.red,
                BallColor.Blue => UnityEngine.Color.blue,
                BallColor.Green => UnityEngine.Color.green,
                _ => UnityEngine.Color.white
            };
        }
    }
}