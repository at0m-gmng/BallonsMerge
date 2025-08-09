using UnityEngine;

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
        
        [SerializeField] private Camera _camera = default;
        [SerializeField] private PendulumController _pendulumController = default;
        [SerializeField] private ZoneController _zoneController = default;

        public void Initialize()
        {
            _pendulumController.InitializeController(_ballFactory);
            _zoneController.InitializeController();
        }
        public void StartGame() => _pendulumController.StartGame();
        
        public void StopGame() => _pendulumController.StopGame();
    }
}
