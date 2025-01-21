using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Ring Event Data", menuName = "2D Top-down Rogue-like/Event Data/Ring")]
public class RingEventData : EventData
{
    [Header("Mob Data")]
    public ParticleSystem spawnEffectPrefab;
    public Vector2 scale = new Vector2(1, 1);
    [Min(0)] public float spawnRadius = 10f, lifespan = 15f;

    public override bool Activate(PlayerStats player = null)
    {
        if (player)
        {
            GameObject[] spawns = GetSpawns();
        
            float angleOffset = 2 * Mathf.PI / Mathf.Max(1,spawns.Length);
            float currentAngle = 0;
            foreach(GameObject g in spawns)
            {
                Vector3 spawnPositon = player.transform.position + new Vector3(
                    spawnRadius + Mathf.Cos(currentAngle) * scale.x,
                    spawnRadius + Mathf.Sin(currentAngle) * scale.y);

                if (spawnEffectPrefab)
                {
                    Instantiate(spawnEffectPrefab,spawnPositon,Quaternion.identity);
                }
                GameObject s = Instantiate(g,spawnPositon,Quaternion.identity); 
                if(lifespan >0) Destroy(s,lifespan);

                currentAngle += angleOffset;
            }
        }
        return false;
    }
}
