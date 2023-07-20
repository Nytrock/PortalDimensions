using UnityEngine;

public class PlayerCalmState : PlayerBaseState
{
    private PlayerStateManager stateManager;

    public override void EnterState(PlayerStateManager player)
    {
        stateManager = player;
        stateManager.portalGun.enabled = true;

        stateManager.animations.SetAnimation("IsCalm", true);
    }

    public override void ExitState()
    {
        stateManager.animations.SetAnimation("IsCalm", false);
    }

    public override void FixedUpdateState()
    {

    }

    public override void UpdateState()
    {
        if (ButtonFunctional.isGamePaused)
            stateManager.SwitchState(stateManager.disabledState);
        else if (Input.GetKey(stateManager.walkLeftKey) || Input.GetKey(stateManager.walkRightKey))
            stateManager.SwitchState(stateManager.walkState);
        else if (Input.GetKeyDown(stateManager.jumpKey) || !stateManager.onGround)
            stateManager.SwitchState(stateManager.jumpState);

        stateManager.Glide();
    }
}
