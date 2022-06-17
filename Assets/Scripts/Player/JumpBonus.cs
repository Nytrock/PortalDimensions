using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBonus : MonoBehaviour
{
    public bool TripleJump;
    private List<bool> Stable = new List<bool> { true, true, true, false };
    void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Player player))
        {
            if (TripleJump && !player.TripleJump)
            {
                player.TripleJump = true;
                player.DoubleJump = false;
                GetComponent<Animator>().SetBool("Death", true);
            }
            else if (!player.DoubleJump && !player.TripleJump)
            {
                player.DoubleJump = true;
                GetComponent<Animator>().SetBool("Death", true);
            }
            player.Animations.UpdateJumpBonus();
        }
    }

    void SetStable()
    {
        bool choose = Stable[Random.Range(0, Stable.Count)];
        GetComponent<Animator>().SetBool("Stable", choose);
    }

    void Death()
    {
        Destroy(gameObject);
    }
}
