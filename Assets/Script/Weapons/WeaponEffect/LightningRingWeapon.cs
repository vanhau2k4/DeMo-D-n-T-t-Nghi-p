using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningRingWeapon : ProjectileWeapon

{
    List<EnemyState> allSelectedEnemies = new List<EnemyState>();

    protected override bool Attack(int attackCount = 1)
    {
        if (!currentStats.hitEffect)
        {
            Debug.LogWarning(string.Format("Hit effect"));
            ActivateCooldown();
            return false;
        }
        if(!CanAttack()) return false;

        if(currentCooldown <= 0)
        {
            allSelectedEnemies = new List<EnemyState>(FindObjectsOfType<EnemyState>());
            ActivateCooldown();
            currentAttackCount = attackCount;
        }
        EnemyState target = PickEnemy();
        if (target)
        {
            DamageArea(target.transform.position,GetArea(),GetDamage());

            Instantiate(currentStats.hitEffect, target.transform.position, Quaternion.identity);
        }
        if(attackCount > 0)
        {
            currentAttackCount = attackCount-1;
            currentAttackInterval = currentStats.projectileInterval;
        }
        return true;
    }
    EnemyState PickEnemy()
    {
        EnemyState target = null;
        while(!target && allSelectedEnemies.Count > 0)
        {
            int idx = Random.Range(0, allSelectedEnemies.Count);
            target = allSelectedEnemies[idx];

            if (!target)
            {
                allSelectedEnemies.RemoveAt(idx);
                continue;
            }

            Renderer r = target.GetComponent<Renderer>();
            if(!r || !r.isVisible)
            {
                allSelectedEnemies.Remove(target);
                target = null;
                continue;
            }
        }
        allSelectedEnemies.Remove(target);
        return target;
    }
    private void DamageArea(Vector2 position, float radius, float damage)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(position, radius);
        foreach(Collider2D t in targets)
        {
            EnemyState es = t.GetComponent<EnemyState>();
            if (es)
            {
                es.TakeDamage(damage, transform.position);
                ApplyBuffs(es);
            }
        }
    }
}
