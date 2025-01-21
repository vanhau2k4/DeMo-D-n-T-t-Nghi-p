using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator am;

    private void Start()
    {
        am = GetComponent<Animator>();
    }
    public void SetAnimatorController(RuntimeAnimatorController c)
    {
        if(!am) am = GetComponent<Animator>();

        am.runtimeAnimatorController = c;
    }
}
