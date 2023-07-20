using UnityEngine;

public abstract class PlayerBaseState
{
    public abstract void EnterState(PlayerStateManager player);

    public abstract void ExitState();

    public abstract void UpdateState();
    public abstract void FixedUpdateState();
}
