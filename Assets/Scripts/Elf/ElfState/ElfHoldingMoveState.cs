public class ElfHoldingMoveState : ElfState
{
    public ElfHoldingMoveState(ElfController elfController, ElfStateMachine stateMachine, string animBoolName) : base(elfController, stateMachine, animBoolName) { }

    public override void Enter() {
        base.Enter();
    }

    public override void Update() {
        base.Update();
        if (!isPlayerHoldingItem) {
            if (!elfController.isGrounded()) {
                stateMachine.ChangeState(elfController.airState);
            } else if (playerXMoveInput == 0) {
                stateMachine.ChangeState(elfController.idleState);
            } else {
                stateMachine.ChangeState(elfController.moveState);
            }
            return;
        }

        if (isPlayerStartInteract) {
            stateMachine.ChangeState(elfController.interactState);
            return;
        }
        if (playerXMoveInput == 0) {
            stateMachine.ChangeState(elfController.holdingState);
            return;
        }
        if (!elfController.isGrounded()) {
            stateMachine.ChangeState(elfController.holdingAirState);
            return;
        }
        if (isPlayerJumpInput && elfController.isGrounded()) {
            stateMachine.ChangeState(elfController.holdingJumpState);
            return;
        }
        
        elfController.SetVelocity(playerXMoveInput * elfController.moveSpeed, elfController._rigidbody.linearVelocity.y);
    }

    public override void Exit() {
        base.Exit();
    }
}
