using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aura : WeaponEffect
{
    Dictionary<EnemyState,float> affectedTargets = new Dictionary<EnemyState, float>();
    List<EnemyState> targetsToUnaffect = new List<EnemyState>();

    private void Update()
    {
        Dictionary<EnemyState, float> affectedTargsCopy = new Dictionary<EnemyState, float>(affectedTargets);

        foreach(KeyValuePair<EnemyState,float> pair in affectedTargsCopy)
        {
            affectedTargets[pair.Key] -= Time.deltaTime;
            if(pair.Value <= 0 )
            {
                if(targetsToUnaffect.Contains(pair.Key))
                {
                    affectedTargets.Remove(pair.Key);
                    targetsToUnaffect.Remove(pair.Key);
                }
                else
                {
                    Weapon.Stats stats = weapon.GetStats();
                    affectedTargets[pair.Key] = stats.cooldown * owner.Stats.cooldown;
                    pair.Key.TakeDamage(GetDamage(), transform.position, stats.knocback);

                    weapon.ApplyBuffs(pair.Key);

                    if (stats.hitEffect)
                    {
                        Destroy(Instantiate(stats.hitEffect,pair.Key.transform.position,Quaternion.identity),5f);
                    }
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D othe)
    {
        
        if(othe.TryGetComponent(out EnemyState es))
        {
            if (!affectedTargets.ContainsKey(es))
            {
                affectedTargets.Add(es, 0);
            }
            else
            {
                if (targetsToUnaffect.Contains(es))
                {
                    targetsToUnaffect.Remove(es);
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D othe)
    {
        if (othe.TryGetComponent(out EnemyState es))
        {
            if (affectedTargets.ContainsKey(es))
            {
                targetsToUnaffect.Add(es);
            }
        }
    }
}
