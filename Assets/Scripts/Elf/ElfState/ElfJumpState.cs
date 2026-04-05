public class ElfJumpState : ElfState
{
    public ElfJumpState(ElfController elfController, ElfStateMachine stateMachine, string animBoolName) : base(elfController, stateMachine, animBoolName) { }

    public override void Enter() {
        base.Enter();
        elfController.SetVelocity(elfController._rigidbody.linearVelocity.x, elfController.jumpSpeed);
    }

    public override void Update() {
        base.Update();
        
        if (elfController._rigidbody.linearVelocity.y < 0)
            stateMachine.ChangeState(elfController.airState);
        if (isPlayerHoldingItem)
            stateMachine.ChangeState(elfController.holdingState);
    }

    public override void Exit() {
        base.Exit();
    }
}