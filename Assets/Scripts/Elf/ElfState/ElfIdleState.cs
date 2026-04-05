public class ElfIdleState : ElfState
{
    public ElfIdleState(ElfController elfController, ElfStateMachine stateMachine, string animBoolName) : base(elfController, stateMachine, animBoolName) { }

    public override void Enter() {
        base.Enter();
        elfController.SetVelocity(0, elfController._rigidbody.linearVelocity.y);
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
        
        elfController.SetVelocity(0, elfController._rigidbody.linearVelocity.y);
    }

    public override void Exit() {
        base.Exit();
    }
}