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
    }

    public override void Exit() {
        base.Exit();
    }
}