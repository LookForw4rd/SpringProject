public class ElfHoldingJumpState : ElfState
{
    public ElfHoldingJumpState(ElfController elfController, ElfStateMachine stateMachine, string animBoolName) : base(elfController, stateMachine, animBoolName) { }

    public override void Enter() {
        base.Enter();
        elfController.SetVelocity(elfController._rigidbody.linearVelocity.x, elfController.jumpSpeed);
    }

    public override void Update() {
        base.Update();
        if (!isPlayerHoldingItem) {
            if (elfController._rigidbody.linearVelocity.y < 0) {
                stateMachine.ChangeState(elfController.airState);
            } else {
                stateMachine.ChangeState(elfController.jumpState);
            }
            return;
        }

        if (isPlayerStartInteract) {
            stateMachine.ChangeState(elfController.interactState);
            return;
        }
        if (elfController._rigidbody.linearVelocity.y < 0) {
            stateMachine.ChangeState(elfController.holdingAirState);
            return;
        }
    }

    public override void Exit() {
        base.Exit();
    }
}
