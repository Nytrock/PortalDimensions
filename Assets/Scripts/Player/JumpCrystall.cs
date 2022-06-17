using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCrystall : MonoBehaviour
{
    public bool BoostJump;
    public Animator animator;
    void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Player player))
        {
            if (!player.CrystallJump)
            {
                player.CrystallJump = true;
                player.BoostCrystallJump = BoostJump;
                player.jumpCrystall = this;
            }
        }
    }
    void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Player player))
        {
            player.CrystallJump = false;
            player.BoostCrystallJump = false;
            player.jumpCrystall = null;
        }
    }

    public void Active(bool act)
    {
        animator.SetBool("Active", act);
    }

    public void ActiveAnim()
    {
        animator.SetBool("Active", false);
    }
}
