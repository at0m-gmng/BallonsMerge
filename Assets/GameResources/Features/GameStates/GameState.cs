namespace GameResources.Features.GameStates
{
    using Core;
    using GameControllers;
    using UISystem;
    using UISystem.SO;
    using UnityEngine;

    public sealed class GameState : IState
    {
        public GameState(IGameStateMachine gameStateMachine, IUISystem uiSystem, GameFacade gameFacade)
        {
            _gameStateMachine = gameStateMachine;
            _uiSystem = uiSystem;
            _gameFacade = gameFacade;
        }
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IUISystem _uiSystem;
        private readonly GameFacade _gameFacade;
        
        private GameWindow _gameWindow = default;
        private PauseWindow _pauseWindow = default;

        public void Enter()
        {
            Debug.Log($"Enter {nameof(GameState)}");
            _uiSystem.TryGetWindow(UIWindowID.Game, out _gameWindow);
            _uiSystem.TryGetWindow(UIWindowID.Pause, out _pauseWindow);

            _gameWindow.ButtonPause.onClick.AddListener(OnButtonPauseClicked);
            _pauseWindow.ButtonContinue.onClick.AddListener(OnButtonContinueClicked);
            _pauseWindow.ButtonMenu.onClick.AddListener(OnButtonMenuClicked);
            
            _uiSystem.ShowWindow(UIWindowID.Game);

            _gameFacade.StartGame();
        }

        public void Exit()
        {
            _gameFacade.StopGame();
            _gameWindow.ButtonPause.onClick.RemoveListener(OnButtonPauseClicked);
            _pauseWindow.ButtonContinue.onClick.RemoveListener(OnButtonContinueClicked);
            _pauseWindow.ButtonMenu.onClick.RemoveListener(OnButtonMenuClicked);
            _uiSystem.HideWindow(UIWindowID.Pause);
            _uiSystem.HideWindow(UIWindowID.Game);
            Debug.Log($"Exit {nameof(GameState)}");
        }

        private void OnButtonPauseClicked()
        {
            _gameFacade.StopGame();
            _uiSystem.ShowWindow(UIWindowID.Pause);
        }

        private void OnButtonContinueClicked()
        {
            _gameFacade.StartGame();
            _uiSystem.HideWindow(UIWindowID.Pause);
        }
        
        private void OnButtonMenuClicked() => _gameStateMachine.Enter<MenuState>();
    }
}