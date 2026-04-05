public class ElfHoldingState : ElfState
{
    public ElfHoldingState(ElfController elfController, ElfStateMachine stateMachine, string animBoolName) : base(elfController, stateMachine, animBoolName) { }

    public override void Enter() {
        base.Enter();
        elfController.SetVelocity(0, 0);
    }

    public override void Update() {
        base.Update();
        if (playerXMoveInput != 0)
            stateMachine.ChangeState(elfController.holdingMoveState);
        if (!elfController.isGrounded())
            stateMachine.ChangeState(elfController.holdingAirState); 
        if (isPlayerJumpInput && elfController.isGrounded())
            stateMachine.ChangeState(elfController.holdingJumpState);
        if (isPlayerStartInteract)
            stateMachine.ChangeState(elfController.interactState);
    }

    public override void Exit() {
        base.Exit();
    }
}