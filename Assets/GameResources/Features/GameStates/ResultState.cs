namespace GameResources.Features.GameStates
{
    using Core;
    using UISystem;
    using UISystem.SO;
    using UnityEngine;

    public sealed class ResultState : IState
    {
        public ResultState(IGameStateMachine gameStateMachine, IUISystem uiSystem)
        {
            _gameStateMachine = gameStateMachine;
            _uiSystem = uiSystem;
        }
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IUISystem _uiSystem;
        
        public void Enter()
        {
            Debug.Log($"Enter {nameof(ResultState)}");
            _uiSystem.ShowWindow(UIWindowID.Result);
        }

        public void Exit()
        {
            _uiSystem.HideWindow(UIWindowID.Result);
            Debug.Log($"Exit {nameof(ResultState)}");
        }
    }
}