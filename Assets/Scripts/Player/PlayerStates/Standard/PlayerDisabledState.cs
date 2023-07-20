using UnityEngine;

public class PlayerDisabledState : PlayerBaseState
{
    private PlayerStateManager stateManager;
    private PlayerBaseState lastState;
    private bool isPause;

    public override void EnterState(PlayerStateManager player)
    {
        stateManager = player;
        stateManager.GetRigidbody().velocity = Vector3.zero;

        lastState = stateManager.currentState;
        isPause = Time.timeScale == 0;
    }

    public override void ExitState()
    {

    }

    public override void FixedUpdateState()
    {

    }

    public override void UpdateState()
    {
        stateManager.animations.SetAnimation("IsCalm", stateManager.onGround);
        stateManager.animations.SetAnimation("IsJump", !stateManager.onGround);

        if (isPause && !ButtonFunctional.isGamePaused)
            stateManager.currentState = lastState;
    }
}
