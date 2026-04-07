public class ElfHoldingState : ElfState
{
    public ElfHoldingState(ElfController elfController, ElfStateMachine stateMachine, string animBoolName) : base(elfController, stateMachine, animBoolName) { }

    public override void Enter() {
        base.Enter();
        elfController.SetVelocity(0, elfController._rigidbody.linearVelocity.y);
    }

    public override void Update() {
        base.Update();
        if (!isPlayerHoldingItem) {
            if (!elfController.isGrounded()) {
                stateMachine.ChangeState(elfController.airState);
            } else if (playerXMoveInput != 0) {
                stateMachine.ChangeState(elfController.moveState);
            } else {
                stateMachine.ChangeState(elfController.idleState);
            }
            return;
        }

        // 先处理交互，避免交互导致地面状态变化时丢失 interact 信号
        if (isPlayerStartInteract) {
            stateMachine.ChangeState(elfController.interactState);
            return;
        }
        if (playerXMoveInput != 0) {
            stateMachine.ChangeState(elfController.holdingMoveState);
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
        
        elfController.SetVelocity(0, elfController._rigidbody.linearVelocity.y);
    }

    public override void Exit() {
        base.Exit();
    }
}
