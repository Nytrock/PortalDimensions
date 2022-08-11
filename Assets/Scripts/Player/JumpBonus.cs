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
            player.AddToJumpBonusesLists(this, false);
    }

    public void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Player player))
            player.AddToJumpBonusesLists(this, true);
    }

    public void SetStable()
    {
        bool choose = Stable[Random.Range(0, Stable.Count)];
        GetComponent<Animator>().SetBool("Stable", choose);
    }

    public void Death()
    {
        Destroy(gameObject);
    }
}
