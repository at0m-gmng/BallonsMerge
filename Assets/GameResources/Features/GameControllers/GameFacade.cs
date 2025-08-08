using UnityEngine;

namespace GameResources.Features.GameControllers
{
    using Pools;
    using Zenject;

    public sealed class GameFacade : MonoBehaviour
    {
        [Inject]
        private void Construct(BallsPool ballsPool)
        {
            _ballsPool = ballsPool;
        }
        private BallsPool _ballsPool;
        
        [SerializeField] private Camera _camera = default;
        [SerializeField] private PendulumController _pendulumController = default;


        public void Initialize()
        {
            _pendulumController.InitializeController(_ballsPool);
        }
        public void StartGame() => _pendulumController.StartGame();
        
        public void StopGame() => _pendulumController.StopGame();
    }
}
