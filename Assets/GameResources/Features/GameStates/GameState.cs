namespace GameResources.Features.GameStates
{
    using Core;
    using UISystem;
    using UISystem.SO;
    using UnityEngine;

    public sealed class GameState : IState
    {
        public GameState(IGameStateMachine gameStateMachine, IUISystem uiSystem)
        {
            _gameStateMachine = gameStateMachine;
            _uiSystem = uiSystem;
        }
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IUISystem _uiSystem;
        
        public void Enter()
        {
            Debug.Log($"Enter {nameof(GameState)}");
            _uiSystem.ShowWindow(UIWindowID.Game);
            _gameStateMachine.Enter<ResultState>();
        }

        public void Exit()
        {
            _uiSystem.HideWindow(UIWindowID.Game);
            Debug.Log($"Exit {nameof(GameState)}");
        }
    }
}