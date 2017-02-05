namespace Assets.Engine.Spawners
{
    public interface ISpawner
    {
#if UNITY_EDITOR
        bool Clear { get; set; }
        bool Respawn { get; set; }
        int SpawnsCount { get; }
#endif

        void DoClearSpawns();
        void DoRespawn();
        void DoSpawn();
    }
}