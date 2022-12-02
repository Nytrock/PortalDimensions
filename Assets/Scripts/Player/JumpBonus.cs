using UnityEngine;

public class JumpBonus : MonoBehaviour
{
    private Animator animator;
    public bool tripleJump;
    private readonly bool[] stable = { true, true, true, false };

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Player player))
            player.AddToJumpBonusesLists(this, false);
    }

    private void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Player player) && !animator.GetBool("Death"))
            player.AddToJumpBonusesLists(this, true);
    }

    public void SetStable()
    {
        bool choose = stable[Random.Range(0, stable.Length)];
        animator.SetBool("Stable", choose);
    }

    public void Death()
    {
        Destroy(gameObject);
    }
}
