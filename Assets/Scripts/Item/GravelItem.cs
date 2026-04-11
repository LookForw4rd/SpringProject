using UnityEngine;
using System.Collections.Generic;

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
        TriggerPlantInteraction(PlantInteractionType.Grit);

        // 2. 播放动画
        if (_animator != null) {
            _animator.SetBool("hold", false);
            _animator.SetTrigger("interact"); // 触发交互/消失动画
        } else {
            DestroyItemAfterInteract();
        }
    }

    private void TriggerPlantInteraction(PlantInteractionType interactionType) {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, effectRadius);
        HashSet<PlantComponent> targets = new HashSet<PlantComponent>();

        foreach (var hit in hitColliders) {
            // 只命中真正挂有 PlantComponent 的物体，避免 EffectArea 这类子触发器误触发 Core
            PlantComponent plantComp = hit.GetComponent<PlantComponent>();
            if (plantComp != null && targets.Add(plantComp)) {
                plantComp.OnLocalInteract(interactionType);
            }
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
