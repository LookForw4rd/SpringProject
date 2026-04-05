public class ElfHoldingAirState : ElfState
{
    public ElfHoldingAirState(ElfController elfController, ElfStateMachine stateMachine, string animBoolName) : base(elfController, stateMachine, animBoolName) { }

    public override void Enter() {
        base.Enter();
    }

    public override void Update() {
        base.Update();
        if (playerXMoveInput != 0)
            elfController.SetVelocity(playerXMoveInput * elfController.moveSpeed, elfController._rigidbody.linearVelocity.y);
        
        if (elfController.isGrounded())
            stateMachine.ChangeState(elfController.holdingState);
        if (isPlayerStartInteract)
            stateMachine.ChangeState(elfController.interactState);
    }

    public override void Exit() {
        base.Exit();
    }
}