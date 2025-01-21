using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enity : MonoBehaviour
{
    public Animator anim {  get; private set; }
    public Rigidbody2D rb {  get; private set; }
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }
}
