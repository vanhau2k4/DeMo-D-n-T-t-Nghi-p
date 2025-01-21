using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : WeaponEffect
{
    public enum DamageSource { projectile, owner};
    public DamageSource damageSource = DamageSource.projectile;
    public bool hasAutoim = false;
    public Vector3 rotationSpeed = new Vector3(0,0, 0);

    protected Rigidbody2D rb;
    protected int piercing;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Weapon.Stats stats = weapon.GetStats();
        if(rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.angularVelocity = rotationSpeed.z;
            rb.velocity = transform.right * stats.speed * weapon.Owner.Stats.speed;
        }

        float area = weapon.GetArea();
        if (area <= 0) area = 1;
        transform.localScale = new Vector3(
            area * Mathf.Sign(transform.localScale.x),
            area * Mathf.Sign(transform.localScale.y),1);

        piercing = stats.piercing;
        if(stats.lifespan > 0) Destroy(gameObject,stats.lifespan);
        if (hasAutoim) AcquireAutoAimFacing();
    }
    public virtual void AcquireAutoAimFacing()
    {
        float aimAngle;
        EnemyState[] targets = FindObjectsOfType<EnemyState>();

        if(targets.Length > 0)
        {
            EnemyState selectedTarget = targets[Random.Range(0, targets.Length)];
            Vector2 difference = selectedTarget.transform.position - transform.position;
            aimAngle = Mathf.Atan2(difference.x, difference.y) * Mathf.Rad2Deg;
        }
        else
        {
            aimAngle = Random.Range(0f, 360f);
        }
        transform.rotation = Quaternion.Euler(0,0,aimAngle);
    }
    protected virtual void FixedUpdate()
    {
        if(rb.bodyType == RigidbodyType2D.Kinematic)
        {
            Weapon.Stats stats = weapon.GetStats();
            transform.position += transform.right * stats.speed * weapon.Owner.Stats.speed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position);
            transform.Rotate(rotationSpeed * Time.fixedDeltaTime);
        }
    }
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        EnemyState es = other.GetComponent<EnemyState>();
        BreakableProps p = other.GetComponent<BreakableProps>();

        if (es)
        {
            Vector3 source = damageSource == DamageSource.owner && owner? owner.transform.position : transform.position;

            es.TakeDamage(GetDamage(),source);

            Weapon.Stats stats = weapon.GetStats();
            weapon.ApplyBuffs(es);
            piercing--;
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }
        else if(p)
        {
            p.TakeDamage(GetDamage());
            piercing--;

            Weapon.Stats stats = weapon.GetStats();

            if(stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }
        if (piercing <= 0)
        {
            Destroy(gameObject);
        }
    }
}
