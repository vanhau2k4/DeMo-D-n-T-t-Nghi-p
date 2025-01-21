using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject gameobjecPlayer;
    public float checkerRadius;
    public LayerMask terrainMask;
    public GameObject currentChunk;
    private Vector3 playLastPosition;

    [Header("Opitimization")]
    public List<GameObject> spawnedChunks;
    GameObject latestChumk;
    public float maxOpDist;
    private float opDist;
    private float optimizerColldown;
    public float optimzerCooldownDur;
    void Start()
    {
        playLastPosition = gameobjecPlayer.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        ChunkCherker();
        ChunkOptimizer();
    }
    private void ChunkCherker()
    {
        if (!currentChunk) return;
        Vector3 moveDir = gameobjecPlayer.transform.position - playLastPosition;
        playLastPosition = gameobjecPlayer.transform.position;
        
        string dir = GetDirectionName(moveDir);

        if (dir.Contains("up"))
        {
            CheckAndSpawnChunk("up");
        }
        if (dir.Contains("down"))
        {
            CheckAndSpawnChunk("down");
        }
        if (dir.Contains("right"))
        {
            CheckAndSpawnChunk("right");
        }
        if (dir.Contains("left"))
        {
            CheckAndSpawnChunk("left");
        }
    }
    private void CheckAndSpawnChunk(string dir)
    {
        if(!Physics2D.OverlapCircle(currentChunk.transform.Find(dir).position, checkerRadius, terrainMask))
        {
            SpawnChunk(currentChunk.transform.Find(dir).position);
        }
    }
    private string GetDirectionName(Vector3 dir)
    {
        dir = dir.normalized;
        if(Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if(dir.y > 0.5f)
            {
                return dir.x > 0 ? "right up" : "left up";
            }
            else if(dir.y < -0.5f)
            {
                return dir.x > 0 ? "right down" : "left down";
            }
            else
            {
                return dir.x > 0 ? "right" : "left";
            }
        }
        else
        {
            if (dir.x > 0.5f)
            {
                return dir.y > 0 ? "right up" : "right down";
            }
            else if (dir.x < -0.5f)
            {
                return dir.y > 0 ? "left up" : "left down";
            }
            else
            {
                return dir.y > 0 ? "up" : "up";
            }
        }
    }
    private void SpawnChunk(Vector3 spawnPosition)
    {
        int rand = Random.Range(0, terrainChunks.Count);
        latestChumk = Instantiate(terrainChunks[rand], spawnPosition, Quaternion.identity);
        spawnedChunks.Add(latestChumk);
    }
    private void ChunkOptimizer()
    {
        optimizerColldown -= Time.deltaTime;
        if (optimizerColldown < 0)
        {
            optimizerColldown = optimzerCooldownDur;
        }
        else
        {
            return;
        }

        foreach(GameObject chunk in spawnedChunks)
        {
            opDist = Vector3.Distance(gameobjecPlayer.transform.position, chunk.transform.position);
            if(opDist > maxOpDist)
                chunk.SetActive(false);
            else 
                chunk.SetActive(true);
        }
    }
}
