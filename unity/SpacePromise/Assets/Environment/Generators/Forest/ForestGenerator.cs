using System.Linq;
using Assets.Engine.Spawners;
using UnityEngine;

namespace Assets.Environment.Generators.Forest
{
    public class ForestGenerator : RadiusSpawner
    {
        private string TreePrefabName = "TreeOakMed01Prefab";
        private string BushPrefabName = "BushEmpty01Prefab";
        private Object treePrefab;
        private Object bushPrefab;
    
        public float TreeTopRadius = 5;
        public float BushRadius = 1;


        public ForestGenerator()
        {
            this.Radius = 100;
            this.MaxSpawnsCount = 200;
        }


        protected override GameObject SpawnInstance(Vector3 position)
        {
            if (Random.value < 0.3)
            {
                var bushInstance = (GameObject)Instantiate(this.bushPrefab, this.transform);
                bushInstance.transform.SetPositionAndRotation(
                    position,
                    Quaternion.AngleAxis(
                        Random.value * 360,
                        Vector3.up));
                return bushInstance;
            }
            else
            {
                var treeInstance = (GameObject)Instantiate(this.treePrefab, this.transform);
                treeInstance.transform.SetPositionAndRotation(
                    position,
                    Quaternion.AngleAxis(
                        Random.value * 360,
                        Vector3.up));
                return treeInstance;
            }
        }

        public override bool IsValidSpawnPosition(Vector3 position)
        {
            // Check if there is tree already on that position (in radius of tree top)
            // If there is, don't spawn new tree
            if (this.Spawns.Where(spawn => spawn.name.StartsWith(TreePrefabName)).Select(spawn => spawn.transform.localPosition).Any(existingTree =>
                Vector3.Distance(existingTree, position) < TreeTopRadius) ||
                this.Spawns.Where(spawn => spawn.name.StartsWith(BushPrefabName)).Select(spawn => spawn.transform.localPosition).Any(existingBush =>
                Vector3.Distance(existingBush, position) < BushRadius))
                return false;
            return true;
        }

        public void Start ()
        {
            this.treePrefab = Resources.Load(TreePrefabName);
            this.bushPrefab = Resources.Load(BushPrefabName);
            this.DoSpawn();
        }
    }
}