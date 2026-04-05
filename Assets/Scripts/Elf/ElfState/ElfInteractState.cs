public class ElfInteractState : ElfState
{
    public ElfInteractState(ElfController elfController, ElfStateMachine stateMachine, string animBoolName) : base(elfController, stateMachine, animBoolName) { }

    public override void Enter() {
        base.Enter();
    }

    public override void Update() {
        base.Update();
        if (isCurrentClipFinished && elfController.isGrounded())
            stateMachine.ChangeState(elfController.idleState);
        if (isCurrentClipFinished && !elfController.isGrounded())
            stateMachine.ChangeState(elfController.airState);
    }

    public override void Exit() {
        base.Exit();
        if (elfController.currentHeldItem != null) 
            if (elfController.currentHeldItem.IsItemConsumedAfterInteract()) 
                elfController.currentHeldItem = null;
    }
}