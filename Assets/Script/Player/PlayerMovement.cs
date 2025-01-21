using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMovement : Sortable
{
    public const float DEFAULT_MOVESPEED = 7f;
    public Vector2 moveDir;
    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;
    [HideInInspector]
    public Vector2 LastMovedVector;

    Animator anim;
    Rigidbody2D rb;
    PlayerStats stats;

    private bool movIng;
    float x;
    float y;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        stats = GetComponent<PlayerStats>();
        LastMovedVector = new Vector2 (-1f, 0);
    }

    // Update is called once per frame
    private void Update()
    {
        InputManager();
        Idle();
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void InputManager()
    {
        if (GameManager.instance.isGameOver)
        {
            return;
        }
        
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        
        moveDir = new Vector2 (x, y).normalized;

        if(moveDir.x != 0)
        {
            lastHorizontalVector = moveDir.x;
            LastMovedVector = new Vector2(lastHorizontalVector, 0f);
        }
        if (moveDir.y != 0)
        {
            lastVerticalVector = moveDir.y;
            LastMovedVector = new Vector2(0f, lastVerticalVector);
        }
        if (moveDir.x != 0 && moveDir.y != 0)
        {
            LastMovedVector = new Vector2(lastHorizontalVector, lastVerticalVector);
        }
    }
    private void Move()
    {
        if (GameManager.instance.isGameOver)
        {
            return;
        }

        rb.velocity = moveDir * DEFAULT_MOVESPEED * stats.Stats.moveSpeed;
    }
    private void Idle()
    {
        if (GameManager.instance.isGameOver)
        {
            return;
        }

        if (moveDir.magnitude > 0.1f || moveDir.magnitude < -0.1f)
        {
            movIng = true;
        }
        else
        {
            movIng = false;
        }
        if (movIng)
        {
            anim.SetFloat("x", x);
            anim.SetFloat("y", y);
        }
        anim.SetBool("Moving",movIng);
    }

}
