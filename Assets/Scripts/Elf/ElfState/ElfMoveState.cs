public class ElfMoveState : ElfState
{
    public ElfMoveState(ElfController elfController, ElfStateMachine stateMachine, string animBoolName) : base(elfController, stateMachine, animBoolName) { }

    public override void Enter() {
        base.Enter();
    }

    public override void Update() {
        base.Update();
        if (playerXMoveInput == 0) 
            stateMachine.ChangeState(elfController.idleState);
        if (!elfController.isGrounded())
            stateMachine.ChangeState(elfController.airState); 
        if (isPlayerJumpInput && elfController.isGrounded())
            stateMachine.ChangeState(elfController.jumpState);
        if (isPlayerHoldingItem)
            stateMachine.ChangeState(elfController.holdingState);
        
        elfController.SetVelocity(playerXMoveInput * elfController.moveSpeed, elfController._rigidbody.linearVelocity.y);
    }

    public override void Exit() {
        base.Exit();
    }
}