using System.Collections.Generic;
using Assets.Engine.Unity;
using UnityEngine;

namespace Assets.Engine.Spawners
{
    public abstract class Spawner : MonoBehaviour, ISpawner
    {
        protected readonly List<GameObject> Spawns = new List<GameObject>();

#if UNITY_EDITOR

        [ExposeProperty(1)]
        public int SpawnsCount
        {
            get { return this.Spawns.Count; }
        }

        [ExposePropertyButton(1)]
        public bool Clear { get; set; }

        [ExposePropertyButton(1)]
        public bool Respawn { get; set; }

#endif

        public virtual void DoClearSpawns()
        {
            foreach (var spawn in Spawns)
                Destroy(spawn);
            this.Spawns.Clear();
        }

        public abstract void DoSpawn();

        public virtual void DoRespawn()
        {
            this.DoClearSpawns();
            this.DoSpawn();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (this.Clear)
                this.DoClearSpawns();
            if (this.Respawn)
            {
                this.DoClearSpawns();
                this.DoSpawn();
            }
            this.Clear = false;
            this.Respawn = false;
#endif
        }
    }
}