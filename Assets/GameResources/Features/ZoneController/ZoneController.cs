namespace GameResources.Features.ZoneController
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Entities.Core;
    using UnityEngine;

    public sealed class ZoneController : MonoBehaviour
    {
         public event Action<ZoneEntity, List<BaseEntity>> onZoneUpdate;
         public event Action onEmptyZonesAreOver;
         
         [SerializeField] private List<ZoneEntity> _zoneEntities = default;
         
         private Dictionary<ZoneEntity, List<BaseEntity>> _matchedLines = new Dictionary<ZoneEntity, List<BaseEntity>>();
         private Dictionary<ZoneEntity, List<BaseEntity>> _ballsToRemoveByZone = new Dictionary<ZoneEntity, List<BaseEntity>>();
         
         private void OnDestroy()
         {
             for (int i = 0; i < _zoneEntities.Count; i++)
             {
                 _zoneEntities[i].onZoneUpdated -= CheckMatches;
             }
         }
         
         public void InitializeController()
         {
             for (int i = 0; i < _zoneEntities.Count; i++)
             {
                 _zoneEntities[i].Initialize(this);
                 _zoneEntities[i].onZoneUpdated += CheckMatches;
             }
         }

         private void CheckMatches()
         {
             _ballsToRemoveByZone.Clear();
             _matchedLines.Clear();

             foreach (ZoneEntity zoneEntity in _zoneEntities)
             {
                 _ballsToRemoveByZone[zoneEntity] = new List<BaseEntity>();
                 
                 List<(List<BaseEntity> balls, EntityType color)> verticalMatches = GetVerticalMatches(zoneEntity);
                 _ballsToRemoveByZone[zoneEntity].AddRange(verticalMatches.SelectMany(m => m.balls));
             }

             int maxHeight = _zoneEntities.Max(z => z.GetBalls().Count);
             for (int height = 0; height < maxHeight; height++)
             {
                 List<(List<BaseEntity> balls, EntityType color)> horizontalMatches = GetHorizontalMatches(height);
                 foreach ((List<BaseEntity> balls, _) in horizontalMatches)
                 {
                     foreach (BaseEntity ball in balls)
                     {
                         ZoneEntity zone = _zoneEntities.First(z => z.GetBalls().Contains(ball));
                         _ballsToRemoveByZone[zone].Add(ball);
                     }
                 }
             }

             List<(List<BaseEntity> balls, EntityType color)> diagonalMatches = GetDiagonalMatches();
             foreach ((List<BaseEntity> balls, _) in diagonalMatches)
             {
                 foreach (BaseEntity ball in balls)
                 {
                     ZoneEntity zone = _zoneEntities.First(z => z.GetBalls().Contains(ball));
                     _ballsToRemoveByZone[zone].Add(ball);
                 }
             }

             _matchedLines = _ballsToRemoveByZone.Where(kv => kv.Value.Count > 0).ToDictionary(kv => kv.Key, kv => kv.Value);

             if (_matchedLines.Count > 0)
             {
                 foreach (KeyValuePair<ZoneEntity, List<BaseEntity>> zone in _matchedLines)
                 {
                     onZoneUpdate?.Invoke(zone.Key, zone.Value);
                 }
             }
             else
             {
                 for (int i = 0; i < _zoneEntities.Count; i++)
                 {
                     if (!_zoneEntities[i].IsFull)
                     {
                         return;
                     }
                 }

                 onEmptyZonesAreOver?.Invoke();
             }
         }

         private List<(List<BaseEntity> balls, EntityType color)> GetVerticalMatches(ZoneEntity zone)
         {
             List<(List<BaseEntity> balls, EntityType color)> matches = new List<(List<BaseEntity> balls, EntityType color)>();
             IReadOnlyList<BaseEntity> balls = zone.GetBalls();
             if (balls.Count < 3) return matches;

             for (int i = 0; i <= balls.Count - 3; i++)
             {
                 if (balls[i].Type == balls[i + 1].Type && balls[i].Type == balls[i + 2].Type)
                 {
                     matches.Add((new List<BaseEntity> { balls[i], balls[i + 1], balls[i + 2] }, balls[i].Type));
                 }
             }
             return matches;
         }

         private List<(List<BaseEntity> balls, EntityType color)> GetHorizontalMatches(int height)
         {
             List<(List<BaseEntity> balls, EntityType color)> matches = new List<(List<BaseEntity> balls, EntityType color)>();
             List<(BaseEntity ball, int zoneIndex)> ballsAtHeight = new List<(BaseEntity ball, int zoneIndex)>();

             for (int zoneIndex = 0; zoneIndex < _zoneEntities.Count; zoneIndex++)
             {
                 IReadOnlyList<BaseEntity> balls = _zoneEntities[zoneIndex].GetBalls();
                 if (balls.Count > height)
                 {
                     ballsAtHeight.Add((balls[height], zoneIndex));
                 }
             }

             if (ballsAtHeight.Count < 3) return matches;

             for (int i = 0; i <= ballsAtHeight.Count - 3; i++)
             {
                 if (ballsAtHeight[i].ball.Type == ballsAtHeight[i + 1].ball.Type && 
                     ballsAtHeight[i].ball.Type == ballsAtHeight[i + 2].ball.Type &&
                     ballsAtHeight[i + 1].zoneIndex == ballsAtHeight[i].zoneIndex + 1 &&
                     ballsAtHeight[i + 2].zoneIndex == ballsAtHeight[i].zoneIndex + 2) // Проверка непрерывности
                 {
                     matches.Add((new List<BaseEntity> { ballsAtHeight[i].ball, ballsAtHeight[i + 1].ball, ballsAtHeight[i + 2].ball }, ballsAtHeight[i].ball.Type));
                 }
             }
             return matches;
         }

         private List<(List<BaseEntity> balls, EntityType color)> GetDiagonalMatches()
         {
             List<(List<BaseEntity> balls, EntityType color)> matches = new List<(List<BaseEntity> balls, EntityType color)>();
             int zoneCount = _zoneEntities.Count;
             int maxHeight = _zoneEntities.Max(z => z.GetBalls().Count);

             // Проверка диагоналей слева-направо вниз
             for (int startZone = 0; startZone <= zoneCount - 3; startZone++)
             {
                 for (int startHeight = 0; startHeight <= maxHeight - 3; startHeight++)
                 {
                     IReadOnlyList<BaseEntity> zone1 = _zoneEntities[startZone].GetBalls();
                     IReadOnlyList<BaseEntity> zone2 = _zoneEntities[startZone + 1].GetBalls();
                     IReadOnlyList<BaseEntity> zone3 = _zoneEntities[startZone + 2].GetBalls();

                     if (startHeight < zone1.Count && 
                         startHeight + 1 < zone2.Count && 
                         startHeight + 2 < zone3.Count)
                     {
                         BaseEntity ball1 = zone1[startHeight];
                         BaseEntity ball2 = zone2[startHeight + 1];
                         BaseEntity ball3 = zone3[startHeight + 2];

                         if (ball1.Type == ball2.Type && ball2.Type == ball3.Type)
                         {
                             matches.Add((new List<BaseEntity> { ball1, ball2, ball3 }, ball1.Type));
                         }
                     }
                 }
             }

             // Проверка диагоналей слева-направо вверх
             for (int startZone = 0; startZone <= zoneCount - 3; startZone++)
             {
                 for (int startHeight = 2; startHeight < maxHeight; startHeight++)
                 {
                     IReadOnlyList<BaseEntity> zone1 = _zoneEntities[startZone].GetBalls();
                     IReadOnlyList<BaseEntity> zone2 = _zoneEntities[startZone + 1].GetBalls();
                     IReadOnlyList<BaseEntity> zone3 = _zoneEntities[startZone + 2].GetBalls();

                     if (startHeight < zone1.Count && 
                         startHeight - 1 < zone2.Count && 
                         startHeight - 2 < zone3.Count)
                     {
                         BaseEntity ball1 = zone1[startHeight];
                         BaseEntity ball2 = zone2[startHeight - 1];
                         BaseEntity ball3 = zone3[startHeight - 2];

                         if (ball1.Type == ball2.Type && ball2.Type == ball3.Type)
                         {
                             matches.Add((new List<BaseEntity> { ball1, ball2, ball3 }, ball1.Type));
                         }
                     }
                 }
             }

             return matches;
         }
    }
}