namespace GameResources.Features.GameStates
{
    using Core;
    using UnityEngine;

    public sealed class MenuState : IState
    {
        public MenuState(IGameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }
        private readonly IGameStateMachine _gameStateMachine;

        public void Enter()
        {
            Debug.Log($"Enter {nameof(MenuState)}");
            _gameStateMachine.Enter<GameState>();
        }

        public void Exit()
        {
            Debug.Log($"Exit {nameof(MenuState)}");
        }
    }
}