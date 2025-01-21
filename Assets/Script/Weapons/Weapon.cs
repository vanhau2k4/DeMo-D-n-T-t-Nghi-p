using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Item
{
    [System.Serializable]
    public class Stats : LevelData
    {

        [Header("Visuals")]
        public Projectile projectilePrefab;
        public Aura auraPrefab;
        public ParticleSystem hitEffect;
        public Rect spawnVariace;

        [Header("Values")]
        public float lifespan;
        public float damage, damageVariance, area, speed, cooldown, projectileInterval, knocback;
        public int number, piercing, maxInstances;

        public EntityStats.BuffInfo[] appliedBuffs;

        public static Stats operator +(Stats s1 ,Stats s2)
        {
            Stats result = new Stats();
            result.name = s2.name ?? s1.name;
            result.description = s2.description ?? s1.description;
            result.projectilePrefab = s2.projectilePrefab == null? s1.projectilePrefab : s2.projectilePrefab;
            result.auraPrefab = s2.auraPrefab ?? s1.auraPrefab;
            result.hitEffect = s2.hitEffect == null ? s1.hitEffect : s2.hitEffect;
            result.spawnVariace = s2.spawnVariace;
            result.lifespan = s1.lifespan + s2.lifespan;
            result.damage = s1.damage + s2.damage;
            result.damageVariance = s1.damageVariance + s2.damageVariance;
            result.area = s1.area + s2.area;
            result.speed = s1.speed + s2.speed;
            result.cooldown = s1.cooldown + s2.cooldown;
            result.number = s1.number + s2.number;
            result.piercing = s1.piercing + s2.piercing;
            result.projectileInterval = s1.projectileInterval + s2.projectileInterval;
            result.knocback = s1.knocback + s2.knocback;
            result.appliedBuffs = s2.appliedBuffs == null || s2.appliedBuffs.Length <= 0 ?  s1.appliedBuffs : s2.appliedBuffs;
            return result;
        }
        public float GetDamage()
        {
            return damage + Random.Range(0, damageVariance);
        }
    }

    protected Stats currentStats;
    protected float currentCooldown;
    protected PlayerMovement playerMovement;

    public virtual void Initialise(WeaponData data)
    {
        base.Initialise(data);
        this.data = data;
        currentStats = data.baseStats;
        playerMovement = GetComponentInParent<PlayerMovement>();
        ActivateCooldown();
    }

    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if(currentCooldown <= 0f)
        {
            Attack(currentStats.number + owner.Stats.amount);
        }
    }
    public override bool DoLevelUp()
    {
        base.DoLevelUp();

        if(!CanLevelUp())
        {
            return false;
        }

        currentStats += (Stats)data.GetLevelData(++currentLevel);
        return true;
    }
    public virtual bool CanAttack()
    {
        return currentCooldown <= 0f;
    }
    protected virtual bool Attack(int attackCount = 1)
    {
        if (CanAttack())
        {
            ActivateCooldown();
            return true;
        }
        return false;
    }
    public virtual float GetDamage()
    {
        return currentStats.GetDamage() * owner.Stats.might;
    }
    public virtual float GetArea()
    {
        return currentStats.area + owner.Stats.area;
    }
    public virtual Stats GetStats() { return currentStats; }

    public virtual bool ActivateCooldown(bool strict = false)
    {
        if (strict && currentCooldown > 0) return false;

        float actualCooldown = currentStats.cooldown * Owner.Stats.cooldown;

        currentCooldown = Mathf.Min(actualCooldown, currentCooldown + actualCooldown);
        return true;
    }
    public void ApplyBuffs(EntityStats e)
    {
        foreach (EntityStats.BuffInfo b in GetStats().appliedBuffs)
        {
            e.ApplyBuff(b, owner.Actual.duration);
        }
    }
}
