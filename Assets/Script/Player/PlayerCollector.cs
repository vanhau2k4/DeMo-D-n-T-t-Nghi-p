using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CircleCollider2D))]
public class PlayerCollector : MonoBehaviour
{
    private PlayerStats player;
    private CircleCollider2D deletor;
    public float pullspeed;
    private void Start()
    {
        player = GetComponentInParent<PlayerStats>();
    }
    public void SetRadius(float r)
    {
        if(!deletor) deletor = GetComponent<CircleCollider2D>();
        deletor.radius = r;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Pickup p))
        {
            p.Collect(player,pullspeed);
        }
    }
}
