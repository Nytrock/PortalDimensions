using UnityEngine;

public class JumpBonus : MonoBehaviour
{
    public bool tripleJump;
    private readonly bool[] stable = { true, true, true, false };

    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Player player))
            player.AddToJumpBonusesLists(this, false);
    }

    private void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Player player))
            player.AddToJumpBonusesLists(this, true);
    }

    public void SetStable()
    {
        bool choose = stable[Random.Range(0, stable.Length)];
        GetComponent<Animator>().SetBool("Stable", choose);
    }

    public void Death()
    {
        Destroy(gameObject);
    }
}
