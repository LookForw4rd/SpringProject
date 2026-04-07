public class ElfHoldingAirState : ElfState
{
    public ElfHoldingAirState(ElfController elfController, ElfStateMachine stateMachine, string animBoolName) : base(elfController, stateMachine, animBoolName) { }

    public override void Enter() {
        base.Enter();
    }

    public override void Update() {
        base.Update();
        if (!isPlayerHoldingItem) {
            if (elfController.isGrounded()) {
                if (playerXMoveInput != 0) {
                    stateMachine.ChangeState(elfController.moveState);
                } else {
                    stateMachine.ChangeState(elfController.idleState);
                }
            } else {
                stateMachine.ChangeState(elfController.airState);
            }
            return;
        }

        if (isPlayerStartInteract) {
            stateMachine.ChangeState(elfController.interactState);
            return;
        }
        if (elfController.isGrounded()) {
            stateMachine.ChangeState(elfController.holdingState);
            return;
        }

        if (playerXMoveInput != 0)
            elfController.SetVelocity(playerXMoveInput * elfController.moveSpeed, elfController._rigidbody.linearVelocity.y);
    }

    public override void Exit() {
        base.Exit();
    }
}
