public class ElfHoldingJumpState : ElfState
{
    public ElfHoldingJumpState(ElfController elfController, ElfStateMachine stateMachine, string animBoolName) : base(elfController, stateMachine, animBoolName) { }

    public override void Enter() {
        base.Enter();
        elfController.SetVelocity(elfController._rigidbody.linearVelocity.x, elfController.jumpSpeed);
    }

    public override void Update() {
        base.Update();
        
        if (elfController._rigidbody.linearVelocity.y < 0)
            stateMachine.ChangeState(elfController.holdingAirState);
        if (isPlayerStartInteract)
            stateMachine.ChangeState(elfController.interactState);
    }

    public override void Exit() {
        base.Exit();
    }
}