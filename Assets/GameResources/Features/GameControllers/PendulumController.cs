namespace GameResources.Features.GameControllers
{
    using System.Collections;
    using DG.Tweening;
    using Entities.Core;
    using Pools;
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
        
        private BallsPool _ballsPool = default;
        private BaseEntity _currentBall = default;
        private Rigidbody2D _pendulumRigidbody = default;
        private Vector3 _spawnPosition = default;
        private bool _isInit = false;
        
        public void InitializeController(BallsPool ballsPool)
        {
            _ballsPool = ballsPool;

            InitComponents();
            InitAnimations();
            SpawnBall();
        }

        public void StartGame() => _isInit = true;
        
        public void StopGame() => _isInit = false;

        private void LateUpdate()
        {
            if (_isInit && !EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonUp(0))
            {
                DropBall();
            }
        }
        
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
            _currentBall = _ballsPool.Spawn(_ballsPool);
            
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
        
            // ballEntity.IsBelongsZone = null;
                
            _currentBall.Rigidbody2D.isKinematic = false;
        }
        
        private IEnumerator SpawnBallWithDelay()
        {
            yield return new WaitForSeconds(0.1f);
            SpawnBall();
        }
        
        private void DropBall()
        {
            if (_currentBall != null)
            {
                _currentBall.DistanceJoint2D.enabled = false;
                _currentBall.Rigidbody2D.gravityScale = 1f;
                _currentBall.transform.SetParent(null);
                _currentBall = null;
        
                StartCoroutine(SpawnBallWithDelay());
            }
        }
    }
}