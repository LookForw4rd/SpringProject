public class ElfAirState : ElfState
{
    public ElfAirState(ElfController elfController, ElfStateMachine stateMachine, string animBoolName) : base(elfController, stateMachine, animBoolName) { }

    public override void Enter() {
        base.Enter();
    }

    public override void Update() {
        base.Update();
        if (playerXMoveInput != 0)
            elfController.SetVelocity(playerXMoveInput * elfController.moveSpeed, elfController._rigidbody.linearVelocity.y);
        
        if (elfController.isGrounded())
            stateMachine.ChangeState(elfController.idleState);
    }

    public override void Exit() {
        base.Exit();
    }
}