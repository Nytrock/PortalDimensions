using UnityEngine;

public class ButtonSwitch : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string animationType;

    public void Normal()
    {
        ResetAllAnimations();
        animator.SetBool("Normal" + animationType, true);
    }

    public void Highlighted()
    {
        ResetAllAnimations();
        animator.SetBool("Highlighted" + animationType, true);
    }

    public void Pressed()
    {
        ResetAllAnimations();
        animator.SetBool("Pressed" + animationType, true);
    }
    public void Disabled()
    {
        ResetAllAnimations();
        animator.SetBool("Disabled" + animationType, true);
    }

    private void ResetAllAnimations()
    {
        animator.SetBool("Normal" + animationType, false);
        animator.SetBool("Highlighted" + animationType, false);
        animator.SetBool("Pressed" + animationType, false);
        animator.SetBool("Disabled" + animationType, false);
    }

    public void SetAnimator(Animator newAnimator)
    {
        animator = newAnimator;
    }
}
