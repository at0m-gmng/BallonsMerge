namespace GameResources.Features.ZoneController
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using Entities.Core;
    using UniRx;
    using UniRx.Triggers;
    using UnityEngine;

    public sealed class ZoneEntity : MonoBehaviour
    {
        private const int MAX_HEIGHT = 3;

        public event Action onZoneUpdated;
        
        public bool IsFull => _balls.Count >= MAX_HEIGHT;
        
        [SerializeField] private BaseEntity _ballPrefab = default;
        [SerializeField] private SpriteRenderer _spriteRenderer = default;
        
        private ZoneController _zoneController = default;
        private List<BaseEntity> _balls = new List<BaseEntity>();
        private float _zoneBottomY = default;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private void OnDestroy()
        {
            if (_zoneController != null)
            {
                _zoneController.onZoneUpdate -= RemoveAndUpdateBalls;
            }
            _disposables.Dispose();
        }

        public void Initialize(ZoneController zoneController)
        {
            _zoneController = zoneController;
            CalculateZoneBounds();
            AdjustZoneHeight();
            
            _zoneController.onZoneUpdate += RemoveAndUpdateBalls;

            this.OnTriggerEnter2DAsObservable()
                .Where(collider => collider.TryGetComponent(out BaseEntity _))
                .Subscribe(HandleBallEnter)
                .AddTo(_disposables);
        }

        public IReadOnlyList<BaseEntity> GetBalls() => _balls;
        
        private void HandleBallEnter(Collider2D other)
        {
            BaseEntity ball = other.GetComponent<BaseEntity>();
        
            if (_balls.Count >= MAX_HEIGHT || ball.IsBelongsZone != null)
                return;

            AttachBallToZone(ball);
        }

        private void AttachBallToZone(BaseEntity ball)
        {
            ball.IsBelongsZone = this;
            ball.Rigidbody2D.linearVelocity = Vector2.zero;
            ball.Rigidbody2D.angularVelocity = 0f;
            ball.Rigidbody2D.isKinematic = true;
            ball.Collider2D.enabled = false;

            ball.transform.SetParent(transform);

            float ballLossyScaleY = ball.transform.lossyScale.y;
            float ballHeight = ball.GetCollider<CircleCollider2D>().radius * 2f * ballLossyScaleY;

            Vector3 targetPosition = CalculatePosition(_balls.Count, ballHeight);

            _balls.Add(ball);
            ball.transform.DOMove(targetPosition, 0.3f)
                .OnComplete(() => onZoneUpdated?.Invoke());
        }

        private void AdjustZoneHeight()
        {
            if (_ballPrefab == null)
            {
                return;
            }

            Vector2 originalSize = _spriteRenderer.size;
            float originalBottomY = _zoneBottomY;

            float ballHeight = _ballPrefab.GetCollider<CircleCollider2D>().radius * 2f;
            float newHeight = ballHeight * MAX_HEIGHT;
            
            _spriteRenderer.size = new Vector2(originalSize.x, newHeight);
            
            if (gameObject.TryGetComponent(out BoxCollider2D boxCollider2D))
            {
                boxCollider2D.size = new Vector2(boxCollider2D.size.x, newHeight);
            }

            float newBottomY = _spriteRenderer.bounds.min.y;
            float positionAdjustment = originalBottomY - newBottomY;
            transform.position += new Vector3(0f, positionAdjustment, 0f);
        }

        private void CalculateZoneBounds()
        {
            if (_spriteRenderer != null)
            {
                _zoneBottomY = _spriteRenderer.bounds.min.y;
            }
            else
            {
                _zoneBottomY = transform.position.y - 1f;
            }
        }

        private Vector3 CalculatePosition(int index, float ballHeight)
        {
            float yPos = _zoneBottomY + (ballHeight / 2f);
            yPos += index * ballHeight;
            return new Vector3(transform.position.x, yPos, transform.position.z);
        }

        private async void RemoveAndUpdateBalls(ZoneEntity zoneEntity, List<BaseEntity> ballsToRemove) 
            => await RemoveAndUpdateBallsCoroutine(zoneEntity, ballsToRemove);

        private async UniTask RemoveAndUpdateBallsCoroutine(ZoneEntity zoneEntity, List<BaseEntity> ballsToRemove)
        {
            if (this != zoneEntity)
            {
                return;
            }
            
            foreach (BaseEntity ball in ballsToRemove)
            {
                if (_balls.Contains(ball))
                {
                    Vector3 position = ball.transform.position;
                    PlayParticleEffect(transform.InverseTransformPoint(position));
                    _balls.Remove(ball);

                    ball.ReturnToPool();
                }
            }
            await UpdateBallPositions();
            onZoneUpdated();
        }

        private async UniTask UpdateBallPositions()
        {
            if (_balls.Count == 0)
                return;

            BaseEntity ball = _balls.First();
            float ballLossyScaleY = ball.transform.lossyScale.y;
            float ballHeight = ball.GetCollider<CircleCollider2D>().radius * 2f * ballLossyScaleY;

            Sequence sequence = DOTween.Sequence();
            for (int i = 0; i < _balls.Count; i++)
            {
                Vector3 targetPosition = CalculatePosition(i, ballHeight);
                sequence.Join(_balls[i].transform.DOMove(targetPosition, 0.3f).SetEase(Ease.OutBounce));
            }

            await UniTask.Delay(TimeSpan.FromSeconds(sequence.Duration()));
        }

        private void PlayParticleEffect(Vector3 position)
        {
            
        }

    }
}