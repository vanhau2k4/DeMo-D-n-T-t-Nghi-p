using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeWeapon : ProjectileWeapon
{
    protected override float GetSpawnAngle()
    {
        int offset = currentAttackCount > 0 ? currentStats.number - currentAttackCount : 0;
        return 90f - Mathf.Sign(playerMovement.LastMovedVector.x) * (5  * offset);
    }
    protected override Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        return  new Vector2(
            Random.Range(currentStats.spawnVariace.xMin, currentStats.spawnVariace.xMax),
            Random.Range(currentStats.spawnVariace.yMin, currentStats.spawnVariace.yMax));
    }
}
