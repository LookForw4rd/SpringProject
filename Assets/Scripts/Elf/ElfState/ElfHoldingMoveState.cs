public class ElfHoldingMoveState : ElfState
{
    public ElfHoldingMoveState(ElfController elfController, ElfStateMachine stateMachine, string animBoolName) : base(elfController, stateMachine, animBoolName) { }

    public override void Enter() {
        base.Enter();
    }

    public override void Update() {
        base.Update();
        if (playerXMoveInput == 0) 
            stateMachine.ChangeState(elfController.holdingState);
        if (!elfController.isGrounded())
            stateMachine.ChangeState(elfController.holdingAirState); 
        if (isPlayerJumpInput && elfController.isGrounded())
            stateMachine.ChangeState(elfController.holdingJumpState);
        if (isPlayerStartInteract)
            stateMachine.ChangeState(elfController.interactState);
        
        elfController.SetVelocity(playerXMoveInput * elfController.moveSpeed, elfController._rigidbody.linearVelocity.y);
    }

    public override void Exit() {
        base.Exit();
    }
}