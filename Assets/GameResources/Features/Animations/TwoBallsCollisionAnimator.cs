using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace GameResources.Features.Animations
{
    public sealed class TwoBallsPhysicsCollisionUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform _boundsRect;
        [SerializeField] private List<GameObject> _ballPrefabs = new List<GameObject>();

        [Header("Collision geometry")]
        [SerializeField] private List<RectTransform> _collisionTargets = new List<RectTransform>();
        [HideInInspector] [SerializeField] private float _collisionPointY = 40f;
        [SerializeField] private float _approachDuration = 0.9f;

        [Header("Physics")]
        [SerializeField] private float _gravity = -2000f;
        [SerializeField] private float _massLeft = 1f;
        [SerializeField] private float _massRight = 1f;
        [Range(0f, 1f)] [SerializeField] private float _restitution = 0.7f;
        [SerializeField] private float _collisionDownwardBias = 200f;
        [SerializeField] private float _collisionTangentialBias = 120f;

        [Header("Visual FX")]
        [SerializeField] private Vector3 _impactScale = new Vector3(1.18f, 0.78f, 1f);
        [SerializeField] private float _impactIn = 0.06f;
        [SerializeField] private float _impactOut = 0.12f;
        [SerializeField] private float _punchMagnitude = 8f;
        [SerializeField] private float _punchDuration = 0.26f;
        [SerializeField] private float _fadeDuration = 0.35f;

        [Header("Looping")]
        [SerializeField] private float _restartDelay = 0.5f;
        [SerializeField] private float _bottomScreenOffset = 100f;

        private List<BallPair> _ballPairs = new List<BallPair>();
        private float _calculatedBottomOffset;
        private Tween _tickTween;
        private float _lastUpdateTime;

        private sealed class BallPair
        {
            public RectTransform leftRT;
            public RectTransform rightRT;
            public CanvasGroup leftCG;
            public CanvasGroup rightCG;
            public Vector2 positionLeft;
            public Vector2 positionRight;
            public Vector2 velocityLeft;
            public Vector2 velocityRight;
            public float radiusLeft;
            public float radiusRight;
            public bool isCollided;
            public bool isRunning;
            public Vector2 currentCollisionPoint;
            public RectTransform assignedTarget;
        }

        private void Awake()
        {
            for (int pairIndex = 0; pairIndex < 3; pairIndex++)
            {
                BallPair ballPair = new BallPair();
                _ballPairs.Add(ballPair);
            }

            CalculateBottomScreenBoundary();
            CreateBallPairs();
        }

        private void OnEnable() => StartAnimationCycle();

        private void OnDisable()
        {
            StopAnimationCycle();
            
            foreach (BallPair ballPair in _ballPairs)
            {
                DOTween.Kill(ballPair.leftRT);
                DOTween.Kill(ballPair.rightRT);
                ballPair.leftCG?.DOKill();
                ballPair.rightCG?.DOKill();
            }
        }

        private void CalculateBottomScreenBoundary() => _calculatedBottomOffset = -Screen.height / 2 + _bottomScreenOffset;

        private void CreateBallPairs()
        {
            for (int pairIndex = 0; pairIndex < _ballPairs.Count; pairIndex++)
            {
                BallPair ballPair = _ballPairs[pairIndex];
                
                GameObject selectedPrefab = _ballPrefabs[Random.Range(0, _ballPrefabs.Count)];
                
                if (ballPair.leftRT != null) Destroy(ballPair.leftRT.gameObject);
                if (ballPair.rightRT != null) Destroy(ballPair.rightRT.gameObject);
                
                GameObject leftBall = Instantiate(selectedPrefab, _boundsRect);
                GameObject rightBall = Instantiate(selectedPrefab, _boundsRect);
                leftBall.name = $"Ball_Left_{pairIndex}";
                rightBall.name = $"Ball_Right_{pairIndex}";

                ballPair.leftRT = leftBall.GetComponent<RectTransform>();
                ballPair.rightRT = rightBall.GetComponent<RectTransform>();
                ballPair.leftCG = leftBall.GetComponent<CanvasGroup>();
                ballPair.rightCG = rightBall.GetComponent<CanvasGroup>();

                ballPair.radiusLeft = ballPair.leftRT.rect.width * 0.5f;
                ballPair.radiusRight = ballPair.rightRT.rect.width * 0.5f;
            }
        }

        private void SetupInitialPositions()
        {
            if (_boundsRect == null)
                return;

            Rect boundsRect = _boundsRect.rect;

            foreach (BallPair ballPair in _ballPairs)
            {
                if (ballPair.leftRT == null || ballPair.rightRT == null)
                    continue;

                float halfWidth = boundsRect.width * 0.5f;
                float offsetDistance = halfWidth + Mathf.Max(ballPair.radiusLeft, ballPair.radiusRight) * 2f;
                
                float leftX = boundsRect.xMin - offsetDistance;
                float rightX = boundsRect.xMax + offsetDistance;

                float startYLeft = Random.Range(-30f, 30f);
                float startYRight = Random.Range(-30f, 30f);

                ballPair.positionLeft = new Vector2(leftX, startYLeft);
                ballPair.positionRight = new Vector2(rightX, startYRight);

                ballPair.leftRT.anchoredPosition = ballPair.positionLeft;
                ballPair.rightRT.anchoredPosition = ballPair.positionRight;

                ballPair.leftRT.localScale = Vector3.one;
                ballPair.rightRT.localScale = Vector3.one;

                if (ballPair.leftCG != null) ballPair.leftCG.alpha = 1f;
                if (ballPair.rightCG != null) ballPair.rightCG.alpha = 1f;
            }
        }

        private void StartAnimationCycle()
        {
            ResetAnimationState();
            StartPhysicsUpdate();
        }

        private void StopAnimationCycle()
        {
            foreach (BallPair ballPair in _ballPairs)
            {
                ballPair.isRunning = false;
            }
        }

        private void ResetAnimationState()
        {
            CreateBallPairs();
            AssignTargetsToBallPairs();
            SetupInitialPositions();

            foreach (BallPair ballPair in _ballPairs)
            {
                ballPair.isCollided = false;
                ballPair.isRunning = true;

                Vector2 gravityVector = new Vector2(0f, _gravity);
                ballPair.velocityLeft = ComputeInitialVelocity(ballPair.positionLeft, ballPair.currentCollisionPoint, gravityVector, _approachDuration);
                ballPair.velocityRight = ComputeInitialVelocity(ballPair.positionRight, ballPair.currentCollisionPoint, gravityVector, _approachDuration);
            }
        }

        private void AssignTargetsToBallPairs()
        {
            List<RectTransform> availableTargets = _collisionTargets.Where(target => target != null).ToList();
            
            if (availableTargets.Count == 0)
            {
                foreach (BallPair ballPair in _ballPairs)
                {
                    ballPair.currentCollisionPoint = new Vector2(0f, _collisionPointY);
                    ballPair.assignedTarget = null;
                }
                return;
            }

            List<int> targetIndices = Enumerable.Range(0, availableTargets.Count).ToList();
            
            for (int pairIndex = 0; pairIndex < _ballPairs.Count; pairIndex++)
            {
                BallPair ballPair = _ballPairs[pairIndex];
                
                if (targetIndices.Count > 0)
                {
                    int randomIndex = Random.Range(0, targetIndices.Count);
                    int targetIndex = targetIndices[randomIndex];
                    targetIndices.RemoveAt(randomIndex);
                    
                    ballPair.assignedTarget = availableTargets[targetIndex];
                    ballPair.currentCollisionPoint = ballPair.assignedTarget.anchoredPosition;
                }
                else
                {
                    ballPair.currentCollisionPoint = new Vector2(0f, _collisionPointY);
                    ballPair.assignedTarget = null;
                }
            }
        }

        private Vector2 ComputeInitialVelocity(Vector2 startPosition, Vector2 targetPosition, Vector2 gravityVector, float timeDuration) 
            => (targetPosition - startPosition - 0.5f * gravityVector * (timeDuration * timeDuration)) / timeDuration;

        private void StartPhysicsUpdate()
        {
            _lastUpdateTime = Time.unscaledTime;
            _tickTween = DOVirtual.Float(0f, 1f, 99999f, _ => UpdatePhysics()).SetUpdate(true);
        }

        private void UpdatePhysics()
        {
            float currentTime = Time.unscaledTime;
            float deltaTime = Mathf.Clamp(currentTime - _lastUpdateTime, 0f, 0.04f);
            _lastUpdateTime = currentTime;

            Vector2 gravityVector = new Vector2(0f, _gravity);

            bool allPairsCompleted = true;

            foreach (BallPair ballPair in _ballPairs)
            {
                if (!ballPair.isRunning) 
                {
                    continue;
                }

                allPairsCompleted = false;

                ballPair.velocityLeft += gravityVector * deltaTime;
                ballPair.velocityRight += gravityVector * deltaTime;
                ballPair.positionLeft += ballPair.velocityLeft * deltaTime;
                ballPair.positionRight += ballPair.velocityRight * deltaTime;

                ballPair.leftRT.anchoredPosition = ballPair.positionLeft;
                ballPair.rightRT.anchoredPosition = ballPair.positionRight;

                float distanceBetweenBalls = Vector2.Distance(ballPair.positionLeft, ballPair.positionRight);
                float minimumDistance = ballPair.radiusLeft + ballPair.radiusRight;
                
                if (!ballPair.isCollided && distanceBetweenBalls <= minimumDistance)
                {
                    ProcessBallCollision(ballPair);
                }

                if (ballPair.isCollided)
                {
                    bool leftBallReachedBottom = ballPair.positionLeft.y < _calculatedBottomOffset;
                    bool rightBallReachedBottom = ballPair.positionRight.y < _calculatedBottomOffset;

                    if (leftBallReachedBottom && rightBallReachedBottom)
                    {
                        ballPair.isRunning = false;
                        DOTween.Sequence().AppendInterval(_restartDelay).OnComplete(() =>
                        {
                            AssignNewTargetToPair(ballPair);
                            ballPair.isRunning = true;
                        }).SetUpdate(true);
                    }
                }
            }

            if (allPairsCompleted)
            {
                DOTween.Sequence().AppendInterval(_restartDelay).OnComplete(() =>
                {
                    ResetAnimationState();
                }).SetUpdate(true);
            }
        }

        private void AssignNewTargetToPair(BallPair ballPair)
        {
            List<RectTransform> availableTargets = GetAvailableTargets();
            
            if (availableTargets.Count > 0)
            {
                RectTransform newTarget = availableTargets[Random.Range(0, availableTargets.Count)];
                ballPair.assignedTarget = newTarget;
                ballPair.currentCollisionPoint = newTarget.anchoredPosition;
                ResetPairState(ballPair);
            }
            else
            {
                ballPair.currentCollisionPoint = new Vector2(0f, _collisionPointY);
                ballPair.assignedTarget = null;
                ResetPairState(ballPair);
            }
        }

        private List<RectTransform> GetAvailableTargets()
        {
            List<RectTransform> allTargets = _collisionTargets.Where(target => target != null).ToList();
            
            List<RectTransform> usedTargets = _ballPairs
                .Where(pair => pair.isRunning && pair.assignedTarget != null)
                .Select(pair => pair.assignedTarget)
                .ToList();
            
            return allTargets.Except(usedTargets).ToList();
        }

        private void ResetPairState(BallPair ballPair)
        {
            ballPair.isCollided = false;
            
            if (_boundsRect != null && ballPair.leftRT != null && ballPair.rightRT != null)
            {
                Rect boundsRect = _boundsRect.rect;
                float halfWidth = boundsRect.width * 0.5f;
                float offsetDistance = halfWidth + Mathf.Max(ballPair.radiusLeft, ballPair.radiusRight) * 2f;
                
                float leftX = boundsRect.xMin - offsetDistance;
                float rightX = boundsRect.xMax + offsetDistance;

                float startYLeft = Random.Range(-30f, 30f);
                float startYRight = Random.Range(-30f, 30f);

                ballPair.positionLeft = new Vector2(leftX, startYLeft);
                ballPair.positionRight = new Vector2(rightX, startYRight);

                ballPair.leftRT.anchoredPosition = ballPair.positionLeft;
                ballPair.rightRT.anchoredPosition = ballPair.positionRight;

                ballPair.leftRT.localScale = Vector3.one;
                ballPair.rightRT.localScale = Vector3.one;

                if (ballPair.leftCG != null) ballPair.leftCG.alpha = 1f;
                if (ballPair.rightCG != null) ballPair.rightCG.alpha = 1f;

                Vector2 gravityVector = new Vector2(0f, _gravity);
                ballPair.velocityLeft = ComputeInitialVelocity(ballPair.positionLeft, ballPair.currentCollisionPoint, gravityVector, _approachDuration);
                ballPair.velocityRight = ComputeInitialVelocity(ballPair.positionRight, ballPair.currentCollisionPoint, gravityVector, _approachDuration);
            }
        }

        private void ProcessBallCollision(BallPair ballPair)
        {
            ballPair.isCollided = true;

            Vector2 collisionNormal = (ballPair.positionLeft - ballPair.positionRight).normalized;
            if (collisionNormal == Vector2.zero) collisionNormal = Vector2.right;

            Vector2 relativeVelocity = ballPair.velocityLeft - ballPair.velocityRight;
            float velocityAlongNormal = Vector2.Dot(relativeVelocity, collisionNormal);

            if (velocityAlongNormal > 0f)
            {
                ExecuteCollisionEffects(ballPair);
            }
            else
            {
                float restitutionCoefficient = _restitution;
                float inverseMassSum = 1f / _massLeft + 1f / _massRight;
                float impulseMagnitude = -(1f + restitutionCoefficient) * velocityAlongNormal / inverseMassSum;

                Vector2 impulseVector = impulseMagnitude * collisionNormal;

                ballPair.velocityLeft += impulseVector / _massLeft;
                ballPair.velocityRight -= impulseVector / _massRight;

                Vector2 tangentVector = new Vector2(-collisionNormal.y, collisionNormal.x);
                float tangentBiasLeft = _collisionTangentialBias * Random.Range(0.8f, 1.2f);
                float tangentBiasRight = _collisionTangentialBias * Random.Range(0.8f, 1.2f);
                ballPair.velocityLeft += tangentVector * tangentBiasLeft;
                ballPair.velocityRight -= tangentVector * tangentBiasRight;

                ballPair.velocityLeft += Vector2.down * _collisionDownwardBias;
                ballPair.velocityRight += Vector2.down * _collisionDownwardBias;
                
                ExecuteCollisionEffects(ballPair);
            }
        }

        private void ExecuteCollisionEffects(BallPair ballPair)
        {
            DOTween.Kill(ballPair.leftRT);
            DOTween.Kill(ballPair.rightRT);

            ballPair.leftRT.DOScale(_impactScale, _impactIn).SetEase(Ease.OutSine).OnComplete(() =>
                ballPair.leftRT.DOScale(Vector3.one, _impactOut).SetEase(Ease.OutSine)).SetUpdate(true);

            ballPair.rightRT.DOScale(_impactScale, _impactIn).SetEase(Ease.OutSine).OnComplete(() =>
                ballPair.rightRT.DOScale(Vector3.one, _impactOut).SetEase(Ease.OutSine)).SetUpdate(true);

            ballPair.leftRT.DOPunchAnchorPos(Vector2.left * _punchMagnitude, _punchDuration, 10, 1f).SetUpdate(true);
            ballPair.rightRT.DOPunchAnchorPos(Vector2.right * _punchMagnitude, _punchDuration, 10, 1f).SetUpdate(true);

            if (ballPair.leftCG == null) ballPair.leftCG = ballPair.leftRT.GetComponent<CanvasGroup>() ?? ballPair.leftRT.gameObject.AddComponent<CanvasGroup>();
            if (ballPair.rightCG == null) ballPair.rightCG = ballPair.rightRT.GetComponent<CanvasGroup>() ?? ballPair.rightRT.gameObject.AddComponent<CanvasGroup>();
            ballPair.leftCG.DOFade(0f, _fadeDuration).SetDelay(0.05f).SetUpdate(true);
            ballPair.rightCG.DOFade(0f, _fadeDuration).SetDelay(0.05f).SetUpdate(true);
        }

        private void OnRectTransformDimensionsChange()
        {
            if (gameObject.activeInHierarchy && enabled)
            {
                if (_boundsRect != null)
                {
                    CalculateBottomScreenBoundary();
                    ResetAnimationState();
                }
            }
        }
    }
}