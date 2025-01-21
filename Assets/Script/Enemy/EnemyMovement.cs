using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class EnemyMovement : Sortable
{
    protected EnemyState stats;
    protected Transform player;
    protected Rigidbody2D rb;

    protected Vector2 knockbackVelocity;
    protected float knockbackDuration;

    public enum OutOffFrameAction { none,resSpawnAtEdge,despawn}
    public OutOffFrameAction outOffFrameAction = OutOffFrameAction.resSpawnAtEdge;

    [System.Flags]
    public enum KnockbackVariance { duration = 1, velocity = 2 }
    public KnockbackVariance knockbackVariance = KnockbackVariance.velocity;
    
    protected bool spawnedOutOffFarme = false;
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        spawnedOutOffFarme = !SpawnManager.IsWithinBoundaries(transform);
        stats = GetComponent<EnemyState>();

        PlayerMovement[] allPlayers = FindObjectsOfType<PlayerMovement>();
        player = allPlayers[Random.Range(0,allPlayers.Length)].transform;
    }

    protected virtual void Update()
    {
        if(knockbackDuration > 0)
        {
            transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
            knockbackDuration -= Time.deltaTime;
        }
        else
        {
            Move();
            HandleOutOffFrameAction();
        }
    }

    protected virtual void HandleOutOffFrameAction()
    {
        if (!SpawnManager.IsWithinBoundaries(transform))
        {
            switch(outOffFrameAction)
            {
                case OutOffFrameAction.none: default:
                    break;
                case OutOffFrameAction.resSpawnAtEdge:
                    transform.position = SpawnManager.GeneratePosition();
                    break;
                case OutOffFrameAction.despawn:
                    if (!spawnedOutOffFarme)
                    {
                        Destroy(gameObject);
                    }
                    break;
            }
        }else spawnedOutOffFarme=false;
    }
    public virtual void Knockback(Vector2 velocity,float duration)
    {
        if(knockbackDuration > 0)
        {
            return;
        }
        if (knockbackVariance == 0) return;

        float pow = 1;
        bool reducesVelocity = (knockbackVariance & KnockbackVariance.velocity) > 0,
            reducesDuration = (knockbackVariance & KnockbackVariance.duration) > 0;

        if (reducesVelocity && reducesDuration) pow = 0.5f;

        knockbackVelocity = velocity * (reducesVelocity ? Mathf.Pow(stats.Actual.knokbackMultiplier, pow) : 1);
        knockbackDuration = duration * (reducesDuration ? Mathf.Pow(stats.Actual.knokbackMultiplier, pow) : 1);
    }
    public virtual void Move()
    {
        if (rb)
        {
            rb.MovePosition(Vector2.MoveTowards(
                rb.position, player.transform.position, stats.Actual.moveSpeed * Time.deltaTime));
        }
        else
        {
            transform.position = Vector2.MoveTowards(
                transform.position,player.transform.position,stats.Actual.moveSpeed * Time.deltaTime);
        }
    }
}
