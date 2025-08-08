namespace GameResources.Features.GameStates
{
    using Core;
    using UnityEngine;

    public sealed class ResultState : IState
    {
        public ResultState(IGameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }
        private readonly IGameStateMachine _gameStateMachine;
        
        public void Enter()
        {
            Debug.Log($"Enter {nameof(ResultState)}");
            // _gameStateMachine.Enter<MenuState>();
        }

        public void Exit()
        {
            Debug.Log($"Exit {nameof(ResultState)}");
        }
    }
}