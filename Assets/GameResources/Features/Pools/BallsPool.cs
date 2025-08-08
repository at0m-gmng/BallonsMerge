namespace GameResources.Features.Pools
{
    using Entities.Core;
    using Zenject;

    public sealed class BallsPool : MemoryPool<IMemoryPool, BaseEntity>
    {
        protected override void OnCreated(BaseEntity item)
        {
            item.gameObject.SetActive(false);
        }

        protected override void OnSpawned(BaseEntity item)
        {
            item.Reset();
            item.gameObject.SetActive(true);
        }

        protected override void OnDespawned(BaseEntity item)
        {
            item.gameObject.SetActive(false);
        }
    }
}