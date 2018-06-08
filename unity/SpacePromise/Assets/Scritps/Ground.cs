using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class GroundSharedRenderData
{
    public bool LockLODViewerPosition;

    public Vector3 ViewerPosition;
    public Vector3 LodViewerPosition;

    public int ChunkSize = 600;
    public int SurfacesPerChunk = 3;


    public void SetViewerPosition(Vector3 position)
    {
        this.ViewerPosition = position;
        if (!this.LockLODViewerPosition)
            this.LodViewerPosition = this.ViewerPosition;
    }
}

public class Ground : MonoBehaviour
{
    public GroundSharedRenderData SharedRenderData = new GroundSharedRenderData(); 

    public int chunkSpawnRadius = 4*600;
    
    private readonly Dictionary<Vector2, GroundChunk> groundChunks = new Dictionary<Vector2, GroundChunk>();
    private readonly Dictionary<Vector2, GroundChunk> inactiveChunks = new Dictionary<Vector2, GroundChunk>();

    private void Start()
    {
        this.DestroyChildrenWithComponent<WaterSurface>();
        this.DestroyChildrenWithComponent<GroundChunk>();

        this.CreateChildGameObjectFromComponent<WaterSurface>();
    }

    private void Update ()
    {
        var newViewerPosition = Camera.main.transform.position;
        if (newViewerPosition == this.SharedRenderData.ViewerPosition) 
            return;

        this.SharedRenderData.SetViewerPosition(newViewerPosition);
        this.UpdateChunksInView();
    }

    private void UpdateChunksInView()
    {
        int currentChunkPosX = Mathf.RoundToInt(this.SharedRenderData.ViewerPosition.x / this.SharedRenderData.ChunkSize);
        int currentChunkPosY = Mathf.RoundToInt(this.SharedRenderData.ViewerPosition.z / this.SharedRenderData.ChunkSize);
        int chunksRadius = this.chunkSpawnRadius / this.SharedRenderData.ChunkSize;

        // NOTE: Can be optimized by using normalized collection 
        //       that is multiplied on request (avoid doing this every frame)
        var lastChunkPositions = this.groundChunks.Keys.ToList();
        var newChunkPositions = Enumerable
            .Range(currentChunkPosX - chunksRadius, chunksRadius * 2)
            .SelectMany(x => Enumerable
                .Range(currentChunkPosY - chunksRadius, chunksRadius * 2)
                .Select(y => new Vector2(x, y)))
            .ToList();

        var toSpawn = newChunkPositions.Except(lastChunkPositions);
        var toDespawn = lastChunkPositions.Except(newChunkPositions);

        foreach (var chunkPosToSpawn in toSpawn)
            this.SpawnChunk(chunkPosToSpawn);

        foreach (var chunkPosToDespawn in toDespawn)
            this.DespawnChunk(chunkPosToDespawn);
    }

    private void DespawnChunk(Vector2 position)
    {
        if (!this.groundChunks.ContainsKey(position))
            return;

        // Retrieve chunk and disable
        var chunk = this.groundChunks[position];
        chunk.gameObject.SetActive(false);

        // Move chunk to inactive chunks collection
        this.groundChunks.Remove(position);
        this.inactiveChunks.Add(position, chunk);
    }

    private void SpawnChunk(Vector2 position)
    {
        if (this.inactiveChunks.ContainsKey(position))
            this.RespawnChunk(position);
        else this.SpawnNewChunk(position);
    }

    private void RespawnChunk(Vector2 position)
    {
        var chunk = this.inactiveChunks[position];

        chunk.gameObject.SetActive(true);

        this.inactiveChunks.Remove(position);
        this.groundChunks.Add(position, chunk);
    }

    private void SpawnNewChunk(Vector2 position)
    {
        var chunkObject = this.CreateChildGameObjectFromComponent<GroundChunk>();
        chunkObject.transform.localPosition = new Vector3(
            position.x * this.SharedRenderData.ChunkSize, 
            0, 
            position.y * this.SharedRenderData.ChunkSize);

        var chunk = chunkObject.GetComponent<GroundChunk>();
        chunk.SharedRenderData = this.SharedRenderData;

        this.groundChunks.Add(position, chunk);
    }
}
