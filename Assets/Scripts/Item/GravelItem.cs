using UnityEngine;
using UnityEngine.XR;

public class GravelItem : MonoBehaviour, IHoldable
{
    private Animator _animator;
    
    [Header("交互设置")]
    public float effectRadius = 1.5f; // 影响半径
    private bool _isInteracted = false;

    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    public bool IsItemConsumedAfterInteract() {
        return true; // 沙砾在使用之后会消失
    }

    public void OnPickedUp(Transform holdPoint) {
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        if (_animator != null) {
            _animator.SetBool("hold", true); // 确保参数名和水珠一致
        }
    }

    public void OnInteracted() {
        if (_isInteracted) {
            return;
        }
        HandleGravelEffect();
    }

    private void HandleGravelEffect() {
        _isInteracted = true;

        // 1. 范围探测：寻找周围的 PlantCore
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, effectRadius);
        foreach (var hit in hitColliders) {
            // 尝试获取 PlantCore 组件（包括父级，防止碰撞体在子物体上）
            PlantCore core = hit.GetComponentInParent<PlantCore>();
            if (core != null) {
                core.InteractWithGrit(); 
            }
        }

        // 2. 播放动画
        if (_animator != null) {
            _animator.SetBool("hold", false);
            _animator.SetTrigger("interact"); // 触发交互/消失动画
        } else {
            DestroyItemAfterInteract();
        }
    }

    public void DestroyItemAfterInteract() {
        // 寻找玩家并清空引用
        ElfController player = GetComponentInParent<ElfController>();
        if (player != null && player.currentHeldItem == (IHoldable)this) {
            player.currentHeldItem = null;
        }
        Destroy(gameObject);
    }

    public float GetWindForceMultiplier() {
        return 1f;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, effectRadius);
    }
}