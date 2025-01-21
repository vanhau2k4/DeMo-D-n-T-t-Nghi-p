using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : Sortable 
{
    public float lifespan = 0.5f;
    protected PlayerStats target;
    protected float speed;
    Vector2 initialPosition;
    float initialOffset;

    [System.Serializable]
    public struct BobbingAnimation
    {
        public float frequency;
        public Vector2 direction;
    }
    public BobbingAnimation bobbingAnimation = new BobbingAnimation
    {
        frequency = 2f,
        direction = new Vector2(0, 0.3f)
    };

    [Header("Bonues")]
    public int experience;
    public int health;

    protected override void Start()
    {
        base.Start();
        initialPosition = transform.position;
        initialOffset = Random.Range(0, bobbingAnimation.frequency);
    }

    protected virtual void Update()
    {
        if (target)
        {
            Vector2 distance = target.transform.position - transform.position;
            if (distance.sqrMagnitude > speed * speed * Time.deltaTime)
                transform.position += (Vector3)distance.normalized * speed * Time.deltaTime;
            else
                Destroy(gameObject);
        }
        else
        {
            transform.position = initialPosition + bobbingAnimation.direction * Mathf.Sin((Time.time + initialOffset) * bobbingAnimation.frequency);
        }
    }
    public virtual bool Collect(PlayerStats target,float speed, float lifespan = 0f)
    {
        if (!this.target)
        {
            this.target = target;
            this.speed = speed;
            if(lifespan > 0f) this.lifespan = lifespan;
            Destroy(gameObject, Mathf.Max(0.01f, this.lifespan));
            return true;
        }
        return false;
    }
    protected virtual void OnDestroy()
    {
        if(!target) return;
        if(experience != 0) target.IncreaseExperience(experience);
        if(health != 0) target.RestoreHealth(health);
    }
}
