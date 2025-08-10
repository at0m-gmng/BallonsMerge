using System;
using System.Collections.Generic;
using GameResources.Features.Entities.Core;
using UnityEngine;

namespace GameResources.Features.PersistentProgress
{
    [CreateAssetMenu(fileName = "ScoreConfig", menuName = "Configs/ScoreConfig")]
    public sealed class ScoreConfig : ScriptableObject
    {
        [field: SerializeField]
        public List<BallScores> ScoresData { get; private set; } = new List<BallScores>();

        public int GetScore(EntityType targetType) => ScoresData.Find(x => x.BallType == targetType).Score;
    }
    [Serializable]
    public sealed class BallScores
    {
        [field: SerializeField]
        public EntityType BallType { get; private set; } = EntityType.Red;

        [field: SerializeField]
        public int Score { get; private set; } = 0;
    }
}