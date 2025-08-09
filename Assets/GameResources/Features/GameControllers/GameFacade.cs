using System.Collections.Generic;
using GameResources.Features.Entities.Core;
using GameResources.Features.PersistentProgress;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Action = System.Action;

namespace GameResources.Features.GameControllers
{
    using Factories;
    using Zenject;
    using ZoneController;

    public sealed class GameFacade : MonoBehaviour
    {
        [Inject]
        private void Construct(BallFactory ballFactory, IPersistentListener persistentListener, ScoreConfig scoreConfig)
        {
            _ballFactory = ballFactory;
            _persistentListener = persistentListener;
            _scoreConfig = scoreConfig;
        }
        private BallFactory _ballFactory;
        private IPersistentListener _persistentListener;
        private ScoreConfig _scoreConfig;

        public event Action GameEnded;
        
        public RenderTexture GraphicsTexture => _renderTexture;
        private RenderTexture _renderTexture;

        [SerializeField] private Camera _camera = default;
        [SerializeField] private PendulumController _pendulumController = default;
        [SerializeField] private ZoneController _zoneController = default;

        private void OnDestroy()
        {
            _zoneController.onZoneUpdate -= OnZoneUpdated;
            _zoneController.onEmptyZonesAreOver -= OnAllZonesAreFull;
        }

        public void Initialize()
        {
            _pendulumController.InitializeController(_ballFactory);
            _zoneController.InitializeController();
            
            _renderTexture = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGB32)
            {
                graphicsFormat = GraphicsFormat.R8G8B8A8_UNorm,
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            _camera.targetTexture = _renderTexture;
            
            _zoneController.onZoneUpdate += OnZoneUpdated;
            _zoneController.onEmptyZonesAreOver += OnAllZonesAreFull;
        }

        public void StartGame()
        {
            _pendulumController.StartGame();
            _persistentListener.Score.Value = 0;
        }
        public void StopGame() => _pendulumController.StopGame();
        public void ContinueGame() => _pendulumController.StartGame();
        
        private void OnZoneUpdated(ZoneEntity zoneEntity, List<BaseEntity> balls)
        {
            for (int i = 0; i < balls.Count; i++)
            {
                _persistentListener.Score.Value += _scoreConfig.GetScore(balls[i].Type);
            }
        }
        
        private void OnAllZonesAreFull()
        {
            GameEnded?.Invoke();
            StopGame();
        }
    }
}
