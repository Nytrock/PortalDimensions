using UnityEngine;

public class JumpCrystall : MonoBehaviour
{
    public bool BoostJump;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Player player))
            player.CheckCrystallList(this, true);
    }
    void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Player player))
            player.CheckCrystallList(this, false);
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
