using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

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
    private Transform holdItemTransform; // 玩家握持物品时物品的位置（及持有）子对象
    
    // 角色初始设置参数
    public float moveSpeed = 4;
    public float jumpSpeed = 6;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.1f); // 地面检测箱长宽
    public float itemCheckRadius = 0.5f; // 检测可握持物件（目前只有水珠和花朵）的半径
    
    // 状态相关参数
    private ElfStateMachine stateMachine;
    public ElfIdleState idleState;
    public ElfMoveState moveState;
    public ElfJumpState jumpState;
    public ElfAirState airState;
    public ElfHoldingState holdingState;
    public ElfHoldingMoveState holdingMoveState;
    public ElfHoldingJumpState holdingJumpState;
    public ElfHoldingAirState holdingAirState;
    public ElfInteractState interactState;
    
    // 运行参数
    private bool isFacingRight = true;
    private Vector3Int lastSteppedTile = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue); // 保存玩家上一次踩过的tile，防止不动时重复计算tile的成长
    public IHoldable currentHeldItem = null; // 当前玩家正在握持的物品接口
    private Vector2 currentWindForce; // 当前承受的风力
    
    // 关联object & component
    private TutorialTextEffect tutorialText;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        groundTilemap = GameObject.Find("Tilemap_Ground").GetComponent<Tilemap>();
        decorationTilemap = GameObject.Find("Tilemap_Decoration").GetComponent<Tilemap>();
        groundLayer = LayerMask.GetMask("Ground") | LayerMask.GetMask("Plant");
        
        stepTileTransform = transform.Find("StepTileTransform");
        holdItemTransform = transform.Find("HoldItemTransform");

        stateMachine = new ElfStateMachine();
        idleState = new ElfIdleState(this, stateMachine, "idle");
        moveState = new ElfMoveState(this, stateMachine, "move");
        jumpState = new ElfJumpState(this, stateMachine, "jump");
        airState = new ElfAirState(this, stateMachine, "air");
        holdingState = new ElfHoldingState(this, stateMachine, "hold");
        holdingMoveState = new ElfHoldingMoveState(this, stateMachine, "hold_move");
        holdingJumpState = new ElfHoldingJumpState(this, stateMachine, "hold_jump");
        holdingAirState = new ElfHoldingAirState(this, stateMachine, "hold_air");
        interactState = new ElfInteractState(this, stateMachine, "interact");

        tutorialText = FindAnyObjectByType<TutorialTextEffect>();
    }
    
    private void Start() {
        stateMachine.Initialize(idleState);
    }

    private void Update() {
        PlayerInputCheck();
        TestInput();
        stateMachine.currentState.isPlayerHoldingItem = currentHeldItem != null;
        stateMachine.currentState.Update();
    }

    private void FixedUpdate() {
        SetWindForceY();
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
        // 计算当前经过风力加持后的x轴速度
        float windMultiplier = (currentHeldItem != null) ? currentHeldItem.GetWindForceMultiplier() : 1f;
        float finalXVelocity = xVelocity + (currentWindForce.x * windMultiplier);
        
        _rigidbody.linearVelocity = new Vector2(finalXVelocity, yVelocity);
        FlipController(xVelocity);
    }

    // 在每一帧检测风的纵向风力，当存在风力时施加纵向力
    private void SetWindForceY() {
        if (currentWindForce.y != 0) {
            float windMultiplier = (currentHeldItem != null) ? currentHeldItem.GetWindForceMultiplier() : 1f;
            _rigidbody.AddForce(new Vector2(0, currentWindForce.y * windMultiplier), ForceMode2D.Force);
        }
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
        if (Input.GetKeyDown(KeyCode.J)) PlayerTryToInteract(); // 按下j键（交互键）时玩家尝试进行与周围环境、握持物品的互动
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

    // 玩家所有点击交互后的可能交互行为的判定
    private void PlayerTryToInteract() {
        if (currentHeldItem == null) {
            // 优先判定：检测周围是否有掉落的 IHoldable 物品（如水珠） 
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, itemCheckRadius);
            IHoldable nearestItem = null;
            float minDistance = float.MaxValue;

            foreach (var col in colliders) {
                IHoldable holdable = col.GetComponent<IHoldable>();
                if (holdable != null && col.gameObject.activeInHierarchy) {
                    float dist = Vector2.Distance(transform.position, col.transform.position);
                    if (dist < minDistance) {
                        minDistance = dist;
                        nearestItem = holdable;
                    }
                }
            }
            if (nearestItem != null) {
                // 捡起最近的物件
                currentHeldItem = nearestItem;
                currentHeldItem.OnPickedUp(holdItemTransform);
                Debug.Log("捡起了最近的物件");
            }
            else {
                if (!GetFlowerFromNearby(colliders)) {
                    GetGravelFromTile(); // 最后检测是否能从脚底的gravel tile获取gravel
                }
            }
        }
        else {
            currentHeldItem.OnInteracted(); // 和握持的物品发生交互
            stateMachine.currentState.isPlayerStartInteract = true;
        }
    }
    
    // 玩家从脚下的沙砾tile中获取沙砾item的判定交互方法
    private void GetGravelFromTile() {
        Vector3 hitPosition = stepTileTransform.position;
        Vector3Int currentCell = groundTilemap.WorldToCell(hitPosition);
        TileBase tileUnderFeet = groundTilemap.GetTile(currentCell);

        if (tileUnderFeet is GravelTile gravelTile) {
            GameObject gravelItem = gravelTile.GenerateGravelItem(holdItemTransform);
            IHoldable holdable = gravelItem.GetComponent<IHoldable>();
            currentHeldItem = holdable;
            currentHeldItem.OnPickedUp(holdItemTransform);
        }
    }

    private bool GetFlowerFromNearby(Collider2D[] nearbyColliders) {
        PlantFlower nearestFlower = null;
        float minDistance = float.MaxValue;
        HashSet<PlantFlower> visitedFlowers = new HashSet<PlantFlower>();

        foreach (var col in nearbyColliders) {
            PlantFlower flower = col.GetComponentInParent<PlantFlower>();
            if (flower == null || !flower.gameObject.activeInHierarchy || !visitedFlowers.Add(flower)) {
                continue;
            }

            float dist = Vector2.Distance(transform.position, flower.transform.position);
            if (dist < minDistance) {
                minDistance = dist;
                nearestFlower = flower;
            }
        }

        if (nearestFlower == null) {
            return false;
        }

        if (!nearestFlower.TryHarvest(holdItemTransform, out IHoldable flowerItem)) {
            return false;
        }

        currentHeldItem = flowerItem;
        currentHeldItem.OnPickedUp(holdItemTransform);
        Debug.Log("采摘并握持了花朵");
        return true;
    }

    // 当前状态动画播放完毕后调取此方法通知状态
    public void SetCurrentClipFinished() {
        stateMachine.currentState.isCurrentClipFinished = true;
    }

    // 当气孔为玩家提供风力时调用此方式进行风力信息置入
    public void ApplyWindForce(Vector2 windForce) {
        currentWindForce = windForce;
    }

    // 测试相关逻辑的临时输入检测
    private void TestInput() {
        if (Input.GetKeyDown(KeyCode.Alpha8))
            tutorialText.PlayMoveTutorial();
    }

    // 给玩家施加一次纵向抬升，用于花朵等“跃升”交互效果
    public void ApplyVerticalBoost(float boostSpeed) {
        float nextY = Mathf.Max(_rigidbody.linearVelocity.y, boostSpeed);
        _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, nextY);
    }
}
