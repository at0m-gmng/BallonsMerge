using GameResources.Features.PersistentProgress;
using GameResources.Features.SaveLoadSystem;
using GameResources.Features.VFXEntities;

namespace GameResources.Features.Installers
{
    using Entities;
    using Entities.Core;
    using Factories;
    using GameControllers;
    using GameStates;
    using GameStates.Core;
    using UISystem;
    using UISystem.SO;
    using UnityEngine;
    using Zenject;

    public sealed class GameInstaller : MonoInstaller
    {
        [Header("Configs")]
        [SerializeField] private UIConfig _uiConfig = default;
        [SerializeField] private ScoreConfig _scoreConfig = default;
        
        [Header("Controllers")]
        [SerializeField] private GameFacade _gameFacade = default;
        
        [Header("Pools")]
        [SerializeField] private BaseEntity _baseEntity = default;
        [SerializeField] private VFXEntity _vfxEntity = default;
        
        public override void InstallBindings()
        {
            InstallControllers();
            InstallStateMachine();
        }

        private void InstallControllers()
        {
            Container.BindInstance(_scoreConfig).IfNotBound();
            
            Container.BindInterfacesTo<SaveLoadService>().AsSingle();
            Container.BindInterfacesTo<PersistentProgressService>().AsSingle();
            
            Container.BindFactory<Quaternion, Vector3, VFXEntity, VFXFactory>()
                .FromMonoPoolableMemoryPool(x => x.WithInitialSize(10)
                .FromComponentInNewPrefab(_vfxEntity)
                .UnderTransformGroup("VFXPool"));
            
            Container.BindFactory<BallEntity, BallFactory>()
                .FromMonoPoolableMemoryPool(x => x.WithInitialSize(15)
                .FromComponentInNewPrefab(_baseEntity)
                .UnderTransformGroup("BallsPool"));
            
            Container.BindInstance(_gameFacade).AsSingle();
            Container.BindInterfacesTo<UISystem>().AsSingle().WithArguments(_uiConfig);
        }

        private void InstallStateMachine()
        {
            Container.BindInterfacesTo<GameStateMachine>().AsSingle();
            Container.BindInterfacesTo<BootstrapState>().AsSingle();
            Container.BindInterfacesTo<MenuState>().AsSingle();
            Container.BindInterfacesTo<GameState>().AsSingle();
            Container.BindInterfacesTo<ResultState>().AsSingle();
        }
    }
}