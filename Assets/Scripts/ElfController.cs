using UnityEngine;

public class ElfController : MonoBehaviour
{
    // Unity组件
    public Rigidbody2D _rigidbody { get; private set; }
    public Animator _animator { get; private set; }
    
    // 角色初始设置参数
    public float moveSpeed = 4;
    
    // 状态相关参数
    private ElfStateMachine stateMachine;
    public ElfIdleState idleState;
    public ElfMoveState moveState;
    
    // 运行参数
    private bool isFacingRight = true;
    
    private void Awake() {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        stateMachine = new ElfStateMachine();
        idleState = new ElfIdleState(this, stateMachine, "idle");
        moveState = new ElfMoveState(this, stateMachine, "move");
    }
    
    private void Start() {
        stateMachine.Initialize(idleState);
    }

    private void Update() {
        PlayerInputCheck();
        stateMachine.currentState.Update();
    }

    // 设置玩家rigidbody速度
    public void SetVelocity(float xVelocity, float yVelocity) {
        _rigidbody.linearVelocity = new Vector2(xVelocity, yVelocity);
        FlipController(xVelocity);
    }

    // 玩家角色转身检测方法，在给予玩家速度时检测速度方向和朝向是否违背，如果违背则进行转身
    private void FlipController(float xVelocity) {
        if (xVelocity > 0 && !isFacingRight) 
            Flip();
        else if (xVelocity < 0 && isFacingRight)
            Flip();
    }
    
    // 玩家角色转身
    private void Flip() {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    // 在controller每一帧更新时检查玩家输入的各个操作
    private void PlayerInputCheck() {
        stateMachine.currentState.playerXMoveInput = Input.GetAxisRaw("Horizontal");
    }
}
