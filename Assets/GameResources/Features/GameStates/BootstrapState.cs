﻿namespace GameResources.Features.GameStates
{
    using Core;
    using GameControllers;
    using UISystem;
    using UnityEngine;

    public sealed class BootstrapState : IState
    {
        public BootstrapState(IGameStateMachine gameStateMachine, IUISystem uiSystem, GameFacade gameFacade)
        {
            _gameStateMachine = gameStateMachine;
            _uiSystem = uiSystem;
            _gameFacade = gameFacade;
        }
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IUISystem _uiSystem;
        private readonly GameFacade _gameFacade;

        public async void Enter()
        {
            Debug.Log($"Enter {nameof(BootstrapState)}");
            await _uiSystem.Initialize();
            _gameFacade.Initialize();
            _gameStateMachine.Enter<MenuState>();
        }

        public void Exit()
        {
            Debug.Log($"Exit {nameof(BootstrapState)}");
        }
    }
}