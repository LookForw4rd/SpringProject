public class ElfState
{
    // elfState基本引用
    protected ElfStateMachine stateMachine;
    protected ElfController elfController;
    private string animBoolName;
    
    // 用来提示当前state变化、及state内部逻辑相关的指示参数
    public float playerXMoveInput;
    public bool isPlayerJumpInput; // 是否输入跳跃键
    public bool isPlayerHoldingItem; // 玩家是否持有物品
    public bool isPlayerStartInteract; // 玩家是否开始和持有的物品进行交互
    public bool isCurrentClipFinished; // 当前播放的动画clip是否完成（通过animation event管理）

    public ElfState(ElfController elfController, ElfStateMachine stateMachine, string animBoolName) {
        this.elfController = elfController;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter() {
        elfController._animator.SetBool(animBoolName, true);
        isCurrentClipFinished = false;
    }

    public virtual void Update() {
        
    }

    public virtual void Exit() {
        elfController._animator.SetBool(animBoolName, false);
        isPlayerHoldingItem = false;
        isPlayerStartInteract = false;
    }
}