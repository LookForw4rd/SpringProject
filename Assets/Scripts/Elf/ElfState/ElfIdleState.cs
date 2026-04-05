public class ElfIdleState : ElfState
{
    public ElfIdleState(ElfController elfController, ElfStateMachine stateMachine, string animBoolName) : base(elfController, stateMachine, animBoolName) { }

    public override void Enter() {
        base.Enter();
        elfController.SetVelocity(0, 0);
    }

    public override void Update() {
        base.Update();
        if (playerXMoveInput != 0)
            stateMachine.ChangeState(elfController.moveState);
        if (!elfController.isGrounded())
            stateMachine.ChangeState(elfController.airState); 
        if (isPlayerJumpInput && elfController.isGrounded())
            stateMachine.ChangeState(elfController.jumpState);
        if (isPlayerHoldingItem)
            stateMachine.ChangeState(elfController.holdingState);
    }

    public override void Exit() {
        base.Exit();
    }
}