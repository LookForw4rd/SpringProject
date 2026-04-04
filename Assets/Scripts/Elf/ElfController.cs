using UnityEngine;
using UnityEngine.Tilemaps;

public class ElfController : MonoBehaviour
{
    // Unity组件
    public Rigidbody2D _rigidbody { get; private set; }
    public Animator _animator { get; private set; }
    
    // 相关地图对象
    private Tilemap groundTilemap;
    private Tilemap decorationTilemap;
    private LayerMask groundLayer;
    
    // 角色子对象判定点
    private Transform stepTileTransform; // 玩家所踩tile的判定位置对象（同时用于地面检测）
    
    // 角色初始设置参数
    public float moveSpeed = 4;
    public float jumpSpeed = 6;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.1f); // 地面检测箱长宽
    
    // 状态相关参数
    private ElfStateMachine stateMachine;
    public ElfIdleState idleState;
    public ElfMoveState moveState;
    public ElfJumpState jumpState;
    public ElfAirState airState;
    
    // 运行参数
    private bool isFacingRight = true;
    private Vector3Int lastSteppedTile = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue); // 保存玩家上一次踩过的tile，防止不动时重复计算tile的成长
    
    private void Awake() {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        groundTilemap = GameObject.Find("Tilemap_Ground").GetComponent<Tilemap>();
        decorationTilemap = GameObject.Find("Tilemap_Decoration").GetComponent<Tilemap>();
        groundLayer = LayerMask.GetMask("Ground");
        
        stepTileTransform = transform.Find("StepTileTransform");

        stateMachine = new ElfStateMachine();
        idleState = new ElfIdleState(this, stateMachine, "idle");
        moveState = new ElfMoveState(this, stateMachine, "move");
        jumpState = new ElfJumpState(this, stateMachine, "jump");
        airState = new ElfAirState(this, stateMachine, "air");
    }
    
    private void Start() {
        stateMachine.Initialize(idleState);
    }

    private void Update() {
        PlayerInputCheck();
        stateMachine.currentState.Update();
    }

    private void OnCollisionStay2D(Collision2D collision) {
        CalculateGrassStep(collision);
    }
    
    // 当玩家经过跳跃离开地面后回到地面时从新计算草地的踩他情况
    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.GetComponent<Tilemap>() != null)
            lastSteppedTile = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
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
        stateMachine.currentState.isPlayerJumpInput = Input.GetKeyDown(KeyCode.Space);
    }

    // 计算根据玩家踩踏地面导致的草地进化
    private void CalculateGrassStep(Collision2D collision) {
        if (collision.gameObject != groundTilemap.gameObject) return;
        
        Vector3 hitPosition = stepTileTransform.position;
        Vector3Int currentCell = groundTilemap.WorldToCell(hitPosition);

        if (currentCell != lastSteppedTile) {
            lastSteppedTile = currentCell;
            TileBase tile = groundTilemap.GetTile(currentCell);
            if (tile is SteppableGrassTile steppableGrass)
                steppableGrass.OnStepped(currentCell, groundTilemap, decorationTilemap);
        }
    }

    // 检测玩家是否位于地面
    public bool isGrounded() {
        return Physics2D.OverlapBox(stepTileTransform.position, groundCheckSize, 0f, groundLayer);
    }
    
    // 绘制gizmos，确认地面监测框位置及大小
    private void OnDrawGizmosSelected() {
        if (transform.Find("StepTileTransform") != null) {
            Transform stepPoint = transform.Find("StepTileTransform");
            Gizmos.color = Color.red; 
            Gizmos.DrawWireCube(stepPoint.position, groundCheckSize);
        }
    }
}
