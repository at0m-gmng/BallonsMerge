namespace GameResources.Features.GameStates
{
    using Core;
    using UnityEngine;

    public sealed class GameState : IState
    {
        public GameState(IGameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }
        private readonly IGameStateMachine _gameStateMachine;
      
        public void Enter()
        {
            Debug.Log($"Enter {nameof(GameState)}");
            _gameStateMachine.Enter<ResultState>();
        }

        public void Exit()
        {
            Debug.Log($"Exit {nameof(GameState)}");
        }
    }
}