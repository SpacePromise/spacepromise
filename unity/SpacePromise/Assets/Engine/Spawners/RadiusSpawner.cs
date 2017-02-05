using Assets.Engine.Unity;
using UnityEngine;

namespace Assets.Engine.Spawners
{
    public abstract class RadiusSpawner : Spawner, IRadiusSpawner
    {
        public virtual bool IsValidSpawnPosition(Vector3 position)
        {
            return true;
        }

        public override void DoSpawn()
        {
            for (int index = 0; index < this.MaxSpawnsCount; index++)
            {
                // Get new random tree position (relative to the spawner)
                var randomInRadius = Random.insideUnitCircle * this.Radius;
                var relativeInRadius = new Vector3(
                    this.transform.localPosition.x + randomInRadius.x,
                    this.transform.localPosition.y,
                    this.transform.localPosition.z + randomInRadius.y);

                // Validate new position
                if (!this.IsValidSpawnPosition(relativeInRadius))
                    continue;

                // Instantiate new spawn object
                this.Spawns.Add(this.SpawnInstance(relativeInRadius));
            }
        }

        protected abstract GameObject SpawnInstance(Vector3 position);

        [ExposeProperty]
        public float Radius { get; set; }

        [ExposeProperty]
        public float MaxSpawnsCount { get; set; }
    }
}