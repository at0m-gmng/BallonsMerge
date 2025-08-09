using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace GameResources.Features.GameControllers
{
    using Factories;
    using Zenject;
    using ZoneController;

    public sealed class GameFacade : MonoBehaviour
    {
        [Inject]
        private void Construct(BallFactory ballFactory)
        {
            _ballFactory = ballFactory;
        }
        private BallFactory _ballFactory;
        
        public RenderTexture GraphicsTexture => _renderTexture;
        private RenderTexture _renderTexture;

        [SerializeField] private Camera _camera = default;
        [SerializeField] private PendulumController _pendulumController = default;
        [SerializeField] private ZoneController _zoneController = default;

        
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
        }
        public void StartGame() => _pendulumController.StartGame();
        
        public void StopGame() => _pendulumController.StopGame();
    }
}
