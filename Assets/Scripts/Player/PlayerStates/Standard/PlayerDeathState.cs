using UnityEngine;

public class PlayerDeathState : PlayerBaseState
{
    private PlayerStateManager stateManager;

    private Material material;
    private float dissolveAmount;
    private float dissolveSpeed = 1f;

    public override void EnterState(PlayerStateManager player)
    {
        stateManager = player;
        stateManager.animations.SetAnimation("IsDeath", true);
        material = stateManager.GetDeathMaterial();

        LevelManager.levelManager.AddToScore("Death");
        ButtonFunctional.pauseEnable = false;
    }

    public override void ExitState()
    {

    }

    public override void FixedUpdateState()
    {

    }

    public override void UpdateState()
    {
        dissolveAmount = Mathf.Clamp(dissolveAmount + dissolveSpeed * Time.deltaTime, 0, 1.1f);
        material.SetFloat("_DissolveAmount", dissolveAmount);
        if (dissolveAmount == 1.1f)
            stateManager.GetRigidbody().drag = 1000;
    }
}
