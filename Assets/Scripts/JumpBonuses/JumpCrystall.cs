using UnityEngine;

public class JumpCrystall : MonoBehaviour
{
    public bool boostJump;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Player player))
            player.CheckCrystallList(this, false);
    }

    private void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Player player))
            player.CheckCrystallList(this, true);
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
