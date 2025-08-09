using System;
using GameResources.Features.PersistentProgress;
using UniRx;

namespace GameResources.Features.GameStates
{
    using Core;
    using GameControllers;
    using UISystem;
    using UISystem.SO;
    using UnityEngine;

    public sealed class GameState : IState
    {
        public GameState(IGameStateMachine gameStateMachine, IUISystem uiSystem, GameFacade gameFacade, IPersistentListener persistentListener)
        {
            _gameStateMachine = gameStateMachine;
            _uiSystem = uiSystem;
            _gameFacade = gameFacade;
            _persistentListener = persistentListener;
        }
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IUISystem _uiSystem;
        private readonly GameFacade _gameFacade;
        private readonly IPersistentListener _persistentListener;
        
        private GameWindow _gameWindow = default;
        private PauseWindow _pauseWindow = default;
        private IDisposable _scoreChanged = null;

        public void Enter()
        {
            Debug.Log($"Enter {nameof(GameState)}");
            _uiSystem.TryGetWindow(UIWindowID.Game, out _gameWindow);
            _uiSystem.TryGetWindow(UIWindowID.Pause, out _pauseWindow);

            _gameWindow.GraphicsTexture.texture = _gameFacade.GraphicsTexture;
            
            _gameWindow.ButtonPause.onClick.AddListener(OnButtonPauseClicked);
            _pauseWindow.ButtonContinue.onClick.AddListener(OnButtonContinueClicked);
            _pauseWindow.ButtonMenu.onClick.AddListener(OnButtonMenuClicked);
            
            _uiSystem.ShowWindow(UIWindowID.Game);
            _scoreChanged = _persistentListener.Score.Subscribe(OnGameScoreChanged);

            _gameFacade.GameEnded += OnGameEnded;
            _gameFacade.StartGame();
        }

        public void Exit()
        {
            _scoreChanged.Dispose();
            _gameFacade.GameEnded -= OnGameEnded;
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
            _gameFacade.ContinueGame();
            _uiSystem.HideWindow(UIWindowID.Pause);
        }
        
        private void OnButtonMenuClicked() => _gameStateMachine.Enter<MenuState>();

        private void OnGameScoreChanged(int score) => _gameWindow.UpdateView(score);

        private void OnGameEnded() => _gameStateMachine.Enter<ResultState>();
    }
}