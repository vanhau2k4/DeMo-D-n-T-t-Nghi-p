using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingEnemyMovement : EnemyMovement
{
    Vector2 chargeDirection;

    protected override void Start()
    {
        base.Start();
        chargeDirection = (player.transform.position - transform.position).normalized;
    }
    public override void Move()
    {
        transform.position += (Vector3)chargeDirection * stats.Actual.moveSpeed * Time.deltaTime;
    }
}
