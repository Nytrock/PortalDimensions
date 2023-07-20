using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    private PlayerStateManager stateManager;
    public override void EnterState(PlayerStateManager player)
    {
        stateManager = player;
        stateManager.portalGun.enabled = true;

        stateManager.animations.SetAnimation("IsWalk", true);
    }

    public override void ExitState()
    {
        stateManager.animations.SetAnimation("IsWalk", false);
    }

    public override void FixedUpdateState()
    {

    }

    public override void UpdateState()
    {
        if (ButtonFunctional.isGamePaused)
            stateManager.SwitchState(stateManager.disabledState);
        else if (Input.GetKeyDown(stateManager.jumpKey) || !stateManager.onGround)
            stateManager.SwitchState(stateManager.jumpState);
        else if (!Input.GetKey(stateManager.walkLeftKey) && !Input.GetKey(stateManager.walkRightKey))
            stateManager.SwitchState(stateManager.calmState);

        stateManager.Walk();
    }
}
