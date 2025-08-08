namespace GameResources.Features.GameStates
{
    using Core;
    using UnityEngine;

    public sealed class BootstrapState : IState
    {
        public BootstrapState(IGameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }
        private readonly IGameStateMachine _gameStateMachine;
        
        public void Enter()
        {
            Debug.Log($"Enter {nameof(BootstrapState)}");
            _gameStateMachine.Enter<MenuState>();
        }

        public void Exit()
        {
            Debug.Log($"Exit {nameof(BootstrapState)}");
        }
    }
}