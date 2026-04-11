using UnityEngine;
using System.Collections.Generic;

public class DripItem : MonoBehaviour, IHoldable
{
    private Animator _animator;
    private Rigidbody2D _rb;
    public float effectRadius = 1.5f; // 影响半径
    private bool _isInteracted = false;
    private bool _isHeld = false;
    private bool _isPendingDestroy = false; // 销毁时上锁，避免握持和触碰销毁同时触发的bug

    private void Awake() {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    public bool IsItemConsumedAfterInteract() {
        return true; // 水滴使用后消失
    }

    public void OnPickedUp(Transform holdPoint) {
        if (_isPendingDestroy) {
            return; // 如果正在销毁过程中，禁止被拾取
        }
        _isHeld = true;
        if (_rb != null) {
            _rb.simulated = false;
            _rb.linearVelocity = Vector2.zero; // 清除余速
        }
        GetComponent<Collider2D>().enabled = false;
        // 吸附到手部
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        
        if (_animator != null) {
            _animator.SetBool("hold", true);
        }
    }

    public void OnInteracted() {
        if (_isInteracted) {
            return;
        }
        HandleSplashing();
    }

    // 新增碰撞检测逻辑
    private void OnCollisionEnter2D(Collision2D collision) {
        // 如果已经在手里，或者已经触发过破碎，则不处理
        if (_isHeld || _isInteracted) {
            return;
        }
        // 检查碰撞到的物体层级是否为 Ground 或 Plant
        int layer = collision.gameObject.layer;
        if (layer == LayerMask.NameToLayer("Ground") || layer == LayerMask.NameToLayer("Plant")) {
            HandleSplashing();
        }
    }
    // 真正处理水滴溅射和消失的逻辑
    private void HandleSplashing() {
        if (_isInteracted) {
            return; // 已经在销毁过程中
        }
        TriggerPlantInteraction(PlantInteractionType.Water);
        _isInteracted = true;
        _isPendingDestroy = true; // 锁定销毁状态，防止被拾取
        if (_animator != null) {
            _animator.SetBool("hold", false);
            _animator.SetTrigger("interact"); // 播放溅射/消失动画
        } 
        else {
            DestroyItemAfterInteract();
        }
    }

    private void TriggerPlantInteraction(PlantInteractionType interactionType) {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, effectRadius);
        HashSet<PlantComponent> targets = new HashSet<PlantComponent>();

        foreach (var hit in hitColliders) {
            PlantComponent plantComp = hit.GetComponentInParent<PlantComponent>();
            if (plantComp != null && targets.Add(plantComp)) {
                plantComp.OnLocalInteract(interactionType);
            }

            CanActivateByCore plant = hit.GetComponent<CanActivateByCore>();
            if (plant != null) 
                plant.IrrigatedByDrip();
        }
    }
    // 由动画事件 (Animation Event) 在最后一帧调用
    public void DestroyItemAfterInteract() {
        // 寻找玩家并清空引用
        ElfController player = GetComponentInParent<ElfController>();
        if (player != null && player.currentHeldItem == (IHoldable)this) {
            player.currentHeldItem = null;
        }
        Destroy(gameObject);
    }

    public float GetWindForceMultiplier() {
        return 1.0f;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, effectRadius);
    }
}
