using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    private Rigidbody2D rb;
    private bool jumping;
    private int jumpIteration = 0;
    private float jumpForce;

    private PlayerStateManager stateManager;

    public override void EnterState(PlayerStateManager player)
    {
        stateManager = player;
        stateManager.animations.SetAnimation("IsJump", true);
        stateManager.portalGun.enabled = true;
        rb = stateManager.GetRigidbody();
        StartJump();
    }

    public override void ExitState()
    {
        stateManager.animations.SetAnimation("IsJump", false);
    }

    public override void UpdateState()
    {
        if (ButtonFunctional.isGamePaused)
            stateManager.SwitchState(stateManager.disabledState);
        else if (stateManager.onGround && !Input.GetKey(stateManager.jumpKey))
            stateManager.SwitchState(stateManager.calmState);

        Jump();
        stateManager.Glide();
        stateManager.Walk();
    }

    public override void FixedUpdateState()
    {
        JumpForce();
    }

    public void StartJump()
    {
        jumping = true;
        if (stateManager.onGround) {
            stateManager.animations.PlayJump();
            jumpForce = stateManager.normalForce;
            jumpIteration = 60;
        }
    }

    public void Jump()
    {
        if (Input.GetKeyUp(stateManager.jumpKey))
            jumping = false;
    }

    public void JumpForce()
    {
        if (jumpIteration > 0 && jumping)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + jumpForce);

        if (jumpIteration > 0)
            jumpIteration -= 1;
        else
            jumpIteration = 0;
    }
}
