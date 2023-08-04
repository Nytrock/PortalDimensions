using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    private Rigidbody2D rb;
    private bool jumping;
    private bool wasJumped;
    private float jumpTime = 0;
    private float jumpControlTime = 2f;
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
        else if (stateManager.onGround && (!jumping || wasJumped))
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
        if (stateManager.onGround) {
            jumping = true;
            wasJumped = false;
            stateManager.animations.PlayJump();
            jumpForce = stateManager.normalForce;
            jumpTime = 0;
        }
    }

    public void Jump()
    {
        if (Input.GetKeyUp(stateManager.jumpKey))
            jumping = false;
    }

    public void JumpForce()
    {
        if (jumping) {
            if ((jumpTime += Time.fixedDeltaTime) < jumpControlTime)
                rb.AddForce(Vector2.up * jumpForce / (jumpTime * 10));
            else
                jumping = false;

            if (!stateManager.onGround)
                wasJumped = true;
        }
    }
}
