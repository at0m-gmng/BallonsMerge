namespace GameResources.Features.Installers
{
    using GameStates;
    using GameStates.Core;
    using UISystem;
    using UISystem.SO;
    using UnityEngine;
    using Zenject;

    public sealed class GameInstaller : MonoInstaller
    {
        [SerializeField] private UIConfig _uiConfig = default;
        
        public override void InstallBindings()
        {
            InstallControllers();
            InstallStateMachine();
        }

        private void InstallControllers() => Container.BindInterfacesTo<UISystem>().AsSingle().WithArguments(_uiConfig);

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