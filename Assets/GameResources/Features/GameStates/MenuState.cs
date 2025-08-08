namespace GameResources.Features.GameStates
{
    using Core;
    using UISystem;
    using UISystem.SO;
    using UnityEngine;

    public sealed class MenuState : IState
    {
        public MenuState(IGameStateMachine gameStateMachine, IUISystem uiSystem)
        {
            _gameStateMachine = gameStateMachine;
            _uiSystem = uiSystem;
        }
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IUISystem _uiSystem;

        public void Enter()
        {
            Debug.Log($"Enter {nameof(MenuState)}");
            _uiSystem.ShowWindow(UIWindowID.Menu);
            _gameStateMachine.Enter<GameState>();
        }

        public void Exit()
        {
            _uiSystem.HideWindow(UIWindowID.Menu);
            Debug.Log($"Exit {nameof(MenuState)}");
        }
    }
}