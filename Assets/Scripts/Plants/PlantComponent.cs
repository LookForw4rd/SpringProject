using UnityEngine;

public enum PlantInteractionType {
    Water,
    Grit
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))] 
// 所有植物组件的基类
public class PlantComponent : MonoBehaviour
{
    protected Animator anim;
    protected Collider2D baseCollider; // 获取基类碰撞体

    [Header("基础状态")]
    [SerializeField] protected bool isIrrigated = false;

    // 使用 Hash 提高性能
    protected static readonly int IsIrrigatedHash = Animator.StringToHash("IsIrrigated");

    protected virtual void Awake() {
        anim = GetComponent<Animator>();
        baseCollider = GetComponent<Collider2D>();
    }

    protected virtual void Start() {
        // 初始同步状态
        UpdateVisuals(true);
    }

    /// 核心状态切换接口
    /// <param name="state">true 为湿润，false 为干瘪</param>
    /// <param name="immediate">是否瞬间切换状态（跳过过渡动画）</param>
    public virtual void SetState(bool state, bool immediate = false) {
        if (isIrrigated == state && !immediate){
            return;
        }
        isIrrigated = state;
        UpdateVisuals(immediate);
    }

    /// 同步 Animator 参数
    protected virtual void UpdateVisuals(bool immediate) {
        if (anim == null){
            Debug.LogError("Animator component missing on " + gameObject.name);
            return;
        }

        // 设置布尔值，触发状态机连线
        anim.SetBool(IsIrrigatedHash, isIrrigated);

        // 如果需要瞬间完成（比如周目初始化、重置）
        if (immediate){
            anim.Play(isIrrigated ? "Irrigated" : "Withered", 0, 1f); // 强制播放到该状态的最后
        }
    }

    /// 预留给子类的局部交互接口
    public virtual void OnLocalInteract(PlantInteractionType interactionType) {
        // 基类不实现具体交互，仅供功能组件重写
    }
}
