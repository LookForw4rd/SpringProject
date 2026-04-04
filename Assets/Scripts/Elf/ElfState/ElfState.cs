public class ElfState
{
    // elfState基本引用
    protected ElfStateMachine stateMachine;
    protected ElfController elfController;
    private string animBoolName;
    
    // 用来提示当前state变化、及state内部逻辑相关的指示参数
    public float playerXMoveInput;

    public ElfState(ElfController elfController, ElfStateMachine stateMachine, string animBoolName) {
        this.elfController = elfController;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter() {
        elfController._animator.SetBool(animBoolName, true);
    }

    public virtual void Update() {
        
    }

    public virtual void Exit() {
        elfController._animator.SetBool(animBoolName, false);
    }
}