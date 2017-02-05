using UnityEngine;

namespace Assets.Engine.Spawners
{
    public interface IRadiusSpawner : ISpawner
    {
        float Radius { get; set; }

        float MaxSpawnsCount { get; set; }

        bool IsValidSpawnPosition(Vector3 position);
    }
}