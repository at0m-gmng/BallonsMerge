namespace GameResources.Features.GameControllers
{
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using Entities.Core;
    using Factories;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public sealed class PendulumController : MonoBehaviour
    {
        [SerializeField] 
        private float _swingAngle = 30f;
        
        [SerializeField] 
        private float _swingSpeed = 1f;
        
        [SerializeField] 
        private float _ropeLength = 1f;
        
        private BallFactory _ballsPool = default;
        private BaseEntity _currentBall = default;
        private Rigidbody2D _pendulumRigidbody = default;
        private Vector3 _spawnPosition = default;
        private bool _isInit = false;
        private bool _isDropping = false;
        
        private void LateUpdate()
        {
            if (_isInit && !EventSystem.current.IsPointerOverGameObject() && !_isDropping && Input.GetMouseButtonUp(0))
            {
                DropBall().Forget();
            }
        }
        
        public void InitializeController(BallFactory ballsPool)
        {
            _ballsPool = ballsPool;

            InitComponents();
            InitAnimations();
            SpawnBall();
        }

        public void StartGame() => _isInit = true;
        
        public void StopGame() => _isInit = false;
        
        private void InitComponents()
        {
            _pendulumRigidbody = GetComponent<Rigidbody2D>();
            if (_pendulumRigidbody != null)
            {
                _pendulumRigidbody.isKinematic = true;
            }
        }
        
        private void InitAnimations()
        {
            transform.rotation = Quaternion.Euler(0, 0, -_swingAngle);
            transform.DORotate(new Vector3(0, 0, _swingAngle), _swingSpeed)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
        
        private void SpawnBall()
        {
            _spawnPosition = transform.TransformPoint(Vector3.down * _ropeLength);
            _currentBall = _ballsPool.Create();
            
            if (!_currentBall)
            {
                return;
            }
            _currentBall.transform.SetParent(transform);
            _currentBall.transform.position = _spawnPosition;
        
            _currentBall.Collider2D.enabled = true;
            _currentBall.Rigidbody2D.isKinematic = true;
            _currentBall.Rigidbody2D.gravityScale = 0f;
            _currentBall.DistanceJoint2D.connectedBody = _pendulumRigidbody;
            _currentBall.DistanceJoint2D.distance = _ropeLength;
            _currentBall.DistanceJoint2D.enabled = true;
            _currentBall.Rigidbody2D.isKinematic = false;
        }
        
        private async UniTask DropBall()
        {
            if (_currentBall != null)
            {
                _isDropping = true;
                _currentBall.Drop();
                _currentBall = null;
                await UniTask.Delay(100);
                SpawnBall();
                _isDropping = false;
            }
        }
    }
}